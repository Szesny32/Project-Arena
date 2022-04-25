using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    

    [SerializeField] private GameObject playerObject;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private RectTransform crosshair;
    [SerializeField] private PlayerStatus status;

    public float normalSize = 100.0f;
    private float currentSize = 100.0f;

    public float xAim = 0.5f;
    public float xWalk = 1.5f;
    public float xRun = 2.0f;
    public float xJump = 2.5f;

    private float aimSize;
    private float walkSize;
    private float runSize;
    private float jumpSize;

    public float lineWidth = 2.0f;
    public float lineHeight = 5.0f;
    public float dotSize = 2.0f;
    public float speed = 10.0f;
    public Color crosshairColor = Color.red;

    [SerializeField] private RectTransform topLine_transform;
    [SerializeField] private RectTransform bottomLine_transform;
    [SerializeField] private RectTransform rightLine_transform;
    [SerializeField] private RectTransform leftLine_transform;
    [SerializeField] private RectTransform centerDot_transform;

    [SerializeField] private Image topLine_image;
    [SerializeField] private Image bottomLine_image;
    [SerializeField] private Image rightLine_image;
    [SerializeField] private Image leftLine_image;
    [SerializeField] private Image centerDot_image;



    void Start()
    {
        aimSize =  normalSize*xAim;
        walkSize=  normalSize*xWalk;
        runSize=  normalSize*xRun;
        jumpSize=  normalSize*xJump;
        crosshairConfig();

    }
    void Update()
    {
        if(status)
            updateCrosshair();
    }



    void updateCrosshair()
    {


        crosshairConfig();

        switch (status.state)
        {
            case PlayerStatus.State.Idlee:
            currentSize = Mathf.Lerp(currentSize, normalSize, Time.deltaTime*speed);
            break;

            case PlayerStatus.State.Walk:
            currentSize = Mathf.Lerp(currentSize, walkSize, Time.deltaTime*speed);
            break;

            case PlayerStatus.State.Run:
            currentSize = Mathf.Lerp(currentSize, runSize, Time.deltaTime*speed);
            break;

            case PlayerStatus.State.Jump:
            currentSize = Mathf.Lerp(currentSize, jumpSize, Time.deltaTime*speed);
            break;

            case PlayerStatus.State.Falls:
            currentSize = Mathf.Lerp(currentSize, jumpSize, Time.deltaTime*speed);
            break; 

            case PlayerStatus.State.Rises:
            currentSize = Mathf.Lerp(currentSize, jumpSize, Time.deltaTime*speed);
            break; 

            case PlayerStatus.State.Aiming:
            currentSize = Mathf.Lerp(currentSize, aimSize, Time.deltaTime*speed);
            break; 

            default:
            Debug.Log("ERROR "+playerObject.name+" state undefined");
            break;

        }
        crosshair.sizeDelta = new Vector2(currentSize,currentSize);
    
    }


    void crosshairConfig()
    {
        aimSize =  normalSize*xAim;
        walkSize=  normalSize*xWalk;
        runSize=  normalSize*xRun;
        jumpSize=  normalSize*xJump;

        topLine_image.color = crosshairColor;
        topLine_transform.sizeDelta = new Vector2(lineWidth, lineHeight);
            
        bottomLine_image.color = crosshairColor;
        bottomLine_transform.sizeDelta = new Vector2(lineWidth, lineHeight);
            
        rightLine_image.color = crosshairColor;
        rightLine_transform.sizeDelta = new Vector2(lineWidth, lineHeight);
            
        leftLine_image.color = crosshairColor;
        leftLine_transform.sizeDelta = new Vector2(lineWidth, lineHeight);
            
        centerDot_image.color = crosshairColor;
        centerDot_transform.sizeDelta = new Vector2(dotSize, dotSize);
    }
}
