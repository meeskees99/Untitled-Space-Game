using UnityEngine;

public class CharSlopeState : CharBaseState
{
    public CharSlopeState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        Ctx.Rb.useGravity = false;

        Ctx.JumpMent = new Vector3(0, 1, 0);
    }

    public override void ExitState()
    {
        Ctx.Rb.useGravity = true;
    }

    #region MonoBehaveiours

    public override void UpdateState()
    {
        Ctx.Movement = Ctx.GetSlopeMoveDirection(Ctx.CurrentMovement);

        if (Ctx.Rb.velocity.y > 0)
        {
            Ctx.MoveMultiplier = 2f;
        }

        if (Ctx.Rb.velocity.y > 0 || Ctx.Rb.velocity.y > 0 || Ctx.IsSliding)
        {
            Ctx.Rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
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
        else if (Ctx.IsSlide && Ctx.IsMove)
        {
            SetSubState(Factory.Slide());
        }
        else if (Ctx.IsAired)
        {
            SetSubState(Factory.Fall());
        }
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsGrounded && !Ctx.IsSloped)
        {
            SwitchState(Factory.Grounded());
        }
        else if (!Ctx.IsGrounded && !Ctx.IsSloped)
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsJump && !Ctx.IsSlide)
        {
            SwitchState(Factory.Jump());
        }
    }
}
