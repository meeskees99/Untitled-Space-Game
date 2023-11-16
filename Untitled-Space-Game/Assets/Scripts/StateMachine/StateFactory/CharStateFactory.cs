using System.Collections.Generic;

enum CharStates
{
    Grounded,
    Fall,
    Jump,
    Idle,
    Walk,
    Slide,
    Sloped,
    Walled,
    Grappled,
    Vaulted,
}

public class CharStateFactory
{
    CharStateMachine _context;
    Dictionary<CharStates, CharBaseState> _states = new Dictionary<CharStates, CharBaseState>();

    public CharStateFactory(CharStateMachine currentContext)
    {
        _context = currentContext;
        _states[CharStates.Grounded] = new CharGroundedState(_context, this);
        _states[CharStates.Fall] = new CharFallState(_context, this);
        _states[CharStates.Jump] = new CharJumpState(_context, this);
        _states[CharStates.Idle] = new CharIdleState(_context, this);
        _states[CharStates.Walk] = new CharWalkState(_context, this);
        _states[CharStates.Slide] = new CharSlideState(_context, this);
        _states[CharStates.Sloped] = new CharSlopeState(_context, this);
        _states[CharStates.Walled] = new CharWallrunState(_context, this);
        _states[CharStates.Grappled] = new CharGrappledState(_context, this);
        _states[CharStates.Vaulted] = new CharVaultState(_context, this);
    }

    public CharBaseState Grounded()
    {
        return _states[CharStates.Grounded];
    }

    public CharBaseState Fall()
    {
        return _states[CharStates.Fall];
    }

    public CharBaseState Jump()
    {
        return _states[CharStates.Jump];
    }

    public CharBaseState Idle()
    {
        return _states[CharStates.Idle];
    }

    public CharBaseState Walk()
    {
        return _states[CharStates.Walk];
    }

    public CharBaseState Slide()
    {
        return _states[CharStates.Slide];
    }

    public CharBaseState Sloped()
    {
        return _states[CharStates.Sloped];
    }

    public CharBaseState Walled()
    {
        return _states[CharStates.Walled];
    }

    public CharBaseState Grappled()
    {
        return _states[CharStates.Grappled];
    }

    public CharBaseState Vaulted()
    {
        return _states[CharStates.Vaulted];
    }
}