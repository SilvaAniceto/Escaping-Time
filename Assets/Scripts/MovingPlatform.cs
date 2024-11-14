using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, IInteractable
{
    [SerializeField, Range(1, 3)] private int _tileCount = 1;
    [SerializeField, Range(1, 3.14f)] private float _speed;
    [SerializeField] private Transform _pointA;
    [SerializeField] private Transform _pointB;

    private Vector3 _target;
    private Vector3 _pointAPosition;
    private Vector3 _pointBPosition;
    private SpriteRenderer _spriteRenderer;

    public List<EInteractionType> Interactions { get; set; } = new List<EInteractionType>();
    public bool Activated { get; set; } = false;

    void Awake()
    {
        Interactions.Add(EInteractionType.Stay);
        Interactions.Add(EInteractionType.Exit);

        _pointAPosition = _pointA.position;
        _pointBPosition = _pointB.position;

        _target = _pointAPosition;

        _pointA.gameObject.SetActive(false);
        _pointB.gameObject.SetActive(false);
    }

    void Update()
    {
        if (transform.position == _pointAPosition)
        {
            _target = _pointBPosition;
        }
        else if (transform.position == _pointBPosition)
        {
            _target = _pointAPosition;
        }

        transform.position = Vector3.MoveTowards(transform.position, _target, _speed * Time.deltaTime);
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

    public void SetInteraction(GameObject p_gameObject, EInteractionType p_interactionType)
    {
        Transform transform = p_gameObject.transform;

        if (transform.parent != gameObject.transform && p_interactionType == EInteractionType.Stay)
        {
            Activated = true;
            transform.SetParent(gameObject.transform);
        }
        else if (transform.parent == gameObject.transform && p_interactionType == EInteractionType.Exit)
        {
            Activated = false;
            transform.SetParent(null);
        }
    }
}
