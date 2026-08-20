using System.Collections;
using System.Linq;
using UnityEngine;

public class MovingPlatformBehavior : MonoBehaviour, IInteractableBehavior
{
    public enum MoveType
    {
        OneWay,
        BackAndForth,
        ConstantMove
    }

    [Header("Target Point")]
    [SerializeField] private Transform _targetPoint;

    [Header("Settings")]
    [SerializeField] private MoveType _moveType = MoveType.OneWay;
    [SerializeField, Range(0, 3f)] private float _movementSpeed;
    [SerializeField, Range(1, 3)] private int _tileCount = 1;

    private Vector3 _startPosition;
    private Vector3 _currentTargetPosition;
    private Vector3 _targetDirection;
    private Rigidbody2D _rigidbody;
    private bool _activated = false;
    private bool _stopIsScheduled = false;
    private SpriteRenderer _spriteRenderer;

    private Coroutine _scheduledStartCoroutine;
    private Coroutine _scheduledStopCoroutine;

    private WaitForSeconds WaitForActivate = new WaitForSeconds(0.2f);

    public bool Activated { get { return _activated; } set { _activated = value; } }
    public MoveType MovingType { get { return _moveType; } }

    private EInteractionType[] _interactionType = new[] { EInteractionType.Enter };
    public EInteractionType[] InteractionType { get => _interactionType; }

    private void Awake()
    {
        _startPosition = transform.position;
        _targetPoint.GetComponent<SpriteRenderer>().enabled = false;
        _rigidbody = GetComponent<Rigidbody2D>();

        _currentTargetPosition = _targetPoint.position;
        _targetDirection = (_currentTargetPosition - transform.position).normalized;

        _targetPoint.SetParent(null);
    }

    private void Start()
    {
        if (GameContextManager.Instance != null && GameContextManager.Instance.CharacterContextManager != null)
        {
            GameContextManager.Instance.CharacterContextManager.DamageManager.OnResetState.AddListener(ResetMovingPlatform);
        }
    }

    private void Update()
    {
        if (!_activated)
        {
            return;
        }

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
        }
    }

    private void FixedUpdate()
    {
        if (!_activated)
        {
            return;
        }

        _rigidbody.MovePosition(_rigidbody.position + (Vector2)_targetDirection * _movementSpeed * Time.fixedDeltaTime);
    }

    private void OnDestroy()
    {
        if (_scheduledStartCoroutine != null)
        {
            StopCoroutine(_scheduledStartCoroutine);
        }

        if (_scheduledStopCoroutine != null)
        {
            StopCoroutine(_scheduledStopCoroutine);
        }
    }

    private void OneWayMove()
    {
        if (_startPosition == _currentTargetPosition && ReachCurrentTargetPosition())
        {
            _currentTargetPosition = _targetPoint.position;
            _targetDirection = (_currentTargetPosition - transform.position).normalized;

            _rigidbody.MovePosition(_startPosition);

            _activated = false;
        }
        else if (_targetPoint.position == _currentTargetPosition && ReachCurrentTargetPosition())
        {
            _currentTargetPosition = _startPosition;
            _targetDirection = (_currentTargetPosition - transform.position).normalized;

            _rigidbody.MovePosition(_targetPoint.position);

            _activated = false;
        }
    }

    private void BackAndForthMove()
    {
        if (_startPosition == _currentTargetPosition && ReachCurrentTargetPosition())
        {
            _currentTargetPosition = _targetPoint.position;
            _targetDirection = (_currentTargetPosition - transform.position).normalized;

            _rigidbody.MovePosition(_startPosition);

            _activated = false;
        }
        else if (_targetPoint.position == _currentTargetPosition && ReachCurrentTargetPosition())
        {
            _currentTargetPosition = _startPosition;
            _targetDirection = (_currentTargetPosition - transform.position).normalized;

            _rigidbody.MovePosition(_targetPoint.position);

            _activated = false;

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

    public bool ReachTargetPoint()
    {
        float distanceFromTarget = Vector3.Distance(transform.position, _targetPoint.position);
        distanceFromTarget = Mathf.Round(distanceFromTarget * 100.0f) / 100.0f;

        return distanceFromTarget < 0.05f;
    }

    public void ScheduleStart()
    {
        _scheduledStartCoroutine = StartCoroutine(ScheduledStart());
    }

    private IEnumerator ScheduledStart()
    {
        if (GameContextManager.Instance != null)
        {
            GameContextManager.Instance.AudioManager.StopSFX();
            GameContextManager.Instance.AudioManager.PlaySFX("Enter_Platform");
        }

        yield return WaitForActivate;

        _activated = true;
    }

    public void ScheduleStop()
    {
        _scheduledStopCoroutine = StartCoroutine(ScheduledStop());
    }

    private IEnumerator ScheduledStop()
    {
        _stopIsScheduled = true;

        switch (_moveType)
        {
            case MoveType.BackAndForth:
                yield return new WaitUntil(() => _currentTargetPosition == _startPosition);
                yield return new WaitUntil(() => ReachCurrentTargetPosition());
                _activated = false;
                break;
            case MoveType.ConstantMove:
                yield return new WaitUntil(() => ReachCurrentTargetPosition());
                _activated = false;
                break;
        }

        _stopIsScheduled = false;
    }

    private void ResetMovingPlatform()
    {
        _activated = false;
        _rigidbody.MovePosition(_startPosition);

        if (_scheduledStartCoroutine != null)
        {
            StopCoroutine(_scheduledStartCoroutine);
            _scheduledStartCoroutine = null;
        }

        if (_scheduledStopCoroutine != null)
        {
            StopCoroutine(_scheduledStopCoroutine);
            _scheduledStopCoroutine = null;
        }
    }

    public void Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        if (!_interactionType.Contains(interactionType))
        {
            return;
        }

        if (context.transform.position.y > transform.position.y)
        {
            if (_activated)
            {
                return;
            }

            ScheduleStart();
        }
    }

    [ContextMenu("SetTile")]
    public void SetTile()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }

        _spriteRenderer.size = new Vector2(_tileCount, 1);
    }
}