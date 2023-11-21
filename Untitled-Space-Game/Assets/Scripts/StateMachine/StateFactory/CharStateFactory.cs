using System.Collections.Generic;

enum CharStates
{
    Grounded,
    Fall,
    Sloped,
    Idle,
    Jump,
    Walk,
    // Slide,
    // Walled,
    // Grappled,
    // Vaulted,
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
        _states[CharStates.Sloped] = new CharSlopeState(_context, this);
        _states[CharStates.Idle] = new CharIdleState(_context, this);
        _states[CharStates.Walk] = new CharWalkState(_context, this);
        _states[CharStates.Jump] = new CharJumpState(_context, this);
    }

    public CharBaseState Grounded()
    {
        return _states[CharStates.Grounded];
    }

    public CharBaseState Fall()
    {
        return _states[CharStates.Fall];
    }

    public CharBaseState Sloped()
    {
        return _states[CharStates.Sloped];
    }

    public CharBaseState Idle()
    {
        return _states[CharStates.Idle];
    }

    public CharBaseState Walk()
    {
        return _states[CharStates.Walk];
    }

    public CharBaseState Jump()
    {
        return _states[CharStates.Jump];
    }
}