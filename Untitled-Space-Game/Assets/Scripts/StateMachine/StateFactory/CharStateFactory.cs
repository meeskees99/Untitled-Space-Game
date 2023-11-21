using System.Collections.Generic;

enum CharStates
{
    GROUNDED,
    FALL,
    SLOPED,
    IDLE,
    WALK,
    RUN,
    EXHAUST,
    JUMP,
    CROUCH,
}

public class CharStateFactory
{
    CharStateMachine _context;
    Dictionary<CharStates, CharBaseState> _states = new Dictionary<CharStates, CharBaseState>();

    public CharStateFactory(CharStateMachine currentContext)
    {
        _context = currentContext;
        _states[CharStates.GROUNDED] = new CharGroundedState(_context, this);
        _states[CharStates.FALL] = new CharFallState(_context, this);
        _states[CharStates.SLOPED] = new CharSlopeState(_context, this);
        _states[CharStates.IDLE] = new CharIdleState(_context, this);
        _states[CharStates.WALK] = new CharWalkState(_context, this);
        _states[CharStates.RUN] = new CharRunState(_context, this);
        _states[CharStates.EXHAUST] = new CharExhaustState(_context, this);
        _states[CharStates.JUMP] = new CharJumpState(_context, this);
        _states[CharStates.CROUCH] = new CharCrouchState(_context, this);

    }

    public CharBaseState Grounded()
    {
        return _states[CharStates.GROUNDED];
    }

    public CharBaseState Fall()
    {
        return _states[CharStates.FALL];
    }

    public CharBaseState Sloped()
    {
        return _states[CharStates.SLOPED];
    }

    public CharBaseState Idle()
    {
        return _states[CharStates.IDLE];
    }

    public CharBaseState Walk()
    {
        return _states[CharStates.WALK];
    }

    public CharBaseState Run()
    {
        return _states[CharStates.RUN];
    }

    public CharBaseState Exhaust()
    {
        return _states[CharStates.EXHAUST];
    }

    public CharBaseState Jump()
    {
        return _states[CharStates.JUMP];
    }

    public CharBaseState Crouch()
    {
        return _states[CharStates.CROUCH];
    }
}