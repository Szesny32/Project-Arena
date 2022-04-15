using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public float gravityAcceleration = -9.81f;
    public float walkSpeed = 4.0f;
    public float runSpeed = 6.0f;
    public float jumpHeight = 0.6f;
    public float mouseSensitivity = 300.0f;
    public bool showHead = true;

    private CharacterController controller;
    private Camera firstPersonCamera;
    private GameObject playerModel;
    private Renderer head;

    private Vector3 cameraRotation = Vector3.zero;
    private Vector3 playerVelocity = Vector3.zero;


    void Start()
    {
        //MODEL
        //playerPrefab = Resources.Load("Robot_Soldier_Camo2") as GameObject;
        playerModel =  Instantiate(playerPrefab, this.transform);
        playerModel.name = "PlayerModel"; 
        playerModel.transform.localPosition = Vector3.zero;
        Vector3 headPoint = this.transform.Find("PlayerModel/Hips/Spine/Spine1/Spine2/Neck/Head/HeadTop_End").position;
        head = this.transform.Find("PlayerModel/Robot_Soldier_Head").gameObject.GetComponent<Renderer>(); 

        //CHARACTER CONTROLLER
        controller = gameObject.AddComponent<CharacterController>();
        controller.height = headPoint.y - transform.position.y;
        
        //controller.transform.localPosition = Vector3.zero;
        controller.radius = 0.55f;
        controller.skinWidth = 0.1f * controller.radius;
        controller.center = new Vector3(0.0f, controller.height/2.0f + controller.skinWidth,0.0f);
        controller.minMoveDistance = 0.0f;
        controller.stepOffset = 0.2f;
   
        //CAMERA
        Vector3 cameraPos = new Vector3(0f, headPoint.y + 0.1f ,0.12f);
        Cursor.lockState = CursorLockMode.Locked;
        GameObject PlayerCamera = new GameObject("Player Camera");

        PlayerCamera.transform.parent = this.transform;
        PlayerCamera.transform.localPosition = cameraPos;
        PlayerCamera.transform.localRotation = Quaternion.identity;
        firstPersonCamera = PlayerCamera.AddComponent<Camera>();
        firstPersonCamera.enabled = true;
    }

    void Update()
    {
        movement();
        mouse();
        head.enabled = showHead;
    }

    void movement()
    {
        //SIDEWAYS MOVEMENT (X/Z)
        float playerSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed: walkSpeed;
        Vector3 direction = firstPersonCamera.transform.right * Input.GetAxis("Horizontal") + firstPersonCamera.transform.forward * Input.GetAxis("Vertical");
        playerVelocity.x = playerSpeed*direction.x;
        playerVelocity.z = playerSpeed*direction.z;

        //UP-DOWN MOVEMENT [V = a * t]
        float lossOfVelocity = gravityAcceleration * Time.deltaTime;
        playerVelocity.y += lossOfVelocity;
        if(controller.isGrounded)
        {
            if(Input.GetKey(KeyCode.Space))
                playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityAcceleration);
            else
                playerVelocity.y = lossOfVelocity;
        }
            
        //MOVE BY DISTANCE [s = V * t]
        Vector3 distance = playerVelocity * Time.deltaTime;
        controller.Move(distance);
    }

    void mouse()
    {
        //[s = V * t]
        float mouseX = mouseSensitivity *Input.GetAxis("Mouse X") * Time.deltaTime;
        float mouseY = mouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;
        cameraRotation.x -= mouseY;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, -30.0f, 60.0f);

        transform.Rotate(Vector3.up * mouseX);
        firstPersonCamera.transform.localRotation = Quaternion.Euler(cameraRotation.x, 0f, 0f);
    }
}