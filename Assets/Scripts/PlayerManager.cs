using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject modelPrefab;
    [SerializeField] private float mouseSensitivity = 200.0f;
    private GameObject model;
    private GameObject playerCamera;
     [SerializeField] private bool debug = false;
    private Vector3 cameraRotation = Vector3.zero;
    [SerializeField] private float cameraMinRange = -30.0f;
    [SerializeField] private float cameraMaxRange = 60.0f;

    [SerializeField] private float walkSpeed = 4.0f;
    [SerializeField] private float runSpeed = 6.0f;
    [SerializeField] private float jumpHeight = 0.6f;
    [SerializeField] private float gravityAcceleration = -9.81f;
    
    private CharacterController controller;
    private PlayerStatus playerStatus;
    private Vector3 velocity = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    void Start()
    {
        setModel(modelPrefab);
        playerCamera = transform.Find("Camera").gameObject;
        Cursor.lockState = CursorLockMode.Locked;
        refreshCameraPosition();
        
        controller = GetComponent<CharacterController>(); //shift alt down
        playerStatus = GetComponent<PlayerStatus>();
        setController(0.55f, 0.2f);
    }

    void Update()
    {
        if(debug)
            shootingTest();
        gravity();
        movement();
        mouse();
    }

void shootingTest()
{
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            Debug.DrawRay(ray.origin, ray.direction*20.0f, Color.red);
            RaycastHit hit;
            if(Input.GetMouseButtonDown(0))
            {
                if(Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.transform.gameObject);
                    HP_Manager HP = hit.transform.gameObject.GetComponent<HP_Manager>();
                    if(HP)
                        HP.takeDamage(25.0f);
                    }  
            }

}

    private void setModel(GameObject prefab)
    {
        if(prefab != null)
        {
            model = Instantiate(prefab, this.transform);
            model.name = "Model";
            model.transform.localPosition = Vector3.zero;
        }
    }
    private void refreshCameraPosition()
    {
        if(model != null)
        {
            Vector3 headPoint = model.transform.Find("Hips/Spine/Spine1/Spine2/Neck/Head/HeadTop_End").position;
            Vector3 position = headPoint + new Vector3(0.0f, 0.1f, 0.12f) - transform.position;
            playerCamera.transform.localPosition = position;
            playerCamera.transform.localRotation = Quaternion.identity;
            cameraRotation = Vector3.zero;
        }
    }


    void gravity()
    {
        //velocity = acceleration * time
        float lossOfVelocity = gravityAcceleration * Time.deltaTime;
        if(controller.isGrounded)
                velocity.y = lossOfVelocity;
        else
            velocity.y += lossOfVelocity;
    }
    void mouse()
    {
        //distance = velocity * time
        float mouseX = mouseSensitivity* Input.GetAxis("Mouse X") * Time.deltaTime;
        float mouseY = mouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;
        Vector3 rotation = new Vector3(-mouseY, 0, 0);
        cameraRotation += rotation;
        cameraRotation.x =  Mathf.Clamp(cameraRotation.x, cameraMinRange, cameraMaxRange);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotation);
        transform.Rotate(Vector3.up * mouseX);
    }

    void movement()
    {
        direction = Vector3.zero;
        direction += transform.right * Input.GetAxis("Horizontal") ;
        direction += transform.forward * Input.GetAxis("Vertical");
        float speed = 0.0f;
        
        if(controller.isGrounded)
        {//WASD

            if(direction.x != 0.0f || direction.z != 0.0f)
                {
                    speed = Input.GetKey(KeyCode.LeftShift)? runSpeed : walkSpeed;
                    playerStatus.state = Input.GetKey(KeyCode.LeftShift)? PlayerStatus.State.Run : PlayerStatus.State.Walk;
                }
            else
                playerStatus.state = PlayerStatus.State.Idlee;
            
            //JUMP
            if(Input.GetKey(KeyCode.Space) && controller.isGrounded)
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityAcceleration);
                    playerStatus.state = PlayerStatus.State.Jump;
                }
        
            velocity.x = speed*direction.x;
            velocity.z = speed*direction.z;
        }
        else
        {
            if(direction.x != 0.0f || direction.z != 0.0f)
                speed = walkSpeed;
            if(velocity.y > 0)
                playerStatus.state = PlayerStatus.State.Rises; 
            else
                playerStatus.state = PlayerStatus.State.Falls; 
        }
        //MOVE BY DISTANCE (distance = velocity * time)
        Vector3 distance = velocity * Time.deltaTime;
        controller.Move(distance);
    }
    

    void setController(float radius,float stepOffset)
    {
        Vector3 headPoint = transform.Find("Model/Hips/Spine/Spine1/Spine2/Neck/Head/HeadTop_End").position;
        controller.height = headPoint.y - transform.position.y;
        controller.radius = radius;
        controller.skinWidth = 0.1f * controller.radius;
        controller.center = new Vector3(0.0f, ((controller.height/2.0f) + controller.skinWidth) ,0.0f);
        controller.minMoveDistance = 0.0f;
        controller.stepOffset = stepOffset;
    }

    

    public Vector3 getVelocity()
    {
        return velocity;
    }
    public Vector3 getDirection()
    {
        return direction;
    }
    public bool getIsGrounded()
    {
        return controller.isGrounded;
    }

}

