using UnityEngine;

public class CharJumpState : CharBaseState
{
    public CharJumpState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        Debug.Log("JUMP ENTER");

        Ctx.IsJumping = true;

        // Ctx.PlayerAnimator.SetBool("Jump", true);
        Ctx.IsExitingSlope = true;

        HandleJump();
    }

    public override void ExitState()
    {
        Ctx.IsJumping = false;
        Ctx.IsExitingSlope = false;
        // Ctx.PlayerAnimator.SetBool("Jump", false);
    }

    #region MonoBehaveiours

    public override void UpdateState() { }

    public override void FixedUpdateState()
    {
        CheckSwitchStates();
        HandleJumpTime();
    }

    public override void LateUpdateState() { }

    #endregion

    public override void InitializeSubState()
    {
        if (!Ctx.IsMove)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsMove)
        {
            SetSubState(Factory.Walk());
        }
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsGrounded && Ctx.JumpTimer >= 0)
        {
            SwitchState(Factory.Grounded());
        }
        else if (Ctx.IsSloped && Ctx.JumpTimer >= 0)
        {
            SwitchState(Factory.Sloped());
        }
        else if (!Ctx.IsGrounded && !Ctx.IsSloped)
        {
            SwitchState(Factory.Fall());
        }
    }

    void HandleJump()
    {
        Debug.Log("jump");
        Ctx.Rb.velocity = new Vector3(Ctx.Rb.velocity.x, 0f, Ctx.Rb.velocity.z);
        Ctx.Rb.AddForce(Vector3.up * Ctx.JumpForce, ForceMode.Impulse);
    }



    void HandleJumpTime()
    {
        if (Ctx.JumpTimer < Ctx.IsJumpTime)
        {
            Ctx.JumpTimer += Time.deltaTime;
        }
        else if (Ctx.JumpTimer >= Ctx.IsJumpTime)
        {
            Ctx.JumpTimer = 0;
        }
    }
}
