using UnityEngine;

public abstract class CharacterAbstractState
{
    public CharacterAbstractState(CharacterContextManager currentContextManager, CharacterStateFactory stateFactory, CharacterAnimationManager animationManager)
    {
        _characterContextManager = currentContextManager;
        _characterStateFactory = stateFactory;
        _characterAnimationManager = animationManager;
    }

    private bool _isRootState = false;
    private CharacterContextManager _characterContextManager;
    private CharacterAnimationManager _characterAnimationManager;
    private CharacterStateFactory _characterStateFactory;
    private CharacterAbstractState _currentSuperState;
    private CharacterAbstractState _currentSubState;

    protected bool IsRootState { set { _isRootState = value; } }
    protected CharacterContextManager CharacterContextManager { get { return _characterContextManager; } }
    public CharacterAnimationManager CharacterAnimationManager { get { return _characterAnimationManager; } }
    public CharacterStateFactory CharacterStateFactory { get { return _characterStateFactory; } }
    public CharacterAbstractState CurrentSuperState { get { return _currentSuperState; } }
    public CharacterAbstractState CurrentSubState { get { return _currentSubState; } }
    public bool IsWallColliding { get => _characterContextManager.CollisionDetector.IsTouchingWall; }
    public bool Grounded { get => _characterContextManager.CollisionDetector.IsGrounded; }
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

        CharacterContextManager.Rigidbody.MovePosition(CharacterContextManager.Rigidbody.position + CharacterContextManager.PhysicsManager.MovePosition * Time.fixedDeltaTime);
        
        if (_currentSubState != null)
        {
            _currentSubState.FixedUpdateStates();
        }
    }
    public void UpdateStates()
    {
        UpdateState();

        CheckSwitchStates();
        CheckSwitchSubStates();

        if (_currentSubState != null)
        {
             _currentSubState.UpdateStates();
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
    public void SwitchState(CharacterAbstractState newState)
    {
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
    public void SetSubState(CharacterAbstractState newSubState)
    {
        if (_currentSubState != null)
        {
            _currentSubState.ExitState();
        }

        _currentSubState = newSubState;

        if (newSubState != null)
        {
            newSubState.SetSuperState(this);
            _currentSubState.EnterState();
        }

    }
    protected void SetSuperState(CharacterAbstractState newSuperState)
    {
        _currentSuperState = newSuperState;
    }
    
    public virtual void OnCollisionEnter2D(Collision2D collision) { }
    public virtual void OnCollisionStay(Collision2D collision) { }
    public virtual void OnCollisionExit2D(Collision2D collision) { }
    public virtual void OnTriggerEnter2D(Collider2D collision) { }
    public virtual void OnTriggerStay2D(Collider2D collision) { }
    public virtual void OnTriggerExit2D(Collider2D collision) { }
} 
