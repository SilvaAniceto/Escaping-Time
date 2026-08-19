using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterAnimationManager))]
[RequireComponent(typeof(CharacterPowerUpManager))]
[RequireComponent(typeof(CharacterDamageManager))]
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
    [Header("Physics Settings")]
    [SerializeField] private CharacterPhysicsManager _physicsManager = new CharacterPhysicsManager();
    [Header("Power-Up Manager")]
    [SerializeField] private CharacterPowerUpManager _powerUpManager;
    [Header("Damage Controller")]
    [SerializeField] private CharacterDamageManager _damageManager;

    public CharacterPhysicsManager PhysicsManager => _physicsManager;

    private CharacterAbstractState _currentState;
    public CharacterAbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }

    public Transform CameraTarget { get => _cameraTarget; }
    public PlayerInputManager PlayerInputManager { get; private set; }
    public CharacterPowerUpManager PowerUpManager => _powerUpManager;
    public CharacterDamageManager DamageManager => _damageManager;
    public CameraBehaviourController CameraBehaviourController { get; private set; }
    public IInteractable Interactable { get; set; }

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
