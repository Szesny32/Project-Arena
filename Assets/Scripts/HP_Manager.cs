using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class HP_Manager : NetworkBehaviour
{
    public Slider slider;
    private float MAXHP = 100.0f;
    [SerializeField] private NetworkVariable<float> HP = new NetworkVariable<float>();

    void Start()
    {
        HP.Value = MAXHP;
        slider.value = HP.Value;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public float getHP()
    {
        return HP.Value;
    }

    [ServerRpc]
    public void takeDamageServerRpc(float DMG)
    {
        HP.Value-=DMG;
        Debug.Log(this.name+" has taken: "+DMG+" HP! Now has HP:"+HP.Value);
    }
}
