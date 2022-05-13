using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

<<<<<<< HEAD
public class PlayerStatus : MonoBehaviour
{
    public enum State {Idlee, Walk, Run, Jump, Falls, Rises, Aiming, Crouch, CrouchAim, CrouchMove, WalkingAim, IdleAim};
    
    public State state; 
=======
public class PlayerStatus : NetworkBehaviour {
    public enum State {Idlee, Walk, Run, Jump, Falls, Rises, Aiming, Crouch, CrouchMove};
    public NetworkVariable<State> state = new NetworkVariable<State>();
>>>>>>> 533c51980f3720c90ddfbaca863c3f224f3fe300
    [SerializeField] private bool debug = false;


    void Start()
    {   
        if (!IsLocalPlayer) {
            return;
        }
        state.Value = State.Run;
    }

    // Update is called once per frame
    void Update()
    {

        if(debug)
            Debug.Log(state);
    }

    
    [ServerRpc]
    public void UpdateStatusServerRpc(PlayerStatus.State newState) {

        state.Value = newState;

    }


}


