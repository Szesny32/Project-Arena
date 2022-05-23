using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //x
using Unity.Netcode;


public class HUD_Manager : NetworkBehaviour
{

    private Image dmgTakenEffect;
    private float effectTime = 0.25f;
    public NetworkVariable<bool> playerReceivedDmg = new NetworkVariable<bool>();


    public Slider sliderHP;
    public NetworkVariable<float> HP = new NetworkVariable<float>();
    private float MAXHP = 100.0f;
    private Color effectColor_HP = new Color(1f,0f,0f,0.5f);

   
    
    public Slider sliderSHIELD;
    public NetworkVariable<float> SHIELD = new NetworkVariable<float>();
    private Color effectColor_SHIELD = new Color(0.0859375f, 0.82421875f, 0.94140625f, 0.5f);
    private float SHIELD_Regeneration = 20f; // (20/s)
    private float MAXSHIELD = 100.0f;
 private float timer = 0.0f;
    

    void Start()
    {
        if (!IsLocalPlayer) {
            return;
       }
        dmgTakenEffect =  GameObject.Find("PlayerHUDCanvas/Image").GetComponent<Image>();   

        setHPServerRpc(MAXHP);
        sliderHP = GameObject.Find("PlayerHUDCanvas/HealthBar").GetComponent<Slider>();
        sliderHP.value = MAXHP; //= HP.Value - not working!

        setSHIELDServerRpc(MAXSHIELD);
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
            if(playerReceivedDmg.Value )
            {
                if(SHIELD.Value>0)
                    dmgTakenEffect.color = effectColor_SHIELD;
                else
                    dmgTakenEffect.color = effectColor_HP;
                setPlayerReceivedDmgServerRpc(false);
                sliderHP.value = HP.Value;
                sliderSHIELD.value = SHIELD.Value;
            }
            else if(HP.Value>0f)
            {
                float A= (HP.Value>50f) ? 0f :  0.5f * (1f - (HP.Value / MAXHP) ) - 0.25f;
                dmgTakenEffect.color = Color.Lerp(dmgTakenEffect.color, new Color(1f ,0f,0f, A), (1f/effectTime)*Time.deltaTime);
                
                ShieldRegenServerRpc();
                 sliderSHIELD.value = SHIELD.Value;
            }




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

            if(x > HP.Value)
                HP.Value = 0f;
            else
                HP.Value -= DMG;
        }

        playerReceivedDmg.Value = true;
    }

    [ServerRpc]
    public void setHPServerRpc(float newHP)
    {  
        HP.Value = newHP;
    }

    [ServerRpc]
    public void setSHIELDServerRpc(float newSHIELD)
    {  
        SHIELD.Value = newSHIELD;
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


}

