using UnityEngine;

public class CharWalkState : CharBaseState
{
    public CharWalkState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory) { }

    public override void EnterState()
    {
        Debug.Log("ENTER WALK");
        Ctx.DesiredMoveForce = Ctx.WalkSpeed;

        if (Ctx.MoveForce > Ctx.WalkSpeed)
        {
            Ctx.MoveForce = Ctx.WalkSpeed;
        }
    }

    public override void ExitState() { }

    #region MonoBehaveiours

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void LateUpdateState() { }

    public override void FixedUpdateState()
    {
        WalkMovement();
    }

    #endregion

    public override void InitializeSubState() { }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsMove && !Ctx.IsCrouch)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsMove && Ctx.IsRun && !Ctx.IsCrouch && Ctx.Stamina > 0)
        {
            Debug.Log("walk > run");
            SwitchState(Factory.Run());
        }
        else if (Ctx.IsCrouch)
        {
            SwitchState(Factory.Crouch());
        }
    }

    private void WalkMovement()
    {
        Debug.Log("WALK");
        Ctx.Rb.AddForce(Ctx.Movement * Ctx.MoveForce * 10f * Ctx.MoveMultiplier, ForceMode.Force);
    }
}