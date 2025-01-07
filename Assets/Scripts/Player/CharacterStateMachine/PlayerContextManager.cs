using UnityEngine;

public class PlayerContextManager : MonoBehaviour
{
    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private LayerMask _interactionLayer;
    [SerializeField] private BoxCollider2D _characterCollider;
    [SerializeField] private BoxCollider2D _groundChecker;

    private CharacterAbstractState _currentState;
    private CharacterStateFactory _characterStateFactory;
    private Animator _characterAnimator;

    private PlayerInputActions PlayerInputActions { get; set; }
    public CharacterAbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Animator CharacterAnimator { get => _characterAnimator; }
    public Rigidbody2D Rigidbody { get; private set; }
    public BoxCollider2D CharacterCollider { get => _characterCollider; }
    public BoxCollider2D GroundChecker { get => _groundChecker; }
    public float MoveInput { get => PlayerInputActions.PlayerActionMap.Move.ReadValue<float>(); }
    public bool IsWallColliding { get => Physics2D.Raycast(CharacterAnimator.transform.position + (Vector3.up * 0.02f), CharacterAnimator.transform.right, 0.2f, _wallLayer); }
    public bool JumpInput { get => PlayerInputActions.PlayerActionMap.Jump.WasPressedThisFrame(); }
    public bool PerformingJump { get; set; }
    public bool Falling { get; set; }
    public float VerticalVelocity { get => Mathf.Round(Rigidbody.linearVelocity.y * 100) / 100; }
    public bool Damaged { get; set; }
    public Vector3 HitDirection { get; set; }
    public bool InteractionInput { get => PlayerInputActions.PlayerActionMap.Interact.WasPressedThisFrame(); }
    public int KeyItem { get; set; }
    public bool WaitingInteraction { get; set; }
    public bool SpawningCharacter { get; set; }
    public Vector3 SpawningPosition { get; set; }

    public const string IDLE_ANIMATION = "Idle";
    public const string RUN_ANIMATION = "Run";
    public const string JUMP_ANIMATION = "Jump";
    public const string FALL_ANIMATION = "Fall";
    public const string HIT_ANIMATION = "Hit";
    public const string SPAWNING_ANIMATION = "Spawning";
    public const string DISABLED_ANIMATION = "Disabled";

    #region INITIALIZATION
    void Awake()
    {
        _characterAnimator = GetComponentInChildren<Animator>();
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

        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if (interactable.Interactions.Contains(EInteractionType.TriggerEnter))
            {
                interactable.SetInteraction(gameObject, EInteractionType.TriggerEnter);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        _currentState.OnTriggerStay2D(collision);

        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if (interactable.Interactions.Contains(EInteractionType.TriggerStay))
            {
                interactable.SetInteraction(gameObject, EInteractionType.TriggerStay);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        _currentState.OnTriggerExit2D(collision);

        if (collision.TryGetComponent(out IInteractable interactable))
        {
            if (interactable.Interactions.Contains(EInteractionType.TriggerExit))
            {
                interactable.SetInteraction(gameObject, EInteractionType.TriggerExit);
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
    private void OnDrawGizmos()
    {
        
    }
    void OnGUI()
    {
        //GUI.Box(new Rect(transform.position.x, transform.position.y + 3, 300, 80), "");
        //GUILayout.Label("CURRENT STATE: " + CurrentState.ToString());
        //GUILayout.Label("CURRENT SUB STATE: " + (CurrentState._currentSubState != null ? CurrentState._currentSubState.ToString() : ""));
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
