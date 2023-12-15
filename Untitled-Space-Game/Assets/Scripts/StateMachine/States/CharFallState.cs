using UnityEngine;

public class CharFallState : CharBaseState
{
    public CharFallState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        // Fall animation should be true
        Ctx.IsFalling = true;
        Ctx.PlayerAnimator.SetBool(Ctx.FallingAnimation, !Ctx.IsFalling);

        InitializeSubState();
        Ctx.IsFalling = true;
        Ctx.MoveMultiplier = Ctx.AirSpeed;
    }

    public override void ExitState()
    {
        // Fall animation should be false
        Ctx.IsFalling = false;
        Ctx.PlayerAnimator.SetBool(Ctx.FallingAnimation, !Ctx.IsFalling);
    }

    #region MonoBehaveiours

    public override void UpdateState()
    {
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
        if (Ctx.IsGrounded)
        {
            SwitchState(Factory.Grounded());
        }
        else if (Ctx.IsSloped)
        {
            SwitchState(Factory.Sloped());
        }
    }
}