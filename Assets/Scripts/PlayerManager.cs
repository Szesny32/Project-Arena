using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Collections;

public class PlayerManager : NetworkBehaviour {
    //[SerializeField] private GameObject modelPrefab;
    [SerializeField] private float mouseSensitivity = 200.0f;
    public GameObject model;
    [SerializeField] private  GameObject playerCamera;
    //[SerializeField] private bool debug = false;
    private Vector3 cameraRotation = Vector3.zero;
    [SerializeField] private float cameraMinRange = -30.0f;
    [SerializeField] private float cameraMaxRange = 60.0f;
    [SerializeField] private GameObject TeamPanel;
    [SerializeField] private Text inputName;
    public NetworkVariable<FixedString64Bytes> playerName =  new NetworkVariable<FixedString64Bytes>();
    private float crouchSpeed = 2f;
    private float walkSpeed = 2.5f;
    private float runSpeed = 4.0f;
    [SerializeField] private float jumpHeight = 0.6f;
    [SerializeField] private float gravityAcceleration = -9.81f;
    [SerializeField] private NetworkVariable<Vector3> networkPositionDirection = new NetworkVariable<Vector3>();
    [SerializeField] private NetworkVariable<Vector3> networkRotationDirection = new NetworkVariable<Vector3>();
    [SerializeField] private NetworkVariable<float> networkDirectionMagnitude = new NetworkVariable<float>();
   
    [SerializeField] private LayerMask layerMask;
    [SerializeField] public NetworkVariable<int> team = new NetworkVariable<int>();
    [SerializeField] public NetworkVariable<int> shoot = new NetworkVariable<int>();
    public int shootL;

 
    private int prevTeam;
    public Texture _red, _blue;
    private CharacterController controller;
    private PlayerStatus playerStatus;
    
    private Vector3 velocity = Vector3.zero;
    private Vector3 direction = Vector3.zero;
    private HUD_Manager HUD;
    private float timer = 0f;
    private float shootDelay = 0.3f;
    private float shootTimer = 0.0f;
    private float reloadDelay = 1.0f;
    private float reloadTimer = 0f;
    private float sensitivity = 0f;

    private float[] xStartPos = { -2.38f, 5.16f, 5.71f, -4.507f, -2.21f, 1.12f};
    private float[] yStartPos = { 0f, 0f,0f,0f,1.432f, 1.432f};
    private float[] zStartPos = { -2.55f, -2.63f,8.78f, 8.299f, 2.302f, 1.71f};
    public TextMeshProUGUI TabMenuTeam1;
    public TextMeshProUGUI TabMenuTeam2;
    public GameManagerScript GM;

    public GameObject tabMenu;
    bool refToken = false;
    public bool friendlyFire = false;
    void Start() {
        // setModel(modelPrefab);
        //playerCamera = transform.Find("Camera").gameObject;
   
        

        if (!IsLocalPlayer) {
            playerCamera.GetComponent<Camera>().enabled = false;
            GetComponent<AudioListener>().enabled = false;
            return;
            
        }
        GameObject Lobby = GameObject.Find("Lobby");
        transform.position = new Vector3(Lobby.transform.position.x, Lobby.transform.position.y + 2f,Lobby.transform.position.z);
        UpdateTeamServerRpc(0);
        TeamPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None; 
        refreshCameraPosition();

        controller = GetComponent<CharacterController>(); //shift alt down
        controller.enabled = false;
        playerStatus = GetComponent<PlayerStatus>();
        
        setController(0.55f, 0.2f);


        model.transform.Find("Robot_Soldier_Head").GetComponent<Renderer>().enabled = false;
        //model.transform.Find("Robot_Soldier_Body").GetComponent<Renderer>().enabled = false;
       // model.transform.Find("Robot_Soldier_Feet").GetComponent<Renderer>().enabled = false;
        //model.transform.Find("Robot_Soldier_Legs2").GetComponent<Renderer>().enabled = false;
        HUD = GetComponent<HUD_Manager>(); 
        
        GM = GameObject.Find("GameManager").GetComponent<GameManagerScript>();

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

        //Debug.Log(playerName.Value);
        reloadTimer -= Time.deltaTime;
        shootTimer -= Time.deltaTime;
        if(prevTeam!=team.Value)
            setTexture();

        
        if(IsLocalPlayer && GM.pause.Value == true && refToken==false)
        {

                respawn();

        
        }
            
        refToken = GM.pause.Value;


        if (IsLocalPlayer && team.Value!=0 && GM.pause.Value == false) 
        {
       
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                if(tabMenu.active)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    tabMenu.SetActive(false);
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    tabMenu.SetActive(true);
                }
            }



        TabMenuTeam1.text = GM.redTeamList.Value.ToString();
        TabMenuTeam2.text = GM.blueTeamList.Value.ToString();

  

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
                movement();




                if(reloadTimer<=0f)
                    shootingTest();
                mouse();
            }
            else
                playerStatus.setFlagServerRpc(8, true);
            
            UpdateClientServerRpc(transform.position, transform.eulerAngles);

            UpdateDirectionMagnitudeServerRpc(Mathf.Clamp01(direction.magnitude));
            refreshCameraPosition();
        }
        prevTeam = team.Value;

        
        
    }




    void shootingTest() 
    {
        float range = 20f;
        float damage = 35f;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * range, Color.red);

        RaycastHit hit;

        playerStatus.setFlagServerRpc(2, false); //LPM
        playerStatus.setFlagServerRpc(6, false); //R

        if (Input.GetMouseButton(0) && shootTimer<=0f && playerStatus.PPM.Value) 
        {  


    //playerStatus.setFlagServerRpc(int flagIndex, bool f); 
    //playerStatus.setFlagServerRpc(1, bool f); //PPM
    //playerStatus.setFlagServerRpc(2, bool f); //LPM
    //playerStatus.setFlagServerRpc(3, bool f); //WASD
    //playerStatus.setFlagServerRpc(4, bool f); //SHIFT
    //playerStatus.setFlagServerRpc(5, bool f); //SPACE
    //playerStatus.setFlagServerRpc(6, bool f); //R
    //playerStatus.setFlagServerRpc(7, bool f); //CTRL
    //playerStatus.setFlagServerRpc(8, bool f); //DEAD

            if(HUD.ammunition>0)
            {
                playerStatus.setFlagServerRpc(2, true); //LPM

                shootTimer = shootDelay;
                HUD.ammunition--;
                
                //TS & AG :: Animacja wystrzału
                ifShootingServerRpc(1);
                shootL=1;
                
                
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
                ifShootingServerRpc(2);
                shootL=2;
                //KJ :: Dodać dźwięk braku amunicji
            }
        }
        else if(Input.GetKeyDown(KeyCode.R) && reloadTimer <= 0)
        {
            playerStatus.setFlagServerRpc(6, true);

            reloadTimer = reloadDelay;
            HUD.ammunition = HUD.maxAmmunition;
            HUD.AmmoImage.SetActive(false);
            HUD.Reloading_Circle.SetActive(true);
            HUD.CircleFill.fillAmount = 1.0f;
            ifShootingServerRpc(3); 
            shootL=3;//przeladowanie
            //MG :: Przeładowanie w Hudzie
        }
        else
        {
            ifShootingServerRpc(0);
            shootL=0;
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

        if (Input.GetMouseButton(0) && shootTimer==shootDelay && reloadTimer<=0f&& HUD.ammunition>0) 
        {
            HUD.setCrosshairScale(new Vector3(2f, 2, 2f));
            HUD.crosshairScalingTime = 1f/0.01f;
            sensitivity = 0.1f * mouseSensitivity;
        }
        else if(Input.GetMouseButton(1))
        {
            HUD.setCrosshairScale(new Vector3(0.75f, 0.75f, 0.75f));
            sensitivity = 0.5f * mouseSensitivity;
             HUD.crosshairScalingTime = 1f/0.25f;
        }

        else
        {
            HUD.setCrosshairScale(new Vector3(1f, 1f, 1f));
            sensitivity = mouseSensitivity;
            HUD.crosshairScalingTime = 1f/0.25f;
        }
        
      
        
        // BS :: przesunięcie kamery bliżej gdy celowanie

        float mouseX = sensitivity * Input.GetAxis("Mouse X") * Time.deltaTime;
        float mouseY = sensitivity * Input.GetAxis("Mouse Y") * Time.deltaTime;
        Vector3 rotation = new Vector3(-mouseY, 0, 0);
        cameraRotation += rotation;
        cameraRotation.x = Mathf.Clamp(cameraRotation.x, cameraMinRange, cameraMaxRange);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotation);
        transform.Rotate(Vector3.up * mouseX);
    }


    // [1:PPM]
    // [2:LPM]
    // [3:WASD]
    // [4:SHIFT]
    // [5:SPACE]
    // [6:R]
    // [7:CTRL]
    // [8:DEAD]

    //playerStatus.setFlagServerRpc(int flagIndex, bool f); 
    //playerStatus.setFlagServerRpc(1, bool f); //PPM
    //playerStatus.setFlagServerRpc(2, bool f); //LPM
    //playerStatus.setFlagServerRpc(3, bool f); //WASD
    //playerStatus.setFlagServerRpc(4, bool f); //SHIFT
    //playerStatus.setFlagServerRpc(5, bool f); //SPACE
    //playerStatus.setFlagServerRpc(6, bool f); //R
    //playerStatus.setFlagServerRpc(7, bool f); //CTRL
    //playerStatus.setFlagServerRpc(8, bool f); //DEAD

    void movement() 
    {
        direction = Vector3.zero;
        direction += transform.right * Input.GetAxis("Horizontal");
        direction += transform.forward * Input.GetAxis("Vertical");
        float speed = 0.0f;

        
                                //SHIFT
        playerStatus.setFlagServerRpc(1, (reloadTimer>0f) ? false : Input.GetMouseButton(1));           //PPM
        playerStatus.setFlagServerRpc(7, Input.GetKey(KeyCode.LeftControl));                            //CTRL


        if (controller.isGrounded) 
        {
            if (direction.x != 0.0f || direction.z != 0.0f) 
            {
                playerStatus.setFlagServerRpc(3, true);                                                 //WASD
                playerStatus.setFlagServerRpc(4, Input.GetKey(KeyCode.LeftShift));     

                if (Input.GetKey(KeyCode.LeftShift)) 
                    speed = runSpeed;
                else if (Input.GetKey(KeyCode.LeftControl)) 
                    speed = crouchSpeed;
                else 
                    speed = walkSpeed;
            } 
            else
            {
                playerStatus.setFlagServerRpc(3, false);  
                playerStatus.setFlagServerRpc(4, false);     
            }
                                                             //WASD
            
            //JUMP
            if (Input.GetKey(KeyCode.Space) && controller.isGrounded) 
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityAcceleration);
                playerStatus.setFlagServerRpc(5, true); //SPACE
            }
            else
                playerStatus.setFlagServerRpc(5, false); //SPACE
            
            velocity.x = speed * direction.x;
            velocity.z = speed * direction.z;
        } 
        else 
        {
            if (direction.x != 0.0f || direction.z != 0.0f)
                speed = walkSpeed;
            // if (velocity.y > 0)
            //     playerStatus.UpdateStatusServerRpc(PlayerStatus.State.Rises);
            // else
            //     playerStatus.UpdateStatusServerRpc(PlayerStatus.State.Falls);
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

 public void changeTeam(int team)
    {
        UpdateTeamServerRpc(team);
        Cursor.lockState = CursorLockMode.Locked;
        tabMenu.SetActive(false);
    }

    public void changeTeamA()
    {
        changeTeam(1);
    }
    public void changeTeamB()
    {
       changeTeam(2);
    }


    private void joinToGame(int team)
    {
        if(inputName.text.Length!=0)
        {
            UpdateTeamServerRpc(team);
            updateNameServerRpc(inputName.text);
            //Debug.Log(inputName.text);
            Cursor.lockState = CursorLockMode.Locked;
            respawnlocation();
            TeamPanel.SetActive(false);
        }
    }

    private void respawnlocation()
    {
        controller.enabled = false;
        int spot = Random.Range(0, 5);
        transform.position = new Vector3(xStartPos[spot] + Random.Range(-0.5f, 0.5f), yStartPos[spot], zStartPos[spot]+ Random.Range(-0.5f, 0.5f));
        controller.enabled = true;
    }



    public void teamA()
    {
        joinToGame(1);
    }

    public void teamB()
    {
        joinToGame(2);
    }

   
[ServerRpc]
    public void updateNameServerRpc(string name)
    {
        playerName.Value = name;
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

    [ServerRpc]
    public void ifShootingServerRpc(int x)
    {
        shoot.Value = x;
    }


    public void respawn()
    {  
        HUD.setHPServerRpc();
        HUD.setSHIELDServerRpc();
        respawnlocation();



    }
}

