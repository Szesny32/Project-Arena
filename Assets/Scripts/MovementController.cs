using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{

    public float walkSpeed = 4.0f;
    public float runSpeed = 6.0f;
    public float jumpHeight = 0.6f;
    public float gravityAcceleration = -9.81f;
    

    private CharacterController controller;
    private Vector3 velocity = Vector3.zero;
    private Vector3 direction = Vector3.zero;

    private GameObject playerCamera;
    private PlayerManager playerManager;
    private PlayerStatus playerStatus;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerManager = GetComponent<PlayerManager>();
        playerStatus = GetComponent<PlayerStatus>();
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
        Vector3 direction = Vector3.zero;

        direction += transform.right * Input.GetAxis("Horizontal") ;
        direction += transform.forward * Input.GetAxis("Vertical");
        
        float speed;
        if(direction.x != 0.0f || direction.z != 0.0f)
        {
            bool sprint = Input.GetKey(KeyCode.LeftShift);
            playerStatus.state = sprint? PlayerStatus.State.Run : PlayerStatus.State.Walk;
            speed = sprint? runSpeed : walkSpeed;
        }
        else
        {
            playerStatus.state = PlayerStatus.State.Idlee;
            speed = 0.0f;
        }

        //JUMP
        if(controller.isGrounded)
        {
            if(Input.GetKey(KeyCode.Space))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityAcceleration);
                playerStatus.state = PlayerStatus.State.Jump;
            }
        }
        else
        {
            if(velocity.y > 0)
                playerStatus.state = PlayerStatus.State.Rises; 
            else
                playerStatus.state = PlayerStatus.State.Falls; 
        }


        velocity.x = speed*direction.x;
        velocity.z = speed*direction.z;
        
        //MOVE BY DISTANCE (distance = velocity * time)
        Vector3 distance = velocity * Time.deltaTime;
        controller.Move(distance);
    }

    void mouse()
    {
        //distance = velocity * time
        float mouseX = playerManager.mouseSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
        float mouseY = playerManager.mouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;
        playerManager.cameraRotation.x -= mouseY;
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

}
