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
        Ctx.CanStartWallTimer = false;
        Ctx.WallClingTime = Ctx.MaxWallClingTime;
        Ctx.ForceSlowDownRate = 5;
        Ctx.IsAired = false;
        Ctx.DesiredMoveForce = Ctx.MoveSpeed;
        Ctx.IsJumpTime = Ctx.MaxJumpTime;
        Ctx.JumpMent = new Vector3(0, 1, 0);


        Ctx.GrappleHooks = 1;

        if (Ctx.MoveForce < Ctx.MoveSpeed)
        {
            Ctx.MoveForce = Ctx.MoveSpeed;
        }
    }

    public override void ExitState()
    {
    }

    #region MonoBehaveiours

    public override void UpdateState()
    {
        Ctx.PlayerAnimator.SetFloat("Running", Ctx.MovementSpeed);
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
        else if (Ctx.IsMove && Ctx.IsSlide && Ctx.MoveForce >= Ctx.MoveSpeed && !Ctx.IsAired)
        {
            SetSubState(Factory.Slide());
        }
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsGrounded && !Ctx.IsSloped)
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsJump && Ctx.VaultLow && !Ctx.VaultMedium)
        {
            SwitchState(Factory.Vaulted());
        }
        else if (Ctx.IsJump)
        {
            SwitchState(Factory.Jump());
        }
        else if (Ctx.IsSloped)
        {
            SwitchState(Factory.Sloped());
        }
        else if (Ctx.IsGrappled && Ctx.IsShoot && Ctx.GrappleHooks > 0)
        {
            SwitchState(Factory.Grappled());
        }
    }
}