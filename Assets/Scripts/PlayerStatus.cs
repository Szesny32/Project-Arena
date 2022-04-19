using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public enum State {Idlee, Walk, Run, Jump, Falls, Rises};
    public State state; 
    public bool debug = false;
    void Start()
    {
        state = State.Idlee;
    }

    // Update is called once per frame
    void Update()
    {
        if(debug)
        {
            Debug.Log(state);
        }
    }
}
