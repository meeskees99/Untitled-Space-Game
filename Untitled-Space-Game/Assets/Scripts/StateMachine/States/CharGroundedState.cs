using UnityEngine;
public class CharGroundedState : CharBaseState
{
    public CharGroundedState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        Ctx.MoveMultiplier = 1f;
        Ctx.IsAired = false;
        Ctx.DesiredMoveForce = Ctx.WalkSpeed;
        Ctx.IsJumpTime = Ctx.MaxJumpTime;

        if (Ctx.MoveForce < Ctx.WalkSpeed)
        {
            Ctx.MoveForce = Ctx.WalkSpeed;
        }
    }

    public override void ExitState() { }

    #region MonoBehaveiours

    public override void UpdateState()
    {
        // Ctx.PlayerAnimator.SetFloat("Running", Ctx.MovementSpeed);
        Ctx.Movement = Ctx.CurrentMovement.normalized;
    }

    public override void FixedUpdateState()
    {
        CheckSwitchStates();
    }

    public override void LateUpdateState() { }

    #endregion

    public override void InitializeSubState()
    {
        if (!Ctx.IsMove && !Ctx.IsCrouch)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsMove && !Ctx.IsCrouch && !Ctx.IsRun)
        {
            SetSubState(Factory.Walk());
        }
        else if (Ctx.IsMove && !Ctx.IsCrouch && Ctx.IsRun)
        {
            Debug.Log("ground > run");
            SetSubState(Factory.Run());
        }
        else if (Ctx.IsCrouch)
        {
            SetSubState(Factory.Crouch());
        }
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsGrounded && !Ctx.IsSloped)
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsJump)
        {
            SwitchState(Factory.Jump());
        }
        else if (Ctx.IsSloped)
        {
            SwitchState(Factory.Sloped());
        }
    }
}