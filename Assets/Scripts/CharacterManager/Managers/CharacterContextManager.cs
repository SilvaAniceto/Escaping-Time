using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(CharacterAnimationManager))]
[RequireComponent(typeof(CharacterPowerUpManager))]
[RequireComponent(typeof(CharacterDamageManager))]
public class CharacterContextManager : MonoBehaviour, IStateController, IMovementDirection, IJumpCapability, IDashCapability, IWallMoveCapability, IDamageCapability
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
    [Header("Physics Settings")]
    [SerializeField] private CharacterPhysicsManager _physicsManager = new CharacterPhysicsManager();
    [Header("Power-Up Manager")]
    [SerializeField] private CharacterPowerUpManager _powerUpManager;
    [Header("Damage Controller")]
    [SerializeField] private CharacterDamageManager _damageManager;

    public CharacterPhysicsManager PhysicsManager => _physicsManager;

    private CharacterAbstractState _currentState;
    private CharacterPhysicsHandler _physicsHandler;
    private CharacterCollisionDetector _collisionDetector;

    public CharacterAbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public CharacterCollisionDetector CollisionDetector { get { return _collisionDetector; } }

    public Transform CameraTarget { get => _cameraTarget; }
    public PlayerInputManager PlayerInputManager { get; private set; }
    public CharacterPowerUpManager PowerUpManager => _powerUpManager;
    public CharacterDamageManager DamageManager => _damageManager;
    public CameraBehaviourController CameraBehaviourController { get; private set; }
    public GameObject InteractableGameObject { get; set; }

    #region INTERFACE IMPLEMENTATIONS
    public int MoveDirection
    {
        get { return _physicsManager.MoveDirection; }
        set { _physicsManager.MoveDirection = value; }
    }

    public bool CoyoteTime
    {
        get { return _physicsManager.CoyoteTime; }
        set { _physicsManager.CoyoteTime = value; }
    }

    public bool HasAirJump
    {
        get { return _powerUpManager.HasAirJump; }
    }

    public bool AirJumpIsAllowed
    {
        get { return _powerUpManager.AirJumpIsAllowed; }
    }

    public void EnableAirJump()
    {
        _powerUpManager.EnableAirJump();
    }

    public void DisableAirJump()
    {
        _powerUpManager.DisableAirJump();
    }

    public bool DashIsAllowed
    {
        get { return _powerUpManager.DashIsAllowed; }
    }

    public bool HasWallMove
    {
        get { return _powerUpManager.HasWallMove; }
    }

    public bool IsInvincible
    {
        get { return _damageManager.IsInvincible; }
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

    #region PHYSICS DETECTION PROPERTIES
    public Rigidbody2D FixedJointConnectedBody { get; set;}
    public Joint2D FixedJoint2D { get; set; }
    #endregion

    #region INITIALIZATION
    public void InitializeCharacterContextManager(PlayerInputManager playerInputManager, CameraBehaviourController cameraBehaviourController, bool isGameContext = true)
    {
        PlayerInputManager = playerInputManager;
        CameraBehaviourController = cameraBehaviourController;

        Rigidbody = GetComponent<Rigidbody2D>();
        FixedJoint2D = GetComponent<FixedJoint2D>();

        _collisionDetector = new CharacterCollisionDetector(transform, _wallCheckerPoint, _groundLayerTarget, _wallLayerTarget);
        _physicsHandler = new CharacterPhysicsHandler(this, FixedJoint2D);

        DisableFixedJoint2D();

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Ceiling"), LayerMask.NameToLayer("Default"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Ground"), LayerMask.NameToLayer("Default"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("WallChecker"), LayerMask.NameToLayer("Default"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("WallChecker"), LayerMask.NameToLayer("Camera Objects"));

        _physicsManager.Initialize();

        _powerUpManager.RegisterDashCallback();
        _powerUpManager.RegisterAirJumpCallback();
        _powerUpManager.RegisterWallMoveCallback();

        _damageManager.Initialize(this);

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
        _physicsHandler.EnableFixedJoint2D();
    }
    public void DisableFixedJoint2D()
    {
        _physicsHandler.DisableFixedJoint2D();
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
        _collisionDetector.UpdateCollisions();
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

        if (collision.TryGetComponent(out IInteractableBehavior newInteractable))
        {
            if (newInteractable.InteractionType.Contains(EInteractionType.Enter))
            {
                InteractableGameObject = collision.gameObject;
                newInteractable.Execute(this, EInteractionType.Enter);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        _currentState.OnTriggerStay2D(collision);

        if (collision.TryGetComponent(out IInteractableBehavior newInteractable))
        {
            if (newInteractable.InteractionType.Contains(EInteractionType.Stay))
            {
                InteractableGameObject = collision.gameObject;
                newInteractable.Execute(this, EInteractionType.Stay);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        _currentState.OnTriggerExit2D(collision);

        if (collision.TryGetComponent(out IInteractableBehavior newInteractable))
        {
            if (newInteractable.InteractionType.Contains(EInteractionType.Exit))
            {
                InteractableGameObject = null;
                newInteractable.Execute(this, EInteractionType.Exit);
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
    //    GUILayout.Label("");
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

    }
    #endregion
}
