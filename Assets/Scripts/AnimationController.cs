using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AnimationController : NetworkBehaviour
{
    private Animator animator;
    private PlayerStatus playerStatus;
    private PlayerManager playerManager;
    
    private bool PPM;
    private bool LPM;
    private bool WASD;
    private bool SHIFT;
    private bool SPACE;
    private bool CTRL;
    private bool R;
    private bool DEAD;



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
        animator.SetBool("PPM", playerStatus.PPM.Value);
        animator.SetBool("LPM", playerStatus.LPM.Value);
        animator.SetBool("WASD", playerStatus.WASD.Value);
        animator.SetBool("SHIFT", playerStatus.SHIFT.Value);
        animator.SetBool("SPACE", playerStatus.SPACE.Value);
        animator.SetBool("CTRL", playerStatus.CTRL.Value);
        animator.SetBool("R", playerStatus.R.Value);
    }
}