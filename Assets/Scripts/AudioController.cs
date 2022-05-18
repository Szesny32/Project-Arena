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

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        playerStatus = GetComponent<PlayerStatus>();
        audioSource = GetComponent<AudioSource>();
        footstep = Resources.Load<AudioClip>("Classic Footstep SFX/Floor/Floor_step10");
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
