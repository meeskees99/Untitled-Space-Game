public class CharIdleState : CharBaseState
{
    public CharIdleState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory) { }

    public override void EnterState()
    {

    }

    public override void ExitState() { }


    #region MonoBehaveiours

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void FixedUpdateState() { }

    public override void LateUpdateState() { }

    #endregion

    public override void InitializeSubState() { }

    public override void CheckSwitchStates()
    {
        if (Ctx.IsMove && !Ctx.IsSlide)
        {
            SwitchState(Factory.Walk());
        }
        else if (Ctx.IsMove && Ctx.IsSlide)
        {
            SwitchState(Factory.Slide());
        }
    }
}