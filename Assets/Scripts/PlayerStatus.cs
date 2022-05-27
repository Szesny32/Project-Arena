using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerStatus : NetworkBehaviour
{
    public enum State 
    {
        Idle, IdleAim, IdleShoot, IdleReload,
        Walk, WalkAim, WalkShoot, WalkReload,
        Run, RunAim, RunShoot, RunReload,
        Crouch, CrouchAim, CrouchShoot, CrouchReload,
        Crouching,  CrouchingAim, CrouchingShoot, CrouchingReload,
        Jump, Falls, Rises, 
        Dying
    };
    public NetworkVariable<State> state = new NetworkVariable<State>();
    

    [SerializeField] private bool debug = false;


    void Start()
    {   
        if (!IsLocalPlayer) return;
        
        UpdateStatusServerRpc(State.Idle);
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


