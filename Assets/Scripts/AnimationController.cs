using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AnimationController : NetworkBehaviour
{
    public Animator animator;
    private PlayerStatus playerStatus;
    private PlayerManager playerManager;
    // Start is called before the first frame update
    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        playerStatus = GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerStatus.State state = playerStatus.state.Value;
        switch (state)
        {
            case PlayerStatus.State.Idlee:
                {
                    Debug.Log($"{playerStatus.state.Value} {playerManager.getDirectionMagnitude()}");
                    animator.SetFloat("Input Magnitude", playerManager.getDirectionMagnitude(), 0.05f, Time.deltaTime);
                }
                break;

            case PlayerStatus.State.Walk:
                {
                    Debug.Log($"{playerStatus.state.Value} {playerManager.getDirectionMagnitude()}");
                    animator.SetFloat("Input Magnitude", playerManager.getDirectionMagnitude() / 2f, 0.05f, Time.deltaTime);
                }

                break;

            case PlayerStatus.State.Run:
                {
                    Debug.Log($"{playerStatus.state.Value} {playerManager.getDirectionMagnitude()}");
                    animator.SetFloat("Input Magnitude", playerManager.getDirectionMagnitude(), 0.05f, Time.deltaTime);
                }
                break;

            case PlayerStatus.State.IdleAim:
                {
                    Debug.Log($"{playerStatus.state.Value} {playerManager.getDirectionMagnitude()}");
                    animator.SetBool("isIdleAiming", true); //potem gdzies w kodzie musi sie zmieniac na false po puszczeniu ppm
                    animator.SetFloat("Input Magnitude", playerManager.getDirectionMagnitude(), 0.05f, Time.deltaTime);
                }
                break;

            case PlayerStatus.State.WalkingAim:
                {
                    Debug.Log(playerStatus.state.Value);
                    animator.SetBool("isWalkingAiming", true);
                    animator.SetFloat("Input Magnitude", playerManager.getDirectionMagnitude() / 2f, 0.05f, Time.deltaTime);
                }
                break;

            case PlayerStatus.State.Crouch:
                {
                    Debug.Log(playerStatus.state.Value);
                    animator.SetBool("isCrouching", true);
                    animator.SetFloat("Input Magnitude", playerManager.getDirectionMagnitude(), 0.05f, Time.deltaTime);
                }
                break;
            case PlayerStatus.State.CrouchAim:
                {
                    Debug.Log(playerStatus.state.Value);
                    animator.SetBool("isCrouchingAiming", true);
                    animator.SetFloat("Input Magnitude", playerManager.getDirectionMagnitude() / 2f, 0.05f, Time.deltaTime);
                }
                break;

            default:
                // Debug.Log("Custom ERROR "+playerObject.name+" state undefined");
                break;

        }
    }


}
