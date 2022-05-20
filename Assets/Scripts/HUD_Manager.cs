using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //x
using Unity.Netcode;


public class HUD_Manager : NetworkBehaviour
{
    public Slider slider;
    private float MAXHP = 100.0f;
    public NetworkVariable<float> HP = new NetworkVariable<float>();


    private Image dmgTakenEffect;
    private float effectTime = 0.25f;
    private Color effectColor = new Color(1f,0f,0f,0.5f);
    public NetworkVariable<bool> playerReceivedDmg = new NetworkVariable<bool>();

    void Start()
    {
        if (!IsLocalPlayer) {
            return;
       }
        setHPServerRpc(MAXHP);
        slider = GameObject.Find("PlayerHUDCanvas/HealthBar").GetComponent<Slider>();
        dmgTakenEffect =  GameObject.Find("PlayerHUDCanvas/Image").GetComponent<Image>();   
        slider.value = MAXHP; //= HP.Value - not working!
        setPlayerReceivedDmgServerRpc(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            if(playerReceivedDmg.Value)
            {
                dmgTakenEffect.color = effectColor;
                setPlayerReceivedDmgServerRpc(false);
                slider.value = HP.Value;
            }
            else
            {
                float A= (HP.Value>50f) ? 0f :  0.5f * (1f - (HP.Value / MAXHP) ) - 0.25f;
                dmgTakenEffect.color = Color.Lerp(dmgTakenEffect.color, new Color(1f ,0f,0f, A), (1f/effectTime)*Time.deltaTime);
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
        if(DMG > HP.Value)
            HP.Value = 0f;
        else
            HP.Value -= DMG;
        playerReceivedDmg.Value = true;
    }

    [ServerRpc]
    public void setHPServerRpc(float newHP)
    {  
        HP.Value = newHP;
    }

    [ServerRpc]
    public void setPlayerReceivedDmgServerRpc(bool receivedDmg)
    {  
        playerReceivedDmg.Value = receivedDmg;
    }


}

