using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : InteractableItem
{
    public enum MoveType
    {
        OneWay,
        BackAndForth,
        ConstantMove
    }

    #region INSPECTOR FIELDS
    [Header("Target Point")]
    [SerializeField] private Transform _targetPoint;

    [Header("Switch")]
    [SerializeField] private MovingPlatformSwitch _switch;

    [Header("Settings")]
    [SerializeField] private MoveType _moveType = MoveType.OneWay;
    [SerializeField, Range(0, 3f)] private float _movementSpeed;
    [SerializeField, Range(1, 3)] private int _tileCount = 1;
    #endregion

    #region PRIVATE FIELDS
    private GameContextManager _gameContextManager;

    private Vector3 _startPosition;
    private Vector3 _currentTargetPosition;
    private Vector3 _targetDirection;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    private bool _stopIsScheduled = false;
    #endregion

    #region PROPERTIES
    public MoveType MovingType { get => _moveType; }
    #endregion

    #region DEFAULT METHODS
    public override void Awake()
    {
        base.Awake();

        _gameContextManager = FindAnyObjectByType<GameContextManager>();

        _startPosition = transform.position;
        _targetPoint.GetComponent<SpriteRenderer>().enabled = false;

        _rigidbody = GetComponent<Rigidbody2D>();

        Interactions.Add(EInteractionType.Enter);
        Interactions.Add(EInteractionType.Exit);

        _currentTargetPosition = _targetPoint.position;
        _targetDirection = (_currentTargetPosition - transform.position).normalized;

        if (_switch)
        {
            _switch.MovingPlatform = this;
        }

        _targetPoint.SetParent(null);
    }
    private void Start()
    {
        _gameContextManager.CharacterContextManager.OnResetState.AddListener(ResetMovingPlatform);        
    }
    private void Update()
    {
        switch (_moveType)
        {
            case MoveType.OneWay:
                OneWayMove();
                break;
            case MoveType.BackAndForth:
                BackAndForthMove();
                break;
            case MoveType.ConstantMove:
                ConstantMove();
                break;
            default:
                break;
        }
    }
    private void FixedUpdate()
    {
        if (!Activated) return;

        _rigidbody.MovePosition(_rigidbody.position + (Vector2)_targetDirection * _movementSpeed * Time.fixedDeltaTime);
    }
    #endregion

    #region PLATFORM METHODS
    private void OneWayMove()
    {
        if (_startPosition == _currentTargetPosition && ReachCurrentTargetPosition())
        {
            _currentTargetPosition = _targetPoint.position;
            _targetDirection = (_currentTargetPosition - transform.position).normalized;

            _rigidbody.MovePosition(_startPosition);

            Activated = false;
        }
        else if (_targetPoint.position == _currentTargetPosition && ReachCurrentTargetPosition())
        {
            _currentTargetPosition = _startPosition;
            _targetDirection = (_currentTargetPosition - transform.position).normalized;

            _rigidbody.MovePosition(_targetPoint.position);

            Activated = false;
        }
    }
    private void BackAndForthMove()
    {
        if (_startPosition == _currentTargetPosition && ReachCurrentTargetPosition())
        {
            _currentTargetPosition = _targetPoint.position;
            _targetDirection = (_currentTargetPosition - transform.position).normalized;

            _rigidbody.MovePosition(_startPosition);

            Activated = false;
        }
        else if (_targetPoint.position == _currentTargetPosition && ReachCurrentTargetPosition())
        {
            _currentTargetPosition = _startPosition;
            _targetDirection = (_currentTargetPosition - transform.position).normalized;

            _rigidbody.MovePosition(_targetPoint.position);

            Activated = false;

            ScheduleStart();
        }
    }
    private void ConstantMove()
    {
        if (_stopIsScheduled)
        {
            return;
        }

        if (_startPosition == _currentTargetPosition && ReachCurrentTargetPosition())
        {
            _currentTargetPosition = _targetPoint.position;
            _targetDirection = (_currentTargetPosition - transform.position).normalized;

            _rigidbody.MovePosition(_startPosition);
        }
        else if (_targetPoint.position == _currentTargetPosition && ReachCurrentTargetPosition())
        {
            _currentTargetPosition = _startPosition;
            _targetDirection = (_currentTargetPosition - transform.position).normalized;

            _rigidbody.MovePosition(_targetPoint.position);
        }
    }
    private bool ReachCurrentTargetPosition()
    {
        float distanceFromTarget = Vector3.Distance(transform.position, _currentTargetPosition);
        distanceFromTarget = Mathf.Round(distanceFromTarget * 100.0f) / 100.0f;

        return distanceFromTarget < 0.05f;
    }
    public void ScheduleStart()
    {
        StartCoroutine(ScheduledStart());
    }
    IEnumerator ScheduledStart()
    {
        _gameContextManager.GameAudioManager.StopSFX(AudioSource);
        _gameContextManager.GameAudioManager.PlaySFX("Enter_Platform", AudioSource);

        yield return new WaitForSeconds(_gameContextManager.GameAudioManager.AudioClipLength("Enter_Platform"));

        Activated = true;
    }
    public void ScheduleStop()
    {
        StartCoroutine(ScheduledStop());
    }
    private IEnumerator ScheduledStop()
    {
        _stopIsScheduled = true;

        switch (_moveType)
        {
            case MoveType.BackAndForth:
                yield return new WaitUntil(() => _currentTargetPosition == _startPosition);

                yield return new WaitUntil(() => ReachCurrentTargetPosition());

                Activated = false;
                break;
            case MoveType.ConstantMove:
                yield return new WaitUntil(() => ReachCurrentTargetPosition());

                Activated = false;
                break;
        }

        _stopIsScheduled = false;
    }
    private void ResetMovingPlatform()
    {
        Activated = false;
        _rigidbody.MovePosition(_startPosition);
    }
    #endregion

    #region INTERACTION METHODS
    public override void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {
        switch (interactionType)
        {
            case EInteractionType.Enter:
                if (characterContextManager.transform.position.y > transform.position.y)
                {
                    if (Activated)
                    {
                        return;
                    }

                    ScheduleStart();
                }
                break;
            case EInteractionType.Stay:
                break;
            case EInteractionType.Exit:
                break;
            default:
                break;
        }
    }
    public override void ConfirmInteraction()
    {

    }
    public override void InteractablePauseState(bool value)
    {
        this.enabled = value;
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
