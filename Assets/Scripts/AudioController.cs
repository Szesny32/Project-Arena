using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioController : NetworkBehaviour
{
    private PlayerStatus playerStatus;
    private PlayerManager playerManager;
    private AudioSource audioSource;
    private AudioClip footstep0;
    private AudioClip footstep1;
    private AudioClip footstep2;
    private AudioClip footstep3;
    private AudioClip footstep4;
    private AudioClip jump;
    private AudioClip run;
    [SerializeField] public NetworkVariable<int> whichAudio = new NetworkVariable<int>();

    // Start is called before the first frame update
    void Start()
    { 
        
        playerManager = GetComponent<PlayerManager>();
        playerStatus = GetComponent<PlayerStatus>();
        audioSource = GetComponent<AudioSource>();
        footstep0 = Resources.Load<AudioClip>("Classic Footstep SFX/Ground/Ground_Step0");
        footstep2 = Resources.Load<AudioClip>("Classic Footstep SFX/Ground/Ground_Step2");
        run = Resources.Load<AudioClip>("Classic Footstep SFX/Ground/Ground_running_loop0");
        jump = Resources.Load<AudioClip>("Classic Footstep SFX/Forest ground/Forest_ground_jump0");
    }


    // Update is called once per frame
    void Update()
    {
        PlayerStatus.State state = playerStatus.state.Value;  
        if(IsLocalPlayer)
        {
            if(state==PlayerStatus.State.Walk || state==PlayerStatus.State.WalkingAim)
            {
                setAudioServerRpc(1);
            }
            else if(state==PlayerStatus.State.Run)
            {
                setAudioServerRpc(2);
            }
            else if(state==PlayerStatus.State.Jump)
            {
                setAudioServerRpc(3);
            }
            else if (state==PlayerStatus.State.Falls || state==PlayerStatus.State.Rises)
            {
                
            }
            else
            {
                setAudioServerRpc(0);
            }
                    
        }
        if(whichAudio.Value==1)
        {
            
            if(!audioSource.isPlaying)
            {
                int x = Random.Range(0,1);
                if(x==0)
                    audioSource.PlayOneShot(footstep0, 0.05f);
                else if(x==1)
                    audioSource.PlayOneShot(footstep2, 0.05f);                              
            }
                
        }
        else if(whichAudio.Value==2)
        {
            if(!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(run, 0.5f);
            }
        }
        else if(whichAudio.Value==3)
        {
            if(!audioSource.isPlaying)
                audioSource.PlayOneShot(jump);
        }
        

        
    }

    [ServerRpc]
    private void setAudioServerRpc(int x)
    {
        whichAudio.Value = x;
    }


}
