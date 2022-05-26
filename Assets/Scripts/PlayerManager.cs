using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerManager : NetworkBehaviour {
    //[SerializeField] private GameObject modelPrefab;
    [SerializeField] private float mouseSensitivity = 200.0f;
    public GameObject model;
    [SerializeField] private  GameObject playerCamera;
    [SerializeField] private bool debug = false;
    private Vector3 cameraRotation = Vector3.zero;
    [SerializeField] private float cameraMinRange = -30.0f;
    [SerializeField] private float cameraMaxRange = 60.0f;

    [SerializeField] private float crouchSpeed = 2.0f;
    [SerializeField] private float walkSpeed = 4.0f;
    [SerializeField] private float runSpeed = 6.0f;
    [SerializeField] private float jumpHeight = 0.6f;
    [SerializeField] private float gravityAcceleration = -9.81f;
    [SerializeField] private NetworkVariable<Vector3> networkPositionDirection = new NetworkVariable<Vector3>();
    [SerializeField] private NetworkVariable<Vector3> networkRotationDirection = new NetworkVariable<Vector3>();
    [SerializeField] private NetworkVariable<float> networkDirectionMagnitude = new NetworkVariable<float>();
   
    [SerializeField] private LayerMask layerMask;
    [SerializeField] public NetworkVariable<int> team = new NetworkVariable<int>();
    private int prevTeam;
    public Texture _red, _blue;
    private CharacterController controller;
    private PlayerStatus playerStatus;
    private AudioSource audioSource;
    private AudioClip clip;
    private Vector3 velocity = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    private PlayerStatus.State playerPrevState;
    private HUD_Manager HUD;
    private float timer = 0f;
    private float shootDelay = 0.3f;
    private float shootTimer = 0.0f;
    private float reloadDelay = 1.0f;
    private float reloadTimer = 0f;
    

    private float[] xStartPos = { -2.38f, 5.16f, 5.71f, -4.507f, -2.21f, 1.12f};
    private float[] yStartPos = { 0f, 0f,0f,0f,1.432f, 1.432f};
    private float[] zStartPos = { -2.55f, -2.63f,8.78f, 8.299f, 2.302f, 1.71f};

    public bool friendlyFire = false;
    void Start() {
        // setModel(modelPrefab);
        //playerCamera = transform.Find("Camera").gameObject;

        if (!IsLocalPlayer) {
            playerCamera.GetComponent<Camera>().enabled = false;
            GetComponent<AudioListener>().enabled = false;
            return;
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        refreshCameraPosition();

        controller = GetComponent<CharacterController>(); //shift alt down
        playerStatus = GetComponent<PlayerStatus>();
        audioSource = GetComponent<AudioSource>();
        clip = Resources.Load("Scifi Guns SFX Pack/Gun2_1");
        setController(0.55f, 0.2f);


        model.transform.Find("Robot_Soldier_Head").GetComponent<Renderer>().enabled = false;
        //model.transform.Find("Robot_Soldier_Body").GetComponent<Renderer>().enabled = false;
       // model.transform.Find("Robot_Soldier_Feet").GetComponent<Renderer>().enabled = false;
        //model.transform.Find("Robot_Soldier_Legs2").GetComponent<Renderer>().enabled = false;
        HUD = GetComponent<HUD_Manager>(); 
        
         int spot = Random.Range(0, 5);
        transform.position =  new Vector3(xStartPos[spot] + Random.Range(-0.5f, 0.5f), yStartPos[spot], zStartPos[spot]+ Random.Range(-0.5f, 0.5f));

    }


    public float getDirectionMagnitude()
    {
        return networkDirectionMagnitude.Value;
    }

    void Update()
    {
        timer+=Time.deltaTime;
        if(timer < 0.5f)
            return;

        reloadTimer -= Time.deltaTime;
        shootTimer -= Time.deltaTime;
        if(prevTeam!=team.Value)
            setTexture();

        if (IsLocalPlayer) 
        {

        if(reloadTimer > 0)
        {
            HUD.CircleFill.fillAmount = (Mathf.Clamp(reloadTimer,0f,reloadDelay)/reloadDelay);
  
        }
        else
        {
            HUD.AmmoImage.SetActive(true);
            HUD.Reloading_Circle.SetActive(false);
        }


            gravity();
            if(HUD.HP.Value!=0f)
            {
                if(reloadTimer<=0f)
                    shootingTest();
                mouse();
                movement();
            }
            else
                playerStatus.UpdateStatusServerRpc(PlayerStatus.State.Dying);
            

            UpdateClientServerRpc(transform.position, transform.eulerAngles);
            UpdateDirectionMagnitudeServerRpc(Mathf.Clamp01(direction.magnitude));
            refreshCameraPosition();
        }
        prevTeam = team.Value;
    }




    void shootingTest() 
    {
        float range = 20f;
        float damage = 10f;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red);

        RaycastHit hit;



        if (Input.GetMouseButton(1) && Input.GetMouseButton(0) && shootTimer<=0f) 
        {
            if(HUD.ammunition>0)
            {
                shootTimer = shootDelay;
                HUD.ammunition--;
                
                //TS & AG :: Animacja wystrzału
<<<<<<< Updated upstream
                //KJ :: Dodać dźwięk wystrzału
=======
                //JK :: Dodać dźwięk wystrzału
                audioSource.clip = clip;
                audioSource.PlayOneShot(clip, 0.7f);
>>>>>>> Stashed changes
                
                if(Physics.Raycast (ray, out hit, range, layerMask))
                {
                    //BS :: Stworzenie smugi wystrzału
                    BodyPart bodyPart = hit.transform.GetComponent<BodyPart>();
                    if(bodyPart)
                    {
                        bool enemy = hit.transform.root.GetComponent<PlayerManager>().team.Value != team.Value;
                        if(enemy || friendlyFire)
                            bodyPart.inflictDamage(damage);
                    }
                }
            }
            else
            {
                //KJ :: Dodać dźwięk braku amunicji
            }
        }
        else if(Input.GetKeyDown(KeyCode.R) && reloadTimer <= 0)
        {
            reloadTimer = reloadDelay;
            HUD.ammunition = HUD.maxAmmunition;
            HUD.AmmoImage.SetActive(false);
            HUD.Reloading_Circle.SetActive(true);
            HUD.CircleFill.fillAmount = 1.0f;
            //MG :: Przeładowanie w Hudzie
        }
    }
    



    private void refreshCameraPosition() {
        if (model != null) {
            Vector3 headPoint = model.transform.Find("Hips/Spine/Spine1/Spine2/Neck/Head/HeadTop_End").position;
            Vector3 position = headPoint;
            playerCamera.transform.position = position;
        }
    }

    [ServerRpc]
    public void UpdateClientServerRpc(Vector3 newDirection, Vector3 newRotation) {
        networkPositionDirection.Value = newDirection;
        networkRotationDirection.Value = newRotation;
    }
    

    [ServerRpc]
    public void UpdateDirectionMagnitudeServerRpc(float newDirectionMagnitude) {
        networkDirectionMagnitude.Value = newDirectionMagnitude;
    }


    void gravity() 
    {
        float lossOfVelocity = gravityAcceleration * Time.deltaTime;
        if (controller.isGrounded)
            velocity.y = lossOfVelocity;
        else
            velocity.y += lossOfVelocity;
    }

    void mouse() 
    {
        float mouseX = mouseSensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
        float mouseY = mouseSensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;
        Vector3 rotation = new Vector3(-mouseY, 0, 0);
        cameraRotation += rotation;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, cameraMinRange, cameraMaxRange);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotation);
        transform.Rotate(Vector3.up * mouseX);
    }

    void movement() 
    {
        direction = Vector3.zero;
        direction += transform.right * Input.GetAxis("Horizontal");
        direction += transform.forward * Input.GetAxis("Vertical");
        float speed = 0.0f;

        if (controller.isGrounded) {//WASD

            if (direction.x != 0.0f || direction.z != 0.0f) 
            {
                if (Input.GetKey(KeyCode.LeftShift)) 
                {
                    speed = runSpeed;
                    playerStatus.UpdateStatusServerRpc(PlayerStatus.State.Run);
                }
                else if (Input.GetKey(KeyCode.LeftControl)) 
                {
                    speed = crouchSpeed;
                    playerStatus.UpdateStatusServerRpc(PlayerStatus.State.CrouchMove);
                } 
                else 
                {
                    speed = walkSpeed;
                    if(Input.GetMouseButton(1) && reloadTimer<=0f)
                        playerStatus.UpdateStatusServerRpc(PlayerStatus.State.WalkingAim);
                    else
                        playerStatus.UpdateStatusServerRpc(PlayerStatus.State.Walk);
                }
            } 
            else
            {
                if (Input.GetKey(KeyCode.LeftControl)) 
                {
                    if(Input.GetMouseButton(1)&& reloadTimer<=0)
                        playerStatus.UpdateStatusServerRpc(PlayerStatus.State.CrouchAim);
                    else
                        playerStatus.UpdateStatusServerRpc(PlayerStatus.State.Crouch);  
                }
                else if(Input.GetMouseButton(1) && reloadTimer<=0)
                    playerStatus.UpdateStatusServerRpc(PlayerStatus.State.IdleAim);
                else
                    playerStatus.UpdateStatusServerRpc(PlayerStatus.State.Idlee);
                
            }
               

            //JUMP
            if (Input.GetKey(KeyCode.Space) && controller.isGrounded) {
                velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityAcceleration);
                playerStatus.UpdateStatusServerRpc(PlayerStatus.State.Jump);
            }

            velocity.x = speed * direction.x;
            velocity.z = speed * direction.z;
        } 
        else 
        {
            if (direction.x != 0.0f || direction.z != 0.0f)
                speed = walkSpeed;
            if (velocity.y > 0)
                playerStatus.UpdateStatusServerRpc(PlayerStatus.State.Rises);
            else
                playerStatus.UpdateStatusServerRpc(PlayerStatus.State.Falls);
        }
        //MOVE BY DISTANCE (distance = velocity * time)
        Vector3 distance = velocity * Time.deltaTime;
        controller.Move(distance);
    }




    void setController(float radius, float stepOffset) {
        Vector3 headPoint = transform.Find("Model/Hips/Spine/Spine1/Spine2/Neck/Head/HeadTop_End").position;
        controller.height = headPoint.y - transform.position.y;
        controller.radius = radius;
        controller.skinWidth = 0.1f * controller.radius;
        controller.center = new Vector3(0.0f, ((controller.height / 2.0f) + controller.skinWidth), 0.0f);
        controller.minMoveDistance = 0.0f;
        controller.stepOffset = stepOffset;
    }

    void updateControllerHeight() {
        Vector3 headPoint = transform.Find("Model/Hips/Spine/Spine1/Spine2/Neck/Head/HeadTop_End").position;
        controller.height = headPoint.y - transform.position.y;
        controller.center = new Vector3(0.0f, ((controller.height / 2.0f) + controller.skinWidth), 0.0f);
    }



    public Vector3 getVelocity() {
        return velocity;
    }
    public Vector3 getDirection() {
        return direction;
    }
    public bool getIsGrounded() {
        return controller.isGrounded;
    }



    [ServerRpc(RequireOwnership = false)]
    public void UpdateTeamServerRpc(int newTeam) 
    {

        team.Value = newTeam;
    }

    private void setTexture()
    {   
        Texture _team = _red;
        if(team.Value==1)
            _team = _red;
        else if(team.Value==2)
           _team = _blue;
        transform.Find("Model/Robot_Soldier_Arms1").GetComponent<Renderer>().material.SetTexture("_MainTex", _team);
        transform.Find("Model/Robot_Soldier_Arms2").GetComponent<Renderer>().material.SetTexture("_MainTex", _team);
        transform.Find("Model/Robot_Soldier_Body").GetComponent<Renderer>().material.SetTexture("_MainTex", _team);
        transform.Find("Model/Robot_Soldier_Feet").GetComponent<Renderer>().material.SetTexture("_MainTex", _team);
        transform.Find("Model/Robot_Soldier_Head").GetComponent<Renderer>().material.SetTexture("_MainTex", _team);
        transform.Find("Model/Robot_Soldier_Legs2").GetComponent<Renderer>().material.SetTexture("_MainTex", _team);
    }

}

