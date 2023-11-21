using UnityEngine;

public class CharExhaustState : CharBaseState
{
    public CharExhaustState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory) { }

    public override void EnterState()
    {
        Ctx.ExhaustTime = Ctx.MaxExhastTime;

        Ctx.DesiredMoveForce = Ctx.ExhaustSpeed;

        if (Ctx.MoveForce > Ctx.ExhaustSpeed)
        {
            Ctx.MoveForce = Ctx.ExhaustSpeed;
        }
    }

    public override void ExitState()
    {
        Ctx.ExhaustTime = Ctx.MaxExhastTime;
    }

    #region MonoBehaveiours

    public override void UpdateState()
    {
        CheckSwitchStates();
        ExhaustTime();
    }

    public override void LateUpdateState() { }

    public override void FixedUpdateState()
    {
        ExhaustMovement();
    }

    #endregion

    public override void InitializeSubState() { }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsMove && !Ctx.IsCrouch && Ctx.ExhaustTime < 0)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsMove && !Ctx.IsRun && Ctx.ExhaustTime < 0)
        {
            SwitchState(Factory.Walk());
        }
        else if (Ctx.IsMove && Ctx.IsRun && Ctx.ExhaustTime < 0)
        {
            SwitchState(Factory.Run());
        }
    }

    private void ExhaustMovement()
    {
        Ctx.Rb.AddForce(Ctx.Movement * Ctx.MoveForce * 10f * Ctx.MoveMultiplier, ForceMode.Force);
    }

    private void ExhaustTime()
    {
        Ctx.ExhaustTime -= Time.deltaTime;
    }
}