using UnityEngine;

public class CharRunState : CharBaseState
{
    public CharRunState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory) { }

    public override void EnterState()
    {
        // Run Animation should be true
        Ctx.IsRunning = true;
        Ctx.PlayerAnimator.SetBool(Ctx.RunningAnimation, Ctx.IsRunning);

        Ctx.DesiredMoveForce = Ctx.RunSpeed;

        if (Ctx.MoveForce < Ctx.RunSpeed)
        {
            Ctx.MoveForce = Ctx.RunSpeed;
        }

        Ctx.DecreaseStamina = true;
    }

    public override void ExitState()
    {
        // Run Animation should be false
        Ctx.IsRunning = false;
        Ctx.PlayerAnimator.SetBool(Ctx.RunningAnimation, Ctx.IsRunning);

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