using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AudioController : NetworkBehaviour
{
    private PlayerStatus playerStatus;
    private PlayerManager playerManager;
    private AudioSource audioSource;
    public AudioSource audioSource2;
    private AudioClip footstep0;
    private AudioClip footstep1;
    private AudioClip footstep2;
    private AudioClip footstep3;
    private AudioClip footstep4;
    private AudioClip jump;
    private AudioClip run;
    private AudioClip shoot0;
    private AudioClip shoot1;
    private AudioClip shoot2;
    private AudioClip shoot3;
    private AudioClip noAmmo;
    private AudioClip reload;
    [SerializeField] public NetworkVariable<int> whichAudio = new NetworkVariable<int>();

    // Start is called before the first frame update
    void Start()
    { 
        
        playerManager = GetComponent<PlayerManager>();
        playerStatus = GetComponent<PlayerStatus>();
        audioSource = GetComponent<AudioSource>();
        audioSource2 = GetComponent<AudioSource>();
        footstep0 = Resources.Load<AudioClip>("Classic Footstep SFX/Ground/Ground_Step0");
        footstep2 = Resources.Load<AudioClip>("Classic Footstep SFX/Ground/Ground_Step2");
        run = Resources.Load<AudioClip>("Classic Footstep SFX/Ground/Ground_running_loop0");
        jump = Resources.Load<AudioClip>("Classic Footstep SFX/Forest ground/Forest_ground_jump0");
        shoot0 = Resources.Load<AudioClip>("Scifi Guns SFX Pack/Gun2_1");
        shoot1 = Resources.Load<AudioClip>("Scifi Guns SFX Pack/Gun2_2");
        shoot2 = Resources.Load<AudioClip>("Scifi Guns SFX Pack/Gun2_3");
        shoot3 = Resources.Load<AudioClip>("Scifi Guns SFX Pack/Gun2_4");
        noAmmo = Resources.Load<AudioClip>("noAmmo");
        reload = Resources.Load<AudioClip>("reload");
    }


    // Update is called once per frame
    void Update()
    {  
        if(IsLocalPlayer)
        {
            if(playerStatus.WASD.Value && !playerStatus.SHIFT.Value)
                setAudioServerRpc(1);
            
            else if(playerStatus.WASD.Value && playerStatus.SHIFT.Value)
                setAudioServerRpc(2);
            
            else if(playerStatus.SPACE.Value)
                setAudioServerRpc(3);
            
            else
                setAudioServerRpc(0);
                    
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
                audioSource.PlayOneShot(run, 0.25f);
            }
        }
        else if(whichAudio.Value==3)
        {
            if(!audioSource.isPlaying)
                audioSource.PlayOneShot(jump);
        }
        if(!playerManager.GM.pause.Value)
        {
                      
            int tmp;
            if(IsLocalPlayer)
            {
                tmp =playerManager.shootL;
            }
            else
            {
                tmp = playerManager.shoot.Value;
            }
            if(tmp==1)
            {
                int x = Random.Range(0,3);
                if(x==0)
                    audioSource2.PlayOneShot(shoot0, 0.25f);
                else if(x==1)
                    audioSource2.PlayOneShot(shoot1, 0.25f); 
                else if(x==2)
                    audioSource2.PlayOneShot(shoot2, 0.25f);
                else if(x==3)
                    audioSource2.PlayOneShot(shoot3, 0.25f);

            }
            else if(tmp==2)
            {
                if(!audioSource2.isPlaying)
                    audioSource2.PlayOneShot(noAmmo, 0.5f);
            }
            else if(tmp==3)
            {
                if(!audioSource2.isPlaying)
                    audioSource2.PlayOneShot(reload, 0.3f);
            }

        }
        

        
    }

    [ServerRpc]
    private void setAudioServerRpc(int x)
    {
        whichAudio.Value = x;
    }


}
