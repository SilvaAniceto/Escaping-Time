using UnityEngine;

public class PlayerContextManager : MonoBehaviour
{
    private CharacterAbstractState _currentState;
    private CharacterStateFactory _characterStateFactory;

    public CharacterAbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }
    private PlayerInputActions PlayerInputActions { get; set; }
    public float MoveInput { get; private set; }

    void Awake()
    {
        _characterStateFactory = new CharacterStateFactory(this);
        _currentState = _characterStateFactory.GroundedState();
    }

    void OnEnable()
    {

    }

    void Start()
    {
        PlayerInputActions = new PlayerInputActions();
        PlayerInputActions.Enable();

        _currentState.EnterState();
    }

    void FixedUpdate()
    {
        _currentState.FixedUpdateState();
    }

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
    void ReadPlayerActions()
    {
        MoveInput = PlayerInputActions.PlayerActionMap.Move.ReadValue<float>();
    }
    void Update()
    {
        ReadPlayerActions();
        _currentState.UpdateStates();
    }

    void LateUpdate()
    {
        _currentState.LateUpdateState();
    }

    void OnGUI()
    {
        
    }

    void OnDisable()
    {

    }

    void OnDestroy()
    {

    }
}
