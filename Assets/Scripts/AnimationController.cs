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
        PlayerStatus.State state = playerStatus.state.Value;    

        // public enum State 
        // {
                // Idle, IdleAim, IdleShoot, 
                // Walk, WalkAim, WalkShoot,
                // Run, RunAim, RunShoot,
                // Crouch, CrouchAim, CrouchShoot,
                // Crouching,  CrouchingAim, CrouchingShoot,
                // Jump, Falls, Rises, 
                // Dying
        // };
        //setParameters(bool PPM, bool LPM, bool WASD, bool SHIFT, bool SPACE, bool R, bool CTRL)

        if(state == PlayerStatus.State.Dying)
            animator.SetBool("DEAD", true);

        if(state == PlayerStatus.State.Idle)
            setParameters(false, false, false, false, false, false, false);
        else if(state == PlayerStatus.State.Walk)
            setParameters(false, false, true, false, false, false, false);
        else if(state == PlayerStatus.State.Run)
            setParameters(false, false, true, true, false, false, false);
        else if(state == PlayerStatus.State.Crouch)
            setParameters(false, false, false, false, false, false, true);
        else if(state == PlayerStatus.State.Crouching)
            setParameters(false, false, true, false, false, false, true);
        
        else if(state == PlayerStatus.State.IdleShoot)
            setParameters(PPM, true, false, false, false, false, false);
        else if(state == PlayerStatus.State.WalkShoot)
            setParameters(PPM, true, true, false, false, false, false);
        else if(state == PlayerStatus.State.RunShoot)
            setParameters(PPM, true, true, true, false, false, false);
        else if(state == PlayerStatus.State.CrouchShoot)
            setParameters(PPM, true, false, false, false, false, true);
        else if(state == PlayerStatus.State.CrouchingShoot)
            setParameters(PPM, true, true, false, false, false, true);

        else if(state == PlayerStatus.State.IdleAim)
            setParameters(true, false, false, false, false, false, false);
        else if(state == PlayerStatus.State.WalkAim)
            setParameters(true, false, true, false, false, false, false);
        else if(state == PlayerStatus.State.RunAim)
            setParameters(true, false, true, true, false, false, false);
        else if(state == PlayerStatus.State.CrouchAim)
            setParameters(true, false, false, false, false, false, true);
        else if(state == PlayerStatus.State.CrouchingAim)
            setParameters(true, false, true, false, false, false, true);

        //setParameters(bool PPM, bool LPM, bool WASD, bool SHIFT, bool SPACE, bool R, bool CTRL)
        else if(state == PlayerStatus.State.IdleReload)
            setParameters(PPM, false, false, false, false, true, false);
        else if(state == PlayerStatus.State.WalkReload)
            setParameters(PPM, false, true, false, false, true, false);
        else if(state == PlayerStatus.State.RunReload)
            setParameters(PPM, false, true, true, false, true, false);
        else if(state == PlayerStatus.State.CrouchReload)
            setParameters(PPM, false, false, false, false, true, true);
        else if(state == PlayerStatus.State.CrouchingReload)
            setParameters(PPM, false, true, false, false, true, true);

        animator.SetBool("PPM", PPM);
        animator.SetBool("LPM", LPM);
        animator.SetBool("WASD", WASD);
        animator.SetBool("SHIFT", SHIFT);
        animator.SetBool("SPACE", SPACE);
        animator.SetBool("CTRL", CTRL);
        animator.SetBool("R", R);

    }

    private void setParameters(bool PPM, bool LPM, bool WASD, bool SHIFT, bool SPACE, bool R, bool CTRL)
    {
        this.PPM = PPM;
        this.LPM = LPM;
        this.WASD = WASD;
        this.SHIFT = SHIFT;
        this.SPACE = SPACE;
        this.R = R;
        this.CTRL= CTRL;
    }
}
