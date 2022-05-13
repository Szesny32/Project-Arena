using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public enum State {Idlee, Walk, Run, Jump, Falls, Rises, Aiming, Crouch, CrouchAim, CrouchMove, WalkingAim, IdleAim};
    
    public State state; 
    [SerializeField] private bool debug = false;


    void Start()
    {
        state = State.Idlee;
    }

    // Update is called once per frame
    void Update()
    {

        if(debug)
            Debug.Log(state);
    }

    
}


