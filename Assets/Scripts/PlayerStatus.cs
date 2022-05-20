using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerStatus : NetworkBehaviour
{
    public enum State {Idlee, Walk, Run, Jump, Falls, Rises, Aiming, Crouch, CrouchAim, CrouchMove, WalkingAim, IdleAim, Dying};
    public NetworkVariable<State> state = new NetworkVariable<State>();
    

    [SerializeField] private bool debug = false;


    void Start()
    {   
        if (!IsLocalPlayer) return;
        
        UpdateStatusServerRpc(State.Run);
    }

    // Update is called once per frame
    void Update()
    {
         if (IsLocalPlayer && debug)
            Debug.Log(state.Value);
    }

    
    [ServerRpc]
    public void UpdateStatusServerRpc(PlayerStatus.State newState) {

        state.Value = newState;

    }


}


