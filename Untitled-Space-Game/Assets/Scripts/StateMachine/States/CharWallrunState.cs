using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharWallrunState : CharBaseState
{
    public CharWallrunState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        Ctx.Rb.useGravity = false;
        Ctx.IsWallRunning = true;


        Ctx.Rb.velocity = new Vector3(Ctx.Rb.velocity.x, 0f, Ctx.Rb.velocity.z);
        Ctx.PlayerAnimator.SetBool("IsWallRunning", Ctx.IsWallRunning);
        Ctx.PlayerAnimator.SetFloat("WallRun", Ctx.WallLeftRight);


        if (Ctx.CurrentWall != null)
        {
            Ctx.CurrentWall = Ctx.WallLeft ? Ctx.LeftWallHit.transform : Ctx.WallRight ? Ctx.RightWallHit.transform : null;
            if (Ctx.CurrentWall != Ctx.PreviousWall)
            {
                Ctx.GrappleHooks = 1;
                Ctx.WallClingTime = Ctx.MaxWallClingTime;
                Ctx.CanStartWallTimer = true;
            }
            else
            {
                Ctx.CanStartWallTimer = true;
            }
        }
        else
        {
            Ctx.CurrentWall = Ctx.WallLeft ? Ctx.LeftWallHit.transform : Ctx.WallRight ? Ctx.RightWallHit.transform : null;
            Ctx.CanStartWallTimer = true;
        }
    }

    public override void ExitState()
    {
        Ctx.IsWallRunning = false;

        Ctx.PlayerAnimator.SetBool("WallRunningL", false);
        Ctx.PlayerAnimator.SetBool("WallRunningR", false);

        Ctx.PlayerAnimator.SetBool("IsWallRunning", Ctx.IsWallRunning);
        Ctx.Rb.useGravity = true;
        Ctx.PreviousWall = Ctx.CurrentWall;
    }

    #region MonoBehaveiours

    public override void UpdateState()
    {
        Ctx.WallRunDownForce = 10 - Ctx.MoveForce;

        if (Ctx.WallClingTime <= 0 || Ctx.MovementSpeed <= 2)
        {
            Ctx.Rb.AddForce(Vector3.down * Ctx.WallRunDownForce, ForceMode.Force);
            Ctx.DesiredMoveForce = 0f;
            Ctx.MoveMultiplier = 0.5f;
        }
        else
        {
            Ctx.DesiredMoveForce = Ctx.WallRunSpeed;
        }
    }

    public override void LateUpdateState() { }

    public override void FixedUpdateState()
    {
        CheckSwitchStates();
        WallRunMovement();
    }

    #endregion

    public override void InitializeSubState()
    {
        if (Ctx.IsMove)
        {
            SetSubState(Factory.Walk());
        }
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsMove || !Ctx.IsWalled)
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsJump)
        {
            SwitchState(Factory.Jump());
        }
        else if (Ctx.IsGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }
    private void WallRunMovement()
    {
        if (Ctx.Rb.velocity.y > 0)
        {
            Ctx.Rb.velocity = new Vector3(Ctx.Rb.velocity.x, 0f, Ctx.Rb.velocity.z);
        }


        if ((Ctx.PlayerObj.forward - Ctx.WallForward).magnitude > (Ctx.PlayerObj.forward - -Ctx.WallForward).magnitude)
        {
            Ctx.WallForward = new Vector3(-Ctx.WallForward.x, -Ctx.WallForward.y, -Ctx.WallForward.z).normalized;
        }

        if (!Ctx.IsExitingSlope)
        {
            Ctx.Rb.AddForce(-Ctx.WallNormal.normalized * 225, ForceMode.Force);
        }

        Ctx.JumpMent = new Vector3(Ctx.WallNormal.x * 3, 1, Ctx.WallNormal.z * 3);

        Debug.DrawRay(Ctx.transform.position, Ctx.WallForward, Color.green);

        Ctx.Movement = new Vector3(Ctx.WallForward.x, 0, Ctx.WallForward.z).normalized;
        Ctx.MoveMultiplier = 2f;

        // Ctx.Rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        // upwards/downwards force
        // if (upwardsRunning)
        //     rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        // if (downwardsRunning)
        //     rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);


        // ?? Maybe use this ?? if (!(Ctx.WallLeft && Ctx.CurrentMovementInput.x > 0) && !(Ctx.WallRight && Ctx.CurrentMovementInput.x < 0))
    }
}