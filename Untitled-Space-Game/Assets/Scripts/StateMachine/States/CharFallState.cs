using UnityEngine;

public class CharFallState : CharBaseState
{
    public CharFallState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        Ctx.IsAired = true;
        Ctx.MoveMultiplier = Ctx.AirSpeed;
        Ctx.ForceSlowDownRate = 1;
    }

    public override void ExitState()
    {
        Ctx.IsAired = false;
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
        else if (Ctx.IsMove && !Ctx.IsSlide)
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
        else if (Ctx.IsJump && Ctx.VaultLow && !Ctx.VaultMedium)
        {
            SwitchState(Factory.Vaulted());
        }
        else if (Ctx.IsSloped)
        {
            SwitchState(Factory.Sloped());
        }
        else if (Ctx.IsWalled && !(Ctx.WallLeft && Ctx.CurrentMovementInput.x > 0) && !(Ctx.WallRight && Ctx.CurrentMovementInput.x < 0) && Ctx.IsMove)
        // else if (Ctx.IsWalled && !(Ctx.WallLeft && Ctx.CurrentMovementInput.x > 0) && !(Ctx.WallRight && Ctx.CurrentMovementInput.x < 0) && Ctx.IsMove && Ctx.IsWallAngle)
        {
            SwitchState(Factory.Walled());
        }
        else if (Ctx.IsGrappled && Ctx.IsShoot && Ctx.GrappleHooks > 0)
        {
            SwitchState(Factory.Grappled());
        }
    }
}