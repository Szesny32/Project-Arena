using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AnimationController : NetworkBehaviour
{
    public Animator animator;
    private PlayerStatus playerStatus;
    // Start is called before the first frame update
    void Start()
    {
        
        playerStatus = GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (playerStatus.state)
        {
            case PlayerStatus.State.Idlee:
                {
                    Debug.Log(playerStatus.state);
                    animator.SetFloat("Input Magnitude", 0.0f, 0.05f, Time.deltaTime);
                }
                break;

            case PlayerStatus.State.Walk:
                {
                    Debug.Log(playerStatus.state);
                    animator.SetFloat("Input Magnitude", 0.5f, 0.05f, Time.deltaTime);
                }

                break;

            case PlayerStatus.State.Run:
                {
                Debug.Log(playerStatus.state);
                animator.SetFloat("Input Magnitude", 1.0f, 0.05f, Time.deltaTime);
                }
                break;


            default:
                // Debug.Log("Custom ERROR "+playerObject.name+" state undefined");
                break;

        }
    }
}
