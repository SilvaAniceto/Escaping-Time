using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(PlayerInputManager), typeof(CharacterAnimationManager))]
public class CharacterContextManager : MonoBehaviour
{
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
    
    private CharacterAbstractState _exitState;
    private CharacterAbstractState _currentState;

    public CharacterAbstractState ExitState { get { return _exitState; } set { _exitState = value; } }
    public CharacterAbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }

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
            DashSpeedOvertime += Time.deltaTime / 0.64f;

            return _dashCurve.Evaluate(Mathf.Clamp01(DashSpeedOvertime));
        }
    }
    public float DashCoolDownTime { get; set; }
    public bool DashIsWaitingGroundedState { get; set; }
    public bool DashIsAllowed
    {
        get
        {
            return DashCoolDownTime <= 0 && !DashIsWaitingGroundedState;
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
    public FixedJoint2D FixedJoint2D { get; set; }
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

        GameManagerContext.OnRunOrPauseStateChanged.AddListener((value) => this.enabled = value);
    }
    void OnEnable()
    {
        CurrentState.PlayerInputManager.enabled = true;
        CurrentState.CharacterAnimationManager.CharacterAnimator.enabled = true;
    }
    void Start()
    {
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
        CoyoteTime = 0.20f;
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
    public void AddFixedJoint2D(Rigidbody2D connectedBody)
    {
        if (FixedJoint2D)
        {
            return;
        }

        FixedJoint2D = gameObject.AddComponent<FixedJoint2D>();
        FixedJoint2D.connectedBody = connectedBody;
        FixedJoint2D.enableCollision = true;
    }
    public void RemoveFixedJoint2D()
    {
        if (FixedJoint2D)
        {
            Destroy(FixedJoint2D);
        }
    }
    public void ApplyDamage(float damageDirection)
    {
        DamageHitDirection = damageDirection;
        TakingDamage = true;
    }
    public void SetDamageExitWaitTime()
    {
        DamageExitWaitTime -= Time.deltaTime;

        DamageExitWaitTime = Mathf.Clamp01(DamageExitWaitTime);
    }
    public void ResetDamageExitWaitTime()
    {
        DamageExitWaitTime = 0.12f;
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
                interactable.SetInteraction(this);
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
                interactable.SetInteraction(this);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        _currentState.OnTriggerExit2D(collision);
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
    }
    void OnGUI()
    {
#if UNITY_EDITOR
        //GUILayout.Label("Exit State: " + (ExitState == null ? "" : ExitState.ToString()));
        //GUILayout.Label("Current State: " + CurrentState.ToString());
        //GUILayout.Label("Current Super State: " + (CurrentState.CurrentSuperState != null ? CurrentState.CurrentSuperState.ToString() : ""));
        //GUILayout.Label("Current Sub State: " + (CurrentState.CurrentSubState != null ? CurrentState.CurrentSubState.ToString() : ""));

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
        CurrentState.PlayerInputManager.enabled = false;
        CurrentState.CharacterAnimationManager.CharacterAnimator.enabled = false;
    }

    void OnDestroy()
    {

    }
    #endregion
}
