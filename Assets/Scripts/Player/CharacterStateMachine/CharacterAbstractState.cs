using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterAbstractState
{
    private bool _isRootState = false;
    private PlayerContextManager _playerContextManager;
    private CharacterStateFactory _playerStateFactory;
    private CharacterAbstractState _currentSuperState;
    private CharacterAbstractState _currentSubState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected PlayerContextManager PlayerContextManager { get { return _playerContextManager; } }
    protected CharacterStateFactory PlayerStateFactory { get { return _playerStateFactory; } }

    public CharacterAbstractState(PlayerContextManager currentContextManager, CharacterStateFactory stateFactory)
    {
        _playerContextManager = currentContextManager;
        _playerStateFactory = stateFactory;
    }

    public abstract void EnterState();
    public abstract void FixedUpdateState();
    public abstract void UpdateState();
    public abstract void LateUpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubStates();
    public void FixedUpdateStates()
    {
        FixedUpdateState();

        if (_currentSubState != null)
        {
            _currentSubState.FixedUpdateState();
        }
    }
    public void UpdateStates()
    {
        UpdateState();

        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }
    protected void SwitchState(CharacterAbstractState newState)
    {
        ExitState();

        newState.EnterState();

        if (_isRootState)
        {
            _playerContextManager.CurrentState = newState;
        }
        else if (_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
        }
    }
    protected void SetSuperState(CharacterAbstractState newSuperState)
    {
        _currentSuperState = newSuperState;
    }
    protected void SetSubState(CharacterAbstractState newSubState)
    {
        if (_currentSubState != null)
        {
            _currentSubState.ExitState();
        }

        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
        _currentSubState.EnterState();
    }
    public abstract void OnCollisionEnter2D(Collision2D collision);
    public abstract void OnCollisionStay(Collision collision);
    public abstract void OnCollisionExit2D(Collision2D collision);
    public abstract void OnTriggerEnter2D(Collider2D collision);
    public abstract void OnTriggerStay2D(Collider2D collision);
    public abstract void OnTriggerExit2D(Collider2D collision);
    protected virtual void ProccessJumpInput(bool actionInput)
    {

    }
    protected virtual void ProccessMoveInput(float moveInput)
    {
        PlayerContextManager.transform.rotation = TargetRotation(moveInput);

        Quaternion TargetRotation(float moveInput)
        {
            float angle = Mathf.Atan2(0, moveInput) * Mathf.Rad2Deg;
            return Quaternion.AngleAxis(angle, Vector3.up);
        }
    }
}          
