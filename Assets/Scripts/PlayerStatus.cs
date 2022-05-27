using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerStatus : NetworkBehaviour
{

    public NetworkVariable<bool> PPM = new NetworkVariable<bool>();
    public NetworkVariable<bool> LPM = new NetworkVariable<bool>();
    public NetworkVariable<bool> WASD = new NetworkVariable<bool>();
    public NetworkVariable<bool> SHIFT = new NetworkVariable<bool>();
    public NetworkVariable<bool> SPACE = new NetworkVariable<bool>();
    public NetworkVariable<bool> CTRL = new NetworkVariable<bool>();
    public NetworkVariable<bool> R = new NetworkVariable<bool>();
    public NetworkVariable<bool> DEAD = new NetworkVariable<bool>();
        

    void Start()
    {   
        if (!IsLocalPlayer) return;
        
    }

    // Update is called once per frame
    void Update()
    {

    }


   
    // [1:PPM]
    // [2:LPM]
    // [3:WASD]
    // [4:SHIFT]
    // [5:SPACE]
    // [6:R]
    // [7:CTRL]
    // [8:DEAD]

    [ServerRpc]
    public void setFlagServerRpc(int flagIndex, bool f) 
    {
        if(flagIndex==1)
            PPM.Value = f;
        else if(flagIndex==2)
            LPM.Value = f;
        else if(flagIndex==3)
            WASD.Value = f;
        else if(flagIndex==4)
            SHIFT.Value = f;
        else if(flagIndex==5)
            SPACE.Value = f;
        else if(flagIndex==6)
            R.Value = f;
        else if(flagIndex==7)
            CTRL.Value = f;
        else if(flagIndex==8)
            DEAD.Value = f;
    }

}


