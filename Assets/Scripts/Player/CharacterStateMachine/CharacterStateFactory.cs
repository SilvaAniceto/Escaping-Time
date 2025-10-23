using System.Collections.Generic;

public enum ECharacterState
{
    Disabled = 0,
    Grounded,
    Idle,
    Moving,
    Jumping,
    AirJumping,
    Falling,
    Damaged,
    Interacting,
    Reset,
    Dashing,
    OnWall,
    WallJump
}

public class CharacterStateFactory
{
    CharacterContextManager _contextManager;
    PlayerInputManager _inputManager;
    CharacterAnimationManager _animationManager;
    Dictionary<ECharacterState, CharacterAbstractState> _states = new Dictionary<ECharacterState, CharacterAbstractState>();

    public static CharacterStateFactory Instance;

    public CharacterStateFactory(CharacterContextManager currentContextManager, PlayerInputManager inputManager, CharacterAnimationManager animationManager)
    {
        Instance = this;

        _contextManager = currentContextManager;
        _inputManager = inputManager;
        _animationManager = animationManager;
        _states[ECharacterState.Disabled] = new CharacterDisabledState(_contextManager, this, _inputManager, _animationManager);
        _states[ECharacterState.Grounded] = new CharacterGroundedState(_contextManager, this, _inputManager, _animationManager);
        _states[ECharacterState.Idle] = new CharacterIdleState(_contextManager, this, _inputManager, _animationManager);
        _states[ECharacterState.Moving] = new CharacterMoveState(_contextManager, this, _inputManager, _animationManager);
        _states[ECharacterState.Jumping] = new CharacterJumpState(_contextManager, this, _inputManager, _animationManager);
        _states[ECharacterState.AirJumping] = new CharacterAirJumpState(_contextManager, this, _inputManager, _animationManager);
        _states[ECharacterState.Falling] = new CharacterFallState(_contextManager, this, _inputManager, _animationManager);
        _states[ECharacterState.Damaged] = new CharacterDamagedState(_contextManager, this, _inputManager, _animationManager);
        _states[ECharacterState.Interacting] = new CharacterInteractionState(_contextManager, this, _inputManager, _animationManager);
        _states[ECharacterState.Reset] = new CharacterResetState(_contextManager, this, _inputManager, _animationManager);
        _states[ECharacterState.Dashing] = new CharacterDashState(_contextManager, this, _inputManager, _animationManager);
        _states[ECharacterState.OnWall] = new CharacterOnWallState(_contextManager, this, _inputManager, _animationManager);
        _states[ECharacterState.WallJump] = new CharacterWallJumpState(_contextManager, this, _inputManager, _animationManager);
    }

    public CharacterDisabledState DisabledState()
    {
        return (CharacterDisabledState)_states[ECharacterState.Disabled];
    }
    public CharacterGroundedState GroundedState()
    {
        return (CharacterGroundedState)_states[ECharacterState.Grounded];
    }
    public CharacterIdleState IdleState()
    {
        return (CharacterIdleState)_states[ECharacterState.Idle];
    }
    public CharacterMoveState MoveState()
    {
        return (CharacterMoveState)_states[ECharacterState.Moving];
    }
    public CharacterJumpState JumpState()
    {
        return (CharacterJumpState)_states[ECharacterState.Jumping];
    }
    public CharacterAirJumpState AirJumpState()
    {
        return (CharacterAirJumpState)_states[ECharacterState.AirJumping];
    }
    public CharacterFallState FallState()
    {
        return (CharacterFallState)_states[ECharacterState.Falling];
    }
    public CharacterDamagedState DamagedState()
    {
        return (CharacterDamagedState)_states[ECharacterState.Damaged];
    }
    public CharacterInteractionState InteractionState()
    {
        return (CharacterInteractionState)_states[ECharacterState.Interacting];
    }
    public CharacterResetState ResetState()
    {
        return (CharacterResetState)_states[ECharacterState.Reset];
    }
    public CharacterDashState DashState()
    {
        return (CharacterDashState)_states[ECharacterState.Dashing];
    }
    public CharacterOnWallState OnWallState()
    {
        return (CharacterOnWallState)_states[ECharacterState.OnWall];
    }
    public CharacterWallJumpState WallJumpState()
    {
        return (CharacterWallJumpState)_states[ECharacterState.WallJump];
    }
}
