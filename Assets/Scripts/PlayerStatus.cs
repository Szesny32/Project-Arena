using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStatus : NetworkBehaviour {
    public enum State {Idlee, Walk, Run, Jump, Falls, Rises, Aiming, Crouch, CrouchMove};
    public NetworkVariable<State> state = new NetworkVariable<State>();
    [SerializeField] private bool debug = false;


    void Start()
    {
        state.Value = State.Run;
    }

    // Update is called once per frame
    void Update()
    {

        if(debug)
            Debug.Log(state);
    }


}


