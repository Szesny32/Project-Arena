using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public enum State {Idlee, Walk, Run, Jump, Falls, Rises};
    
    [HideInInspector] public State state; 
    
    [SerializeField] private bool debug = false;
   
    private MovementController playerController;

    void Start()
    {
        playerController = GetComponent<MovementController>();
        state = State.Idlee;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.getIsGrounded())
        {
            if(Input.GetKey(KeyCode.Space))
                state = State.Jump;
            else if(playerController.getDirection().x != 0.0f || playerController.getDirection().z != 0.0f)    
                state = Input.GetKey(KeyCode.LeftShift)? State.Run : State.Walk; 
            else
               state = State.Idlee;
        }
        else
        {
            if(playerController.getVelocity().y > 0)
                state = State.Rises; 
            else
                state = State.Falls; 
        }
        if(debug)
            Debug.Log(state);
    }
}


