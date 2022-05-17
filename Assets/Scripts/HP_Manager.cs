using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class HP_Manager : NetworkBehaviour
{
    public Slider slider;
    private float MAXHP = 100.0f;
    public NetworkVariable<float> HP = new NetworkVariable<float>();


    private Image dmgTakenEffect;
    private float effectTime = 2.0f;
    private Color effectColor = new Color(1f,0f,0f,0.1f);
  public NetworkVariable<bool> playerReceivedDmg = new NetworkVariable<bool>();

    void Start()
    {
        if (!IsLocalPlayer) {
            return;
       }
        HP.Value = MAXHP;
        slider = GameObject.Find("Canvas/HealthBar").GetComponent<Slider>();
        Debug.Log(slider);
        dmgTakenEffect =  GameObject.Find("Canvas/Image").GetComponent<Image>();
        Debug.Log(dmgTakenEffect);
        slider.value = HP.Value;
        playerReceivedDmg.Value = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer)
        {
            if(playerReceivedDmg.Value )
            {
                dmgTakenEffect.color = effectColor;
                playerReceivedDmg.Value  = false;
                slider.value = HP.Value;
            }
            else
            {
                Debug.Log(dmgTakenEffect);
                dmgTakenEffect.color = Color.Lerp(dmgTakenEffect.color, new Color(0f,0f,0f,0.0f),effectTime*Time.deltaTime);
            }
        }
    }

    public float getHP()
    {
        return HP.Value;
    }




}
