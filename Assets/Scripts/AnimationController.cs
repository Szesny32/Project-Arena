using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AnimationController : NetworkBehaviour
{
    private Animator animator;
    private PlayerStatus playerStatus;
    private PlayerManager playerManager;
    
    private float InputMagnitude;
    private bool IsMoving;
    private bool isIdleAiming;
    private bool isWalkingAiming;
    private bool isCrouching;
    private bool isCrouchingAiming;




    // Start is called before the first frame update
    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        playerStatus = GetComponent<PlayerStatus>();
        animator = transform.Find("Model").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerStatus.State state = playerStatus.state.Value;

        IsMoving = false; 
        isIdleAiming = false;
        isWalkingAiming = false;
        isCrouching = false;
        isCrouchingAiming = false;

        switch (state)
        {
            case PlayerStatus.State.Idlee:
                {
                    InputMagnitude = playerManager.getDirectionMagnitude();
                }
                break;

            case PlayerStatus.State.Walk:
                {
                    InputMagnitude = playerManager.getDirectionMagnitude()/2f;
                }

                break;

            case PlayerStatus.State.Run:
                {
                    InputMagnitude = playerManager.getDirectionMagnitude();
                }
                break;

            case PlayerStatus.State.IdleAim:
                {
                    InputMagnitude = playerManager.getDirectionMagnitude();
                    isIdleAiming = true;
                }
                break;

            case PlayerStatus.State.WalkingAim:
                {
                    InputMagnitude = playerManager.getDirectionMagnitude()/2f;
                    isWalkingAiming = true;
                }
                break;

            case PlayerStatus.State.Crouch:
                {
                    InputMagnitude = playerManager.getDirectionMagnitude();
                    isCrouching = true;
                }
                break;
            case PlayerStatus.State.CrouchAim:
                {
                    InputMagnitude = playerManager.getDirectionMagnitude()/2f;
                    isCrouchingAiming = true;      
                }
                break;

            default:
                // Debug.Log("Custom ERROR "+playerObject.name+" state undefined");
                break;

        }
        Debug.Log(playerStatus.state.Value);
        animator.SetFloat("Input Magnitude", InputMagnitude, 0.05f, Time.deltaTime);
        animator.SetBool("isIdleAiming", isIdleAiming);
        animator.SetBool("isWalkingAiming", isWalkingAiming);
        animator.SetBool("isCrouching", isCrouching);
        animator.SetBool("isCrouchingAiming", isCrouchingAiming);
    }


}
