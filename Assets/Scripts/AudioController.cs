using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioController : NetworkBehaviour
{
    private PlayerStatus playerStatus;
    private PlayerManager playerManager;
    private AudioSource audioSource;
    private AudioClip footstep;
    private AudioClip jump;
    private AudioClip run;

    // Start is called before the first frame update
    void Start()
    { 
        
        playerManager = GetComponent<PlayerManager>();
        playerStatus = GetComponent<PlayerStatus>();
        audioSource = GetComponent<AudioSource>();
        footstep = Resources.Load<AudioClip>("Classic Footstep SFX/Ground/Ground_Step0");
        run = Resources.Load<AudioClip>("Classic Footstep SFX/Ground/Ground_running_loop0");
        jump = Resources.Load<AudioClip>("Classic Footstep SFX/Forest ground/Forest_ground_jump0");
    }


    // Update is called once per frame
    void Update()
    {
        PlayerStatus.State state = playerStatus.state.Value;

       

        switch (state)
        {
            case PlayerStatus.State.Idlee:
                {
                   
                    
                }
                break;

            case PlayerStatus.State.Walk:
                {
                    if (!audioSource.isPlaying)
                        {
                            audioSource.clip = footstep;
                            audioSource.PlayOneShot(footstep);
                        }
                }

                break;

            case PlayerStatus.State.Run:
                {
                    if (!audioSource.isPlaying)
                        {
                            audioSource.clip = run;
                            audioSource.PlayOneShot(run);
                        }
                }
                break;

            case PlayerStatus.State.Jump:
                {
                    if (!audioSource.isPlaying)
                        {
                            audioSource.clip = jump;
                            audioSource.PlayOneShot(jump);
                        }      
                }
                break;

            case PlayerStatus.State.IdleAim:
                {
                    
                }
                break;

            case PlayerStatus.State.WalkingAim:
                {
                    
                }
                break;

            case PlayerStatus.State.Crouch:
                {
                    
                }
                break;
            case PlayerStatus.State.CrouchAim:
                {
                          
                }
                break;
                

            default:
                // Debug.Log("Custom ERROR "+playerObject.name+" state undefined");
                break;

        }
        
    }


}
