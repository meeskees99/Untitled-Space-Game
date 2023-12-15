using UnityEngine;

public class CharCrouchState : CharBaseState
{
    public CharCrouchState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory) { }

    public override void EnterState()
    {
        // Crouch animation should be true
        Ctx.IsCrouching = true;
        Ctx.PlayerAnimator.SetBool(Ctx.CrouchingAnimation, Ctx.IsCrouching);


        // Ctx.PlayerObj.localScale = new Vector3(1, 0.5f, 1);

        Ctx.DesiredMoveForce = Ctx.CrouchSpeed;

        if (Ctx.MoveForce > Ctx.CrouchSpeed)
        {
            Ctx.MoveForce = Ctx.CrouchSpeed;
        }
    }

    public override void ExitState()
    {
        // Crouch animation should be false
        Ctx.IsCrouching = false;
        Ctx.PlayerAnimator.SetBool(Ctx.CrouchingAnimation, Ctx.IsCrouching);

        // Ctx.PlayerObj.localScale = new Vector3(1, 1, 1);
    }

    #region MonoBehaveiours

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void LateUpdateState() { }

    public override void FixedUpdateState()
    {
        CrouchMovement();
    }

    #endregion

    public override void InitializeSubState() { }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsCrouch && !Ctx.IsMove)
        {
            SwitchState(Factory.Idle());
        }
        else if (!Ctx.IsCrouch && Ctx.IsMove && !Ctx.IsRun)
        {
            SwitchState(Factory.Walk());
        }
        else if (!Ctx.IsCrouch && Ctx.IsMove && Ctx.IsRun)
        {
            SwitchState(Factory.Run());
        }
    }

    private void CrouchMovement()
    {
        Ctx.Rb.AddForce(Ctx.Movement * Ctx.MoveForce * 10f * Ctx.MoveMultiplier, ForceMode.Force);
    }

}