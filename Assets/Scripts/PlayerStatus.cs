using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public enum State {Idlee, Walk, Run, Jump, Falls, Rises};
    public State state; 
    public bool debug = false;
    private MovementController movementController;
    void Start()
    {
        movementController = GetComponent<MovementController>();
        state = State.Idlee;
    }
    // Update is called once per frame
    void Update()
    {
        if(movementController.controller.isGrounded)
        {
            if(Input.GetKey(KeyCode.Space))
                state = State.Jump;
            else if(movementController.getDirection().x != 0.0f || movementController.getDirection().z != 0.0f)    
                state = Input.GetKey(KeyCode.LeftShift)? State.Run : State.Walk; 
            else
               state = State.Idlee;
        }
        else
        {
            if(movementController.getVelocity().y > 0)
                state = State.Rises; 
            else
                state = State.Falls; 
        }
        if(debug)
            Debug.Log(state);
    }
}


