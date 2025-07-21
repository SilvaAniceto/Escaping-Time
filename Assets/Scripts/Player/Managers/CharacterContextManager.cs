using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(PlayerInputManager), typeof(CharacterAnimationManager))][System.Serializable]
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
    [Header("Graphics")]
    [SerializeField] private GameObject _characterGraphic;

    private CharacterAbstractState _exitState;
    private CharacterAbstractState _currentState;

    public Transform CameraTarget { get => _cameraTarget; }
    public CharacterAbstractState ExitState { get { return _exitState; } set { _exitState = value; } }
    public CharacterAbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public bool EnableCharacterGraphics { set => _characterGraphic.SetActive(value); }

    #region POWER UP
    public bool HasDash
    {
        get
        {
            return HasTemporaryDash || HasInfinityDash ? true : false;
        }
    }
    public bool HasAirJump
    {
        get
        {
            return HasTemporaryAirJump || HasInfinityAirJump ? true : false;
        }
    }
    public bool HasWallMove
    {
        get
        {
            return HasInfinityWallMove || HasTemporaryWallMove ? true : false;
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
    public float TemporaryWallMoveTime { get; set; }

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
    #endregion

    #region POWER UP CALLBACKS
    [HideInInspector] public UnityEvent OnPowerUpInteractableRecharge = new UnityEvent();
    [HideInInspector] public UnityEvent<string> OnAirJumpPowerStateChange = new UnityEvent<string>();
    [HideInInspector] public UnityEvent<string> OnDashPowerStateChange = new UnityEvent<string>();
    [HideInInspector] public UnityEvent<string> OnWallMovePowerStateChange = new UnityEvent<string>();

    private void SetPowerUpCallBack()
    {
        if (GameManagerContext.Instance.CharacterUI != null)
        {
            OnAirJumpPowerStateChange.AddListener(GameManagerContext.Instance.CharacterUI.SetAirJumpPowerUpUI);
            OnDashPowerStateChange.AddListener(GameManagerContext.Instance.CharacterUI.SetDashPowerUpUI);
            OnWallMovePowerStateChange.AddListener(GameManagerContext.Instance.CharacterUI.SetWallMovePowerUpUI);
        }
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
    public float HorizontalStartSpeed { get; set; }
    public float HorizontalTopSpeed { get; set; }
    public float HorizontalSpeedOvertime { get; set; }
    public float HorizontalSpeedLerpOvertime
    {
        get
        {
            HorizontalSpeedOvertime += Time.deltaTime / 0.62f;

            return _accelerationCurve.Evaluate(Mathf.Clamp01(HorizontalSpeedOvertime));
        }
    }
    public float CoyoteTime { get; set; }
    public bool AirJumpIsAllowed { get; set; }
    public float GravityUpwardSpeedOvertime { get; set; }
    public float GravityUpwardSpeedLerpOvertime
    {
        get
        {
            GravityUpwardSpeedOvertime += Time.deltaTime / 0.36f;

            return _jumpForceCurve.Evaluate(Mathf.Clamp01(GravityUpwardSpeedOvertime));
        }
    }
    public float GravityDownwardSpeedOvertime { get; set; }
    public float GravityDownwardSpeedLerpOvertime
    {
        get
        {
            GravityDownwardSpeedOvertime += Time.deltaTime / 0.48f;

            return _fallCurve.Evaluate(Mathf.Clamp01(GravityDownwardSpeedOvertime));
        }
    }
    public float DashSpeedOvertime { get; set; }
    public float DashSpeedLerpOvertime
    {
        get
        {
            DashSpeedOvertime += Time.deltaTime / 0.62f;

            return _dashCurve.Evaluate(Mathf.Clamp01(DashSpeedOvertime));
        }
    }
    public float DashCoolDownTime { get; set; }
    public bool DashIsWaitingGroundedState { get; set; }
    public bool DashIsAllowed
    {
        get
        {
            return HasDash ? DashCoolDownTime <= 0 && !DashIsWaitingGroundedState : false;
        }
    }
    public float DamageSpeedOvertime { get; set; }
    public float DamageSpeedLerpOvertime
    {
        get
        {
            DamageSpeedOvertime += Time.deltaTime / 0.62f;

            return _damageCurve.Evaluate(Mathf.Clamp01(DamageSpeedOvertime));
        }
    }
    public float DamageExitWaitTime { get; private set; }
    #endregion

    #region PHYSICS DETECTION PROPERTIES
    public Rigidbody2D FixedJointConnectedBody { get; set;}
    public Joint2D FixedJoint2D { get; set; }
    #endregion

    #region DAMAGE PROPERTIES
    public bool TakingDamage { get; set; }
    public float DamageHitDirection { get; private set; }
    public bool SpawningCharacter { get; set; }
    public Vector3 SpawningPosition { get; set; }
    #endregion

    #region INITIALIZATION
    void Awake()
    {
        _currentState = new CharacterStateFactory(this, GetComponent<PlayerInputManager>(), GetComponent<CharacterAnimationManager>()).GroundedState();

        CurrentState.CharacterAnimationManager.CharacterAnimator = CurrentState.CharacterAnimationManager.GetComponentInChildren<Animator>();

        Rigidbody = GetComponent<Rigidbody2D>();
        FixedJoint2D = GetComponent<FixedJoint2D>();

        RemoveFixedJoint2D();

        GameManagerContext.OnRunOrPauseStateChanged.AddListener((value) => this.enabled = value);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Ceiling"), LayerMask.NameToLayer("Default"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Ground"), LayerMask.NameToLayer("Default"));
    }
    void OnEnable()
    {
        CurrentState.CharacterAnimationManager.CharacterAnimator.enabled = true;
    }
    void Start()
    {
        SetPowerUpCallBack();

        _currentState.EnterState();
    }
    #endregion

    #region CHARACTER CONTEXT
    public void SetCoyoteTime()
    {
        CoyoteTime -= Time.deltaTime;

        CoyoteTime = Mathf.Clamp01(CoyoteTime);
    }
    public void ResetCoyoteTime()
    {
        CoyoteTime = 0.084f;
    }
    public void SetDashCoolDownTime()
    {
        DashCoolDownTime -= Time.deltaTime;

        DashCoolDownTime = Mathf.Clamp01(DashCoolDownTime);
    }
    public void ResetDashCoolDownTime(float value)
    {
        DashIsWaitingGroundedState = false;
        DashCoolDownTime = value;
    }
    public void SetTemporaryWallMoveTime()
    {
        if (TemporaryWallMoveTime == 0.00f)
        {
            GameManagerContext.Instance.CharacterUI.SetOvertimeWallMovePowerUpUI(1.00f);
            return;
        }

        TemporaryWallMoveTime -= Time.deltaTime;

        TemporaryWallMoveTime = Mathf.Clamp(TemporaryWallMoveTime, 0.00f, 6.00f);

        GameManagerContext.Instance.CharacterUI.SetOvertimeWallMovePowerUpUI(Mathf.InverseLerp(0.00f, 6.00f, TemporaryWallMoveTime));

        if (HasTemporaryWallMove && TemporaryWallMoveTime == 0.00f)
        {
            ResetTemporaryWallMoveTime(0.00f);
        }
    }
    public void ResetTemporaryWallMoveTime(float value)
    {
        if (HasTemporaryWallMove && value == 0.00f)
        {
            HasTemporaryWallMove = false;
            DispatchPowerUpInteractableRecharge();
        }

        TemporaryWallMoveTime = value;
    }
    public void AddFixedJoint2D()
    {
        if (FixedJoint2D.enabled || !FixedJointConnectedBody || CurrentState.CurrentSubState != CharacterStateFactory.Instance.IdleState() || CurrentState != CharacterStateFactory.Instance.GroundedState())
        {
            return;
        }

        FixedJoint2D.connectedBody = FixedJointConnectedBody;
        FixedJoint2D.enableCollision = true;
        FixedJoint2D.enabled = true;
    }
    public void RemoveFixedJoint2D()
    {
        FixedJoint2D.enabled = false;
        FixedJoint2D.connectedBody = null;
    }
    public void ApplyDamage(float damageDirection)
    {
        DamageHitDirection = damageDirection;
        TakingDamage = true;
    }
    public void SetDamageExitWaitTime()
    {
        DamageExitWaitTime -= Time.deltaTime;

        DamageExitWaitTime = Mathf.Clamp(DamageExitWaitTime, 0.00f, 3.00f);
    }
    public void ResetDamageExitWaitTime()
    {
        DamageExitWaitTime = 2.52f;
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(WallCheckerPoint.position, new Vector2(0.06f, 0.15f));
        Gizmos.DrawWireCube(transform.position, new Vector2(0.40f, 0.04f));
    }
    void OnGUI()
    {
#if UNITY_EDITOR
        GUILayout.Label("Exit State: " + (ExitState == null ? "" : ExitState.ToString()));
        GUILayout.Label("Current State: " + CurrentState.ToString());
        //GUILayout.Label("Current Super State: " + (CurrentState.CurrentSuperState != null ? CurrentState.CurrentSuperState.ToString() : ""));
        GUILayout.Label("Current Sub State: " + (CurrentState.CurrentSubState != null ? CurrentState.CurrentSubState.ToString() : ""));

        //GUILayout.Label("Dash Cool Down Time: " + DashCoolDownTime.ToString());
        //GUILayout.Label("Coyote Time: " + CoyoteTime.ToString());
        //GUILayout.Label("Character Forward Direction: " + CurrentState.CharacterForwardDirection.ToString());
        //GUILayout.Label("Horizontal Speed: " + HorizontalSpeed.ToString());
        //GUILayout.Label("Vertical Speed: " + VerticalSpeed.ToString());
#endif

    }
    #endregion

    #region DECOMMISSIONING
    void OnDisable()
    {
        CurrentState.CharacterAnimationManager.CharacterAnimator.enabled = false;
    }

    void OnDestroy()
    {

    }
    #endregion
}
