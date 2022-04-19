using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    [SerializeField] private float walkSpeed = 4.0f;
    [SerializeField] private float runSpeed = 6.0f;
    [SerializeField] private float jumpHeight = 0.6f;
    [SerializeField] private float gravityAcceleration = -9.81f;
    
    private CharacterController controller;
    private Vector3 velocity = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    private GameObject playerCamera;
    private PlayerManager playerManager;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerManager = GetComponent<PlayerManager>();
        setController(0.55f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {

        gravity();
        movement();
        mouse();
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

    void movement()
    {
        //WASD
        direction = Vector3.zero;
        direction += transform.right * Input.GetAxis("Horizontal") ;
        direction += transform.forward * Input.GetAxis("Vertical");
        
        float speed = 0.0f;
        if(direction.x != 0.0f || direction.z != 0.0f)
            speed = Input.GetKey(KeyCode.LeftShift)? runSpeed : walkSpeed;
        
        //JUMP
        if(Input.GetKey(KeyCode.Space) && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityAcceleration);
      
        velocity.x = speed*direction.x;
        velocity.z = speed*direction.z;
        
        //MOVE BY DISTANCE (distance = velocity * time)
        Vector3 distance = velocity * Time.deltaTime;
        controller.Move(distance);
    }

    void mouse()
    {
        //distance = velocity * time
        float mouseX = playerManager.MouseSenstivity() * Input.GetAxis("Mouse X") * Time.deltaTime;
        float mouseY = playerManager.MouseSenstivity() * Input.GetAxis("Mouse Y") * Time.deltaTime;
        playerManager.rotateCamera(new Vector3(-mouseY, 0, 0));
        transform.Rotate(Vector3.up * mouseX);
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
