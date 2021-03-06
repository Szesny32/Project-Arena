using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; //x
using Unity.Netcode;


public class HUD_Manager : NetworkBehaviour
{

    private Image dmgTakenEffect;
    private float effectTime = 0.25f;
    public NetworkVariable<bool> playerReceivedDmg = new NetworkVariable<bool>();


    public Slider sliderHP;
    public NetworkVariable<float> HP = new NetworkVariable<float>();
    public NetworkVariable<bool> isAlive = new NetworkVariable<bool>();
    public float MAXHP = 100.0f;
    private Color effectColor_HP = new Color(1f,0f,0f,0.5f);
    public TextMeshProUGUI ammoText;

    public float ammunition = 0 ;
    public float maxAmmunition = 35;
   
    
    
    public Slider sliderSHIELD;
    public NetworkVariable<float> SHIELD = new NetworkVariable<float>();
    private Color effectColor_SHIELD = new Color(0.0859375f, 0.82421875f, 0.94140625f, 0.5f);
    private float SHIELD_Regeneration = 20f; // (20/s)
    public float MAXSHIELD = 100.0f;
    private float timer = 0.0f;
    private float rechargeDelay = 2.0f;
    private float rechargeTimer = 0.0f;


    private GameObject crosshair;
    private Vector3 crosshairScale = new Vector3(1f,1f,1f);
    public float crosshairScalingTime = 1f / 0.25f;
    public GameObject AmmoImage;
    public GameObject Reloading_Circle;
    public Image CircleFill;

    void Start()
    {
        if (!IsLocalPlayer) {
            return;
       }
        dmgTakenEffect =  GameObject.Find("PlayerHUDCanvas/Image").GetComponent<Image>();   
        AmmoImage =  GameObject.Find("PlayerHUDCanvas/AmmoPanel/AmmoImage");
        Reloading_Circle = GameObject.Find("PlayerHUDCanvas/AmmoPanel/Reloading_Circle");
        CircleFill = GameObject.Find("PlayerHUDCanvas/AmmoPanel/Reloading_Circle/Circle/Circle_fill").GetComponent<Image>();
        crosshair = GameObject.Find("PlayerHUDCanvas/CrosshairSprite");
        ammunition = maxAmmunition;
        ammoText = GameObject.Find("PlayerHUDCanvas/AmmoPanel/AmmoCounter").GetComponent<TextMeshProUGUI>();   
        ammoText.text= $"{ammunition}/{maxAmmunition}";
        setHPServerRpc();
        sliderHP = GameObject.Find("PlayerHUDCanvas/HealthBar").GetComponent<Slider>();
        sliderHP.value = MAXHP; //= HP.Value - not working!

        setSHIELDServerRpc();
        sliderSHIELD = GameObject.Find("PlayerHUDCanvas/ShieldBar").GetComponent<Slider>();
        sliderSHIELD.value = MAXSHIELD;
        
        setPlayerReceivedDmgServerRpc(false);
    }

    // Update is called once per frame
    void Update()
    {
           timer+=Time.deltaTime;
        if(timer < 0.5f)
            return;
        if (IsLocalPlayer)
        {        
            sliderHP.value = HP.Value;
            if(playerReceivedDmg.Value )
            {
                if(SHIELD.Value>0)
                    dmgTakenEffect.color = effectColor_SHIELD;
                else
                    dmgTakenEffect.color = effectColor_HP;
                setPlayerReceivedDmgServerRpc(false);
                sliderHP.value = HP.Value;
                sliderSHIELD.value = SHIELD.Value;
                rechargeTimer  = rechargeDelay;
            }
            else if( HP.Value>0f )
            {
                rechargeTimer-=Time.deltaTime;
                float A= (HP.Value>50f) ? 0f :  0.5f * (1f - (HP.Value / MAXHP) ) - 0.25f;
                dmgTakenEffect.color = Color.Lerp(dmgTakenEffect.color, new Color(1f ,0f,0f, A), (1f/effectTime)*Time.deltaTime);
                
                if(rechargeTimer<=0)
                    ShieldRegenServerRpc();
                 sliderSHIELD.value = SHIELD.Value;
            }
            ammoText.text= $"{ammunition}/{maxAmmunition}";
            updateCrosshair();
        }
    }


    public float getHP()
    {
        return HP.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void takeDamageServerRpc(float DMG)
    {  
        if(HP.Value==0f)
            return;
        if(SHIELD.Value > DMG)
        {
            SHIELD.Value-=DMG;
        }
        else
        {
            float x = DMG - SHIELD.Value;
            SHIELD.Value = 0.0f;

            if(x >= HP.Value)
                {
                    HP.Value = 0f;
                    isAlive.Value = false;
                }
            else
                HP.Value -= DMG;
        }

        playerReceivedDmg.Value = true;
    }

    [ServerRpc]
    public void setHPServerRpc()
    {  
        HP.Value = MAXHP;
        isAlive.Value = true;

    }

    [ServerRpc]
    public void setSHIELDServerRpc()
    {  
        SHIELD.Value = MAXSHIELD;
    }

    [ServerRpc]
    public void setPlayerReceivedDmgServerRpc(bool receivedDmg)
    {  
        playerReceivedDmg.Value = receivedDmg;
    }

    [ServerRpc]
    private void ShieldRegenServerRpc()
    {  
        SHIELD.Value = Mathf.Clamp(SHIELD.Value+ SHIELD_Regeneration * Time.deltaTime, 0f, MAXSHIELD);
       
    }

    public void setCrosshairScale(Vector3 scale)
    {
        crosshairScale = scale;
    }
    private void updateCrosshair()
    {
        
        crosshair.transform.localScale = Vector3.Lerp(crosshair.transform.localScale , crosshairScale, crosshairScalingTime * Time.deltaTime);
    }

}

