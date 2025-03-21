using System.Collections.Generic;

public enum ECharacterState
{
    Grounded = 0,
    Ungrounded,
    Idle,
    Moving,
    Jumping,
    Falling,
    Damaged,
    Interacting,
    Spawning
}

public class CharacterStateFactory
{
    PlayerContextManager _contextManager;
    Dictionary<ECharacterState, CharacterAbstractState> _states = new Dictionary<ECharacterState, CharacterAbstractState>();

    public CharacterStateFactory(PlayerContextManager currentContextManager)
    {
        _contextManager = currentContextManager;
        _states[ECharacterState.Grounded] = new CharacterGroundedState(_contextManager, this);
        _states[ECharacterState.Ungrounded] = new CharacterUngroundedState(_contextManager, this);
        _states[ECharacterState.Idle] = new CharacterIdleState(_contextManager, this);
        _states[ECharacterState.Moving] = new CharacterMoveState(_contextManager, this);
        _states[ECharacterState.Jumping] = new CharacterJumpState(_contextManager, this);
        _states[ECharacterState.Falling] = new CharacterFallState(_contextManager, this);
        _states[ECharacterState.Damaged] = new CharacterDamagedState(_contextManager, this);
        _states[ECharacterState.Interacting] = new CharacterInteractionState(_contextManager, this);
        _states[ECharacterState.Spawning] = new CharacterSpawningState(_contextManager, this);
    }

    public CharacterGroundedState GroundedState()
    {
        return (CharacterGroundedState)_states[ECharacterState.Grounded];
    }
    public CharacterUngroundedState UngroundedState()
    {
        return (CharacterUngroundedState)_states[ECharacterState.Ungrounded];
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
    public CharacterSpawningState SpawningState()
    {
        return (CharacterSpawningState)_states[ECharacterState.Spawning];
    }
}
