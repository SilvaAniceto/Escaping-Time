using UnityEngine;

public class PlayerContextManager : MonoBehaviour
{
    private CharacterAbstractState _currentState;
    private CharacterStateFactory _characterStateFactory;

    public CharacterAbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }
    private PlayerInputActions PlayerInputActions { get; set; }
    public Rigidbody2D Rigidbody { get ; private set; }
    public Quaternion TargetRotation
    {
        get
        {
            float angle = Mathf.Atan2(0, MoveInput) * Mathf.Rad2Deg;
            return Quaternion.AngleAxis(angle, Vector3.up);
        }
    }
    public float MoveInput { get => PlayerInputActions.PlayerActionMap.Move.ReadValue<float>() * 4.6f; }
    public bool JumpInput { get => PlayerInputActions.PlayerActionMap.Jump.WasPressedThisFrame(); }
    public float JumpSpeed { get => 290f; }
    public bool PerformingJump { get; set; }
    public float CoyoteTime { get; set; }

    #region INITIALIZATION
    void Awake()
    {
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

    void OnCollisionStay(Collision collision)
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
