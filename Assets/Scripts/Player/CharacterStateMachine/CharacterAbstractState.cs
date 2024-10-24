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
    public abstract void UpdateState();
    public abstract void FixedUpdateState();
    public abstract void LateUpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubStates();
    public void UpdateStates()
    {
        UpdateState();

        if (_currentSubState != null)
        {
            Debug.Log("UPDATING SUB STATE");
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
    public void SetSubState(CharacterAbstractState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {

    }
    public void OnCollisionStay(Collision collision)
    {

    }
    public void OnCollisionExit2D(Collision2D collision)
    {

    }
    public void OnTriggerEnter2D(Collider2D collision)
    {

    }
    public void OnTriggerStay2D(Collider2D collision)
    {

    }
    public void OnTriggerExit2D(Collider2D collision)
    {

    }
}
