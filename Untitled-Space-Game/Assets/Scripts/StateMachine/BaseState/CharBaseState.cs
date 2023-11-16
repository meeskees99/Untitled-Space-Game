using Unity.VisualScripting;
using UnityEngine;

public abstract class CharBaseState
{
    protected bool _isRootState = false;
    protected bool IsRootState
    {
        set
        {
            _isRootState = value;
        }
    }

    private CharStateMachine _ctx;
    protected CharStateMachine Ctx
    {
        get
        {
            return _ctx;
        }
    }

    private CharStateFactory _factory;
    protected CharStateFactory Factory
    {
        get
        {
            return _factory;
        }
    }

    private CharBaseState _currentSubState;
    protected CharBaseState _currentSuperState;

    public CharBaseState(CharStateMachine currentContext, CharStateFactory charachterStateFactory)
    {
        _ctx = currentContext;
        _factory = charachterStateFactory;
    }

    public abstract void EnterState();

    public abstract void ExitState();

    public abstract void UpdateState();

    public abstract void FixedUpdateState();

    public abstract void LateUpdateState();

    public abstract void CheckSwitchStates();

    public abstract void InitializeSubState();

    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    public void FixedUpdateStates()
    {
        FixedUpdateState();
        if (_currentSubState != null)
        {
            _currentSubState.FixedUpdateState();
        }
    }

    public void LateUpdateStates()
    {
        LateUpdateState();
        if (_currentSubState != null)
        {
            _currentSubState.LateUpdateState();
        }
    }
    protected void SwitchState(CharBaseState newState)
    {
        ExitState();

        newState.EnterState();

        if (_isRootState)
        {
            _ctx.CurrentState = newState;
        }
        else if (_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
        }
    }

    protected void SetSuperState(CharBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(CharBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }

}
