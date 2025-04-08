using UnityEngine;

public abstract class CharacterAbstractState
{
    public CharacterAbstractState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, PlayerInputManager inputManager, CharacterAnimationManager animationManager)
    {
        _characterContextManager = currentContextManager;
        _characterStateFactory = stateFactory;
        _playerInputManager = inputManager;
        _characterAnimationManager = animationManager;
    }

    private bool _isRootState = false;
    private PlayerInputManager _playerInputManager;
    private CharacterContextManager _characterContextManager;
    private CharacterAnimationManager _characterAnimationManager;
    private CharacterStateFactory _characterStateFactory;
    private CharacterAbstractState _currentSuperState;
    private CharacterAbstractState _currentSubState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected CharacterContextManager CharacterContextManager { get { return _characterContextManager; } }
    public CharacterAnimationManager CharacterAnimationManager { get { return _characterAnimationManager; } }
    public PlayerInputManager PlayerInputManager { get { return _playerInputManager; } }
    public CharacterStateFactory CharacterStateFactory { get { return _characterStateFactory; } }
    public CharacterAbstractState CurrentSuperState { get { return _currentSuperState; } }
    public CharacterAbstractState CurrentSubState { get { return _currentSubState; } }
    protected bool IsWallColliding { get => Physics2D.OverlapBox(CharacterContextManager.WallCheckerPoint.position, new Vector2(0.06f, 0.15f), 0.00f, CharacterContextManager.WallLayerTarget); }
    protected bool Grounded { get => Physics2D.OverlapBox(CharacterContextManager.transform.position, new Vector2(0.40f, 0.04f), 0.00f, CharacterContextManager.GroundLayerTarget); }
    public int CharacterForwardDirection { get => (int)Vector3.SignedAngle(Vector3.right, CharacterAnimationManager.CharacterAnimator.transform.right, Vector3.up) < 0 ? -1 : 1; }
    protected float DashSpeed { get; set; }

    public abstract void EnterState();
    public abstract void FixedUpdateState();
    public abstract void UpdateState();
    public abstract void LateUpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void CheckSwitchSubStates();
    public abstract Quaternion CurrentLookRotation();

    public void FixedUpdateStates()
    {
        FixedUpdateState();

        CharacterContextManager.Rigidbody.MovePosition(CharacterContextManager.Rigidbody.position + CharacterContextManager.MovePosition * Time.fixedDeltaTime);

        if (_currentSubState != null)
        {
            _currentSubState.FixedUpdateStates();
        }
    }
    public void UpdateStates()
    {
        _characterContextManager.SetCoyoteTime();
        _characterContextManager.SetDashCoolDownTime();

        UpdateState();

        CheckSwitchStates();
        CheckSwitchSubStates();

        if (_currentSubState != null)
        {
             _currentSubState.UpdateStates();
        }

        if (_isRootState)
        {
            if (_characterContextManager.TakingDamage)
            {
                SwitchState(_characterStateFactory.DamagedState());
            }

            if (_characterContextManager.SpawningCharacter)
            {
                SwitchState(_characterStateFactory.SpawningState());
            }
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
    protected void SwitchState(CharacterAbstractState newState)
    {
        if (_isRootState)
        {
            _characterContextManager.ExitState = _characterContextManager.CurrentState;
        }

        ExitState();

        if (_isRootState)
        {
            _characterContextManager.CurrentState = newState;
            _characterContextManager.CurrentState.EnterState();
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
    public abstract void OnCollisionStay(Collision2D collision);
    public abstract void OnCollisionExit2D(Collision2D collision);
    public abstract void OnTriggerEnter2D(Collider2D collision);
    public abstract void OnTriggerStay2D(Collider2D collision);
    public abstract void OnTriggerExit2D(Collider2D collision);
}          
