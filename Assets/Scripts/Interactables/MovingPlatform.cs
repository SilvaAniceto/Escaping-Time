using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour, IInteractable
{
    #region INSPECTOR FIELDS
    [SerializeField] private Transform _targetPoint_A;
    [SerializeField] private MovingPlatformSwitch _switchPoint_A;
    [SerializeField] private Transform _targetPoint_B;
    [SerializeField] private MovingPlatformSwitch _switchPoint_B;
    [SerializeField, Range(0, 3f)] private float _movementSpeed;
    [SerializeField, Range(1, 3)] private int _tileCount = 1;
    #endregion

    #region PRIVATE FIELDS
    private Vector3 _targetDirection;
    private Vector3 _currentTargetPosition;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    #endregion

    #region PROPERTIES
    public List<EInteractionType> Interactions { get; set; } = new List<EInteractionType>();
    public bool Activated { get; set; } = false;
    public float MovementSpeedMultiplier { get; set; }
    public GameObject CurrentSwitch { get; set; }
    #endregion

    #region DEFAULT METHODS
    private void Awake()
    {
        _targetPoint_A.GetComponent<SpriteRenderer>().enabled = false;
        _targetPoint_B.GetComponent<SpriteRenderer>().enabled = false;

        _rigidbody = GetComponent<Rigidbody2D>();

        Interactions.Add(EInteractionType.Enter);
        Interactions.Add(EInteractionType.Exit);

        _currentTargetPosition = _targetPoint_B.position;
        _targetDirection = (_currentTargetPosition - transform.position).normalized;

        _targetPoint_A.SetParent(null);
        _targetPoint_B.SetParent(null);
        _switchPoint_A.transform.SetParent(null);
        _switchPoint_B.transform.SetParent(null);

        MovementSpeedMultiplier = 1;

        GameManagerContext.OnRunOrPauseStateChanged.AddListener(InteractablePauseState);
    }
    private void Update()
    {
        CheckChangeCurrentTarget(); 
    }
    private void FixedUpdate()
    {
        if (!Activated) return;

        _rigidbody.MovePosition(_rigidbody.position + (Vector2)_targetDirection * _movementSpeed * MovementSpeedMultiplier * Time.fixedDeltaTime);
    }
    #endregion

    #region PLATFORM METHODS
    private void CheckChangeCurrentTarget()
    {
        if (_targetPoint_A.position == _currentTargetPosition && ReachCurrentTargetPosition())
        {
            CurrentSwitch = _switchPoint_A.gameObject;
            _currentTargetPosition = _targetPoint_B.position;
            _targetDirection = (_currentTargetPosition - transform.position).normalized;

            _rigidbody.MovePosition(_targetPoint_A.position);

            _switchPoint_A.Activated = true;
            _switchPoint_B.Activated = true;

            Activated = false;

            MovementSpeedMultiplier = 1;
        }
        else if (_targetPoint_B.position == _currentTargetPosition && ReachCurrentTargetPosition())
        {
            CurrentSwitch = _switchPoint_B.gameObject;
            _currentTargetPosition = _targetPoint_A.position;
            _targetDirection = (_currentTargetPosition - transform.position).normalized;

            _rigidbody.MovePosition(_targetPoint_B.position);

            _switchPoint_A.Activated = true;
            _switchPoint_B.Activated = true;

            Activated = false;

            MovementSpeedMultiplier = 1;
        }
    }
    private bool ReachCurrentTargetPosition()
    {
        float distanceFromTarget = Vector3.Distance(transform.position, _currentTargetPosition);
        distanceFromTarget = Mathf.Round(distanceFromTarget * 100.0f) / 100.0f;

        return distanceFromTarget < 0.05f;
    }
    #endregion

    #region INTERACTION METHODS
    public void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {
        if (characterContextManager.transform.position.y > transform.position.y)
        {
            switch (interactionType)
            {
                case EInteractionType.Enter:
                    characterContextManager.FixedJointConnectedBody = _rigidbody;

                    if (Activated)
                    {
                        characterContextManager.AddFixedJoint2D();
                        return;
                    }

                    InteractionDelayed(characterContextManager);
                    break;
                case EInteractionType.Stay:
                    break;
                case EInteractionType.Exit:
                    characterContextManager.FixedJointConnectedBody = null;

                    characterContextManager.RemoveFixedJoint2D();
                    break;
                default:
                    break;
            }
        }
    }
    public void ConfirmInteraction()
    {

    }
    public void InteractablePauseState(bool value)
    {
        this.enabled = value;
    }
    private async void InteractionDelayed(CharacterContextManager characterContextManager)
    {
        await Task.Delay(360);

        Activated = true;

        _switchPoint_A.Activated = false;
        _switchPoint_B.Activated = false;
    }
    #endregion

    #region EDITOR METHODS
    [ContextMenu("SetTile")]
    public void SetTile()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        _spriteRenderer.size = new Vector2(_tileCount, 1);
    }
    #endregion
}
