using UnityEngine;

public class PlayerContextManager : MonoBehaviour
{
    [SerializeField] private LayerMask _wallLayer;

    private CharacterAbstractState _currentState;
    private CharacterStateFactory _characterStateFactory;
    private Animator _characterAnimator;

    private PlayerInputActions PlayerInputActions { get; set; }
    public CharacterAbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Animator CharacterAnimator { get => _characterAnimator; }
    public Rigidbody2D Rigidbody { get; private set; } 
    public float MoveInput { get => PlayerInputActions.PlayerActionMap.Move.ReadValue<float>(); }
    public bool IsWallColliding { get => Physics2D.Raycast(transform.position, transform.right, 0.3f, _wallLayer); }
    public bool JumpInput { get => PlayerInputActions.PlayerActionMap.Jump.WasPressedThisFrame(); }
    public bool PerformingJump { get; set; }
    public bool Falling { get; set; }

    public const string IDLE_ANIMATION = "Idle";
    public const string RUN_ANIMATION = "Run";
    public const string JUMP_ANIMATION = "Jump";
    public const string FALL_ANIMATION = "Fall";
    public const string HIT_ANIMATION = "Hit";

    #region INITIALIZATION
    void Awake()
    {
        _characterAnimator = GetComponent<Animator>();
        _characterStateFactory = new CharacterStateFactory(this);
        _currentState = _characterStateFactory.GroundedState();
    }

    void OnEnable()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        PlayerInputActions = new PlayerInputActions();
        PlayerInputActions.Enable();

        _currentState.EnterState();
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
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        _currentState.OnTriggerStay2D(collision);
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
        _currentState.LateUpdateState();
    }
    #endregion

    #region RENDERING 
    void OnGUI()
    {

    }
    #endregion

    #region DECOMMISSIONING
    void OnDisable()
    {

    }

    void OnDestroy()
    {

    }
    #endregion
}
