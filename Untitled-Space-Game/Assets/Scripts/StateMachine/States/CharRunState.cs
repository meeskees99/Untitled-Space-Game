using UnityEngine;

public class CharRunState : CharBaseState
{
    public CharRunState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory) { }

    public override void EnterState()
    {
        Debug.Log("ENTER RUN");

        Ctx.DesiredMoveForce = Ctx.RunSpeed;

        if (Ctx.MoveForce < Ctx.RunSpeed)
        {
            Ctx.MoveForce = Ctx.RunSpeed;
        }

        Ctx.DecreaseStamina = true;
    }

    public override void ExitState()
    {
        Ctx.DecreaseStamina = false;
    }

    #region MonoBehaveiours

    public override void UpdateState()
    {

    }

    public override void LateUpdateState() { }

    public override void FixedUpdateState()
    {
        CheckSwitchStates();

        RunMovement();
    }

    #endregion

    public override void InitializeSubState() { }

    public override void CheckSwitchStates()
    {
        if (Ctx.Stamina < 0)
        {
            SwitchState(Factory.Exhaust());
        }
        else if (!Ctx.IsCrouch && !Ctx.IsMove)
        {
            SwitchState(Factory.Idle());
        }
        else if (!Ctx.IsCrouch && Ctx.IsMove && !Ctx.IsRun)
        {
            SwitchState(Factory.Walk());
        }
    }

    private void RunMovement()
    {
        Ctx.Rb.AddForce(Ctx.Movement * Ctx.MoveForce * 10f * Ctx.MoveMultiplier, ForceMode.Force);
    }

}