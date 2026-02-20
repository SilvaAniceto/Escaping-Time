using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterAnimationManager))][System.Serializable]
public class CharacterContextManager : MonoBehaviour
{
    [Header("Camera Target")]
    [SerializeField] private Transform _cameraTarget;
    [Header("Layer Settings")]
    [SerializeField] private LayerMask _groundLayerTarget;
    [SerializeField] private LayerMask _wallLayerTarget;
    [Header("Collision Settings")]
    [SerializeField] private Transform _wallCheckerPoint;
    [SerializeField] private BoxCollider2D _characterCollider;
    [SerializeField] private BoxCollider2D _ceilingChecker;
    [SerializeField] private BoxCollider2D _groundChecker;
    [SerializeField] private BoxCollider2D _wallChecker;
    [Header("Curve Settings")]
    [SerializeField] private AnimationCurve _accelerationCurve;
    [SerializeField] private AnimationCurve _jumpForceCurve;
    [SerializeField] private AnimationCurve _fallCurve;
    [SerializeField] private AnimationCurve _dashCurve;
    [SerializeField] private AnimationCurve _damageCurve;

    private CharacterAbstractState _currentState;
    public CharacterAbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }

    public PlayerInputManager PlayerInputManager { get; private set; }

    public Transform CameraTarget { get => _cameraTarget; }

    public CameraBehaviourController CameraBehaviourController { get; private set; }

    public IInteractable Interactable { get; set; }

    #region STATE CALLBACK
    [HideInInspector] public UnityEvent OnResetState = new UnityEvent();
    #endregion

    #region POWER UP
    #region Dash
    [SerializeField] private bool _hasInfinityDash;
    public bool HasInfinityDash
    {
        get
        {
            return _hasInfinityDash;
        }
        set
        {
            if (_hasInfinityDash == value)
            {
                return;
            }

            _hasInfinityDash = value;

            if (_hasInfinityDash)
            {
                OnDashPowerStateChange.Invoke("PwrUp_Infinity");
            }
        }
    }

    private bool _hasTemporaryDash;
    public bool HasTemporaryDash 
    {
        get
        {
            return _hasTemporaryDash;
        }
        set
        {
            if (value == _hasTemporaryDash || HasInfinityDash)
            {
                return;
            }    

            _hasTemporaryDash = value;

            if (_hasTemporaryDash)
            {
                OnDashPowerStateChange.Invoke("PwrUp_UI_Lit");
            }
            else
            {
                OnDashPowerStateChange.Invoke("PwrUp_UI_Unlit");
            }
        }
    }

    public bool HasDash
    {
        get
        {
            return HasTemporaryDash || HasInfinityDash ? true : false;
        }
    }
    public bool DashIsAllowed
    {
        get
        {
            return HasDash ? !DashOnCoolDown && !DashIsWaitingGroundedState : false;
        }
    }
    public bool DashOnCoolDown { get; set; }
    public bool DashIsWaitingGroundedState { get; set; }

    public void SetTemporaryDash(float coolDown = 0)
    {
        HasTemporaryDash = true;

        if (coolDown != 0)
        {
            Action tempAirJumpAction = () =>
            {
                HasTemporaryDash = false;
            };

            WaitSeconds(tempAirJumpAction, coolDown);
            GameUIManager.Instance.SetOvertimeDashPowerUpUI(coolDown, this);
        }
    }
    #endregion

    #region AirJump
    [SerializeField] private bool _hasInfinityAirJump;
    public bool HasInfinityAirJump
    {
        get
        {
            return _hasInfinityAirJump;
        }
        set
        {
            if (_hasInfinityAirJump == value)
            {
                return;
            }

            _hasInfinityAirJump = value;

            if (_hasInfinityAirJump)
            {
                OnAirJumpPowerStateChange.Invoke("PwrUp_Infinity");
            }
        }
    }

    private bool _hasTemporaryAirJump;
    public bool HasTemporaryAirJump 
    { 
        get
        {
            return _hasTemporaryAirJump;
        }
        set
        {
            if (value == _hasTemporaryAirJump || HasInfinityAirJump)
            {
                return;
            }

            _hasTemporaryAirJump = value;

            if (_hasTemporaryAirJump)
            {
                OnAirJumpPowerStateChange.Invoke("PwrUp_UI_Lit");
            }
            else
            {
                OnAirJumpPowerStateChange.Invoke("PwrUp_UI_Unlit");
            }
        }
    }

    public bool HasAirJump
    {
        get
        {
            return HasTemporaryAirJump || HasInfinityAirJump ? true : false;
        }
    }
    public bool AirJumpIsAllowed { get; set; }

    public void SetTemporaryAirJump(float coolDown = 0)
    {
        HasTemporaryAirJump = true;

        if (coolDown != 0)
        {
            Action tempAirJumpAction = () =>
            {
                HasTemporaryAirJump = false;
            };

            WaitSeconds(tempAirJumpAction, coolDown);
            GameUIManager.Instance.SetOvertimeAirJumpPowerUpUI(coolDown, this);
        }
    }
    #endregion

    #region WallMove
    [SerializeField] private bool _hasInfinityWallMove;
    public bool HasInfinityWallMove
    {
        get
        {
            return _hasInfinityWallMove;
        }
        set
        {
            if (_hasInfinityWallMove == value)
            {
                return;
            }

            _hasInfinityWallMove = value;

            if (_hasInfinityWallMove)
            {
                OnWallMovePowerStateChange.Invoke("PwrUp_Infinity");
            }
        }
    }

    private bool _hasTemporaryWallMove;
    public bool HasTemporaryWallMove
    {
        get
        {
            return _hasTemporaryWallMove;
        }
        set
        {
            if (value == _hasTemporaryWallMove || HasInfinityWallMove)
            {
                return;
            }

            _hasTemporaryWallMove = value;

            if (_hasTemporaryWallMove)
            {
                OnWallMovePowerStateChange.Invoke("PwrUp_UI_Lit");
            }
            else
            {
                OnWallMovePowerStateChange.Invoke("PwrUp_UI_Unlit");
            }
        }
    }

    public bool HasWallMove
    {
        get
        {
            return HasInfinityWallMove || HasTemporaryWallMove ? true : false;
        }
    }
    public void SetTemporaryWallMove(float coolDown = 0)
    {
        HasTemporaryWallMove = true;

        if (coolDown != 0)
        {
            Action tempAirJumpAction = () =>
            {
                HasTemporaryWallMove = false;
            };

            WaitSeconds(tempAirJumpAction, coolDown);
            GameUIManager.Instance.SetOvertimeWallMovePowerUpUI(coolDown, this);
        }
    }
    #endregion
    #endregion

    #region POWER UP CALLBACKS
    [HideInInspector] public UnityEvent OnPowerUpInteractableRecharge = new UnityEvent();
    [HideInInspector] public UnityEvent<string> OnAirJumpPowerStateChange = new UnityEvent<string>();
    [HideInInspector] public UnityEvent<string> OnDashPowerStateChange = new UnityEvent<string>();
    [HideInInspector] public UnityEvent<string> OnWallMovePowerStateChange = new UnityEvent<string>();

    public void SetPowerUpCallBack()
    {
        OnAirJumpPowerStateChange.AddListener(GameUIManager.Instance.SetAirJumpPowerUpUI);
        OnDashPowerStateChange.AddListener(GameUIManager.Instance.SetDashPowerUpUI);
        OnWallMovePowerStateChange.AddListener(GameUIManager.Instance.SetWallMovePowerUpUI);
    }
    public void DispatchPowerUpInteractableRecharge()
    {
        OnPowerUpInteractableRecharge?.Invoke();
        OnPowerUpInteractableRecharge.RemoveAllListeners();
    }
    #endregion

    #region COLLISION PROPERTIES
    public Rigidbody2D Rigidbody { get; private set; }
    public LayerMask GroundLayerTarget { get => _groundLayerTarget; }
    public LayerMask WallLayerTarget { get => _wallLayerTarget; }
    public Transform WallCheckerPoint { get => _wallCheckerPoint; }
    public BoxCollider2D CharacterCollider { get => _characterCollider; }
    public BoxCollider2D CeilingChecker { get => _ceilingChecker; }
    public BoxCollider2D GroundChecker { get => _groundChecker; }
    public BoxCollider2D WallChecker { get => _wallChecker; }
    #endregion

    #region PHYSICS MOVEMENT PROPERTIES
    public int MoveDirection { get; set; }
    public Vector2 MovePosition
    {
        get
        {
            return new Vector2(HorizontalSpeed, VerticalSpeed);
        }
    }
    public float HorizontalSpeed { get; set; }
    public float VerticalSpeed { get; set; }
    public float FallStartSpeed { get; set; }
    public bool CoyoteTime { get; set; }
    public float HorizontalStartSpeed { get; set; }
    public float HorizontalTopSpeed { get; set; }
    public float HorizontalSpeedOvertime { get; set; }
    public float HorizontalSpeedLerpOvertime
    {
        get
        {
            HorizontalSpeedOvertime += Time.deltaTime / 0.62f;
            HorizontalSpeedOvertime = Mathf.Clamp01(HorizontalSpeedOvertime);

            return _accelerationCurve.Evaluate(HorizontalSpeedOvertime);
        }
    }
    public float GravityUpwardSpeedOvertime { get; set; }
    public float GravityUpwardSpeedLerpOvertime
    {
        get
        {
            GravityUpwardSpeedOvertime += Time.deltaTime / 0.36f;
            GravityUpwardSpeedOvertime = Mathf.Clamp01(GravityUpwardSpeedOvertime);

            return _jumpForceCurve.Evaluate(GravityUpwardSpeedOvertime);
        }
    }
    public float GravityDownwardSpeedOvertime { get; set; }
    public float GravityDownwardSpeedLerpOvertime
    {
        get
        {
            GravityDownwardSpeedOvertime += Time.deltaTime / 0.48f;
            GravityDownwardSpeedOvertime = Mathf.Clamp01(GravityDownwardSpeedOvertime);

            return _fallCurve.Evaluate(GravityDownwardSpeedOvertime);
        }
    }
    public float DashSpeedOvertime { get; set; }
    public float DashSpeedLerpOvertime
    {
        get
        {
            DashSpeedOvertime += Time.deltaTime / 0.62f;
            DashSpeedOvertime = Mathf.Clamp01(DashSpeedOvertime);

            return _dashCurve.Evaluate(DashSpeedOvertime);
        }
    }
    public float DamageSpeedOvertime { get; set; }
    public float DamageSpeedLerpOvertime
    {
        get
        {
            DamageSpeedOvertime += Time.deltaTime / 0.62f;
            DamageSpeedOvertime = Mathf.Clamp01(DamageSpeedOvertime);

            return _damageCurve.Evaluate(DamageSpeedOvertime);
        }
    }
    public bool DamageOnCoolDown { get; set; } = false;
    #endregion

    #region PHYSICS DETECTION PROPERTIES
    public Rigidbody2D FixedJointConnectedBody { get; set;}
    public Joint2D FixedJoint2D { get; set; }
    #endregion

    #region DAMAGE PROPERTIES
    public float DamageHitDirection { get; private set; }
    public Vector3 SpawningPosition { get; set; }
    #endregion

    #region INITIALIZATION
    public void InitializeCharacterContextManager(PlayerInputManager playerInputManager, CameraBehaviourController cameraBehaviourController, bool isGameContext = true)
    {
        PlayerInputManager = playerInputManager;
        CameraBehaviourController = cameraBehaviourController;

        Rigidbody = GetComponent<Rigidbody2D>();
        FixedJoint2D = GetComponent<FixedJoint2D>();

        DisableFixedJoint2D();

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Ceiling"), LayerMask.NameToLayer("Default"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Ground"), LayerMask.NameToLayer("Default"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("WallChecker"), LayerMask.NameToLayer("Default"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("WallChecker"), LayerMask.NameToLayer("Camera Objects"));

        _currentState = isGameContext ? new CharacterStateFactory(this,  GetComponent<CharacterAnimationManager>()).DisabledState() : new CharacterStateFactory(this, GetComponent<CharacterAnimationManager>()).GroundedState();

        _currentState.CharacterAnimationManager.CharacterAnimator = _currentState.CharacterAnimationManager.GetComponentInChildren<Animator>();

        _currentState.EnterState();
    }
    #endregion

    #region CHARACTER CONTEXT
    public void WaitFrameEnd(Action action)
    {
        StartCoroutine(OnWaitFrameEnd(action));
    }
    IEnumerator OnWaitFrameEnd(Action action)
    {
        yield return new WaitForEndOfFrame();
        if (action != null)
        {
            action();
        }
    }
    public void WaitSeconds(Action action, float waitTime)
    {
        StartCoroutine(OnWaitSeconds(action, waitTime));
    }
    IEnumerator OnWaitSeconds(Action action, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (action != null)
        {
            action();
        }
    }
    public void EnableFixedJoint2D()
    {
        if (FixedJoint2D.enabled || !FixedJointConnectedBody || CurrentState.CurrentSubState != CurrentState.CharacterStateFactory.IdleState() || CurrentState != CurrentState.CharacterStateFactory.GroundedState())
        {
            return;
        }

        FixedJoint2D.connectedBody = FixedJointConnectedBody;
        FixedJoint2D.enableCollision = true;
        FixedJoint2D.enabled = true;
    }
    public void DisableFixedJoint2D()
    {
        FixedJoint2D.enabled = false;
        FixedJoint2D.connectedBody = null;
    }
    public void ApplyDamage(float damageDirection)
    {
        PlayerInputManager.DisableInputAction();

        DamageHitDirection = damageDirection;

        _currentState = new CharacterStateFactory(this, CurrentState.CharacterAnimationManager).DamagedState();

        _currentState.EnterState();
    }
    public void ResetCharacter()
    {
        PlayerInputManager.DisableInputAction();

        _currentState = new CharacterStateFactory(this, CurrentState.CharacterAnimationManager).ResetState();

        _currentState.EnterState();

        if (CameraBehaviourController)
        {
            CameraBehaviourController.CinemachinePositionComposer.Damping = new Vector3(0.00f, 0.80f, 0.00f);
        }
    }
    public void DisableCharacterContext()
    {
        PlayerInputManager.DisableInputAction();

        _currentState = new CharacterStateFactory(this, CurrentState.CharacterAnimationManager).DisabledState();

        _currentState.EnterState();

        if (CameraBehaviourController)
        {
            CameraBehaviourController.CinemachinePositionComposer.Damping = new Vector3(0.00f, 0.80f, 0.00f);
        }
    }
    public void EnableCharacterContext()
    {
        PlayerInputManager.EnableInputAction();

        _currentState = new CharacterStateFactory(this, CurrentState.CharacterAnimationManager).GroundedState();

        _currentState.EnterState();

        if (CameraBehaviourController)
        {
            CameraBehaviourController.CinemachinePositionComposer.Damping = new Vector3(1.00f, 0.80f, 0.00f);
        }
    }
    #endregion

    #region PHYSICS FRAME
    void FixedUpdate()
    {
        _currentState.FixedUpdateStates();
    }
    #endregion

    #region PHYSICS COLLISION
    void OnCollisionEnter2D(Collision2D collision)
    {
        _currentState.OnCollisionEnter2D(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        _currentState.OnCollisionStay(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        _currentState.OnCollisionExit2D(collision);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        _currentState.OnTriggerEnter2D(collision);

        if (collision.TryGetComponent(out IInteractable interactable) && collision.CompareTag("Interactable"))
        {
            if (interactable.Interactions.Contains(EInteractionType.Enter))
            {
                interactable.SetInteraction(this, EInteractionType.Enter);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        _currentState.OnTriggerStay2D(collision);

        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if (interactable.Interactions.Contains(EInteractionType.Stay))
            {
                interactable.SetInteraction(this, EInteractionType.Stay);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        _currentState.OnTriggerExit2D(collision);

        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if (interactable.Interactions.Contains(EInteractionType.Exit))
            {
                interactable.SetInteraction(this, EInteractionType.Exit);
            }
        }
    }
    #endregion

    #region DELTA TIME
    void Update()
    {
        _currentState.UpdateStates();
    }
    void LateUpdate()
    {
        _currentState.LateUpdateStates();
    }
    #endregion

    #region RENDERING 
//    private void OnDrawGizmosSelected()
//    {
//#if UNITY_EDITOR
//        Gizmos.color = Color.red;
//        Gizmos.DrawWireCube(WallCheckerPoint.position, new Vector2(0.06f, 0.15f));
//        Gizmos.DrawWireCube(transform.position, new Vector2(0.40f, 0.04f));
//#endif
//    }
    //void OnGUI()
    //{
    //    GUILayout.Label("");
    //    GUILayout.Label("");
    //    GUILayout.Label("");
    //    GUILayout.Label("Dash On Cool DOwn: " + DashOnCoolDown);
    //    GUILayout.Label("Current State: " + CurrentState.ToString());
    //    GUILayout.Label("Current Sub State: " + (CurrentState.CurrentSubState != null ? CurrentState.CurrentSubState.ToString() : ""));
    //}
    #endregion

    #region DECOMMISSIONING
    void OnDisable()
    {
        
    }
    void OnDestroy()
    {
        OnResetState.RemoveAllListeners();
        OnPowerUpInteractableRecharge.RemoveAllListeners();
        OnAirJumpPowerStateChange.RemoveAllListeners();
        OnDashPowerStateChange.RemoveAllListeners();
        OnWallMovePowerStateChange.RemoveAllListeners();
    }
    #endregion
}
