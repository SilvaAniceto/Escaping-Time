using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour, IInteractable
{
    [SerializeField, Range(1, 3)] private int _tileCount = 1;
    [SerializeField, Range(0, 3.14f)] private float _speed;
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;

    private Vector3 _target;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;

    public List<EInteractionType> Interactions { get; set; } = new List<EInteractionType>();
    public bool Activated { get; set; } = false;
    public bool MovableObject { get; set; }

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        Interactions.Add(EInteractionType.Stay);

        _target = (_pointA.position - transform.position).normalized;

        _pointA.SetParent(null);
        _pointB.SetParent(null);
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position,_pointA.position) <= 0.10f)
        {
            _target = (_pointB.position - transform.position).normalized;
        }
        else if (Vector3.Distance(transform.position, _pointB.position) <= 0.10f)
        {
            _target = (_pointA.position - transform.position).normalized;
        }

        _rigidbody.MovePosition(_rigidbody.position + (Vector2)_target * _speed * Time.fixedDeltaTime);
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

    public void SetInteraction(CharacterContextManager characterContextManager)
    {
        if (characterContextManager.CurrentState.CurrentSubState == characterContextManager.CurrentState.CharacterStateFactory.IdleState())
        {
            characterContextManager.AddFixedJoint2D(_rigidbody);
        }
    }
    public void ConfirmInteraction()
    {

    }
}
