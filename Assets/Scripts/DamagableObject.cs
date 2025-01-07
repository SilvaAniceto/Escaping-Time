using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagableObject : MonoBehaviour, IInteractable
{
    enum EDamageHitDirection
    {
        None,
        Left,
        Right,
        Both
    }

    [SerializeField] private EDamageHitDirection _damageHitDirection;
    [SerializeField, Range (0.0f, 1.0f)] private float _hitMagnitude = 1.0f;

    private Collider2D _collider;

    public List<EInteractionType> Interactions { get; set; } = new List<EInteractionType>();
    public bool Activated { get; set; }
    public bool MovableObject { get; set; }

    private void Awake()
    {
        Interactions.Add(EInteractionType.TriggerEnter);

        _collider = transform.GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        _collider.enabled = true;
    }

    public void SetInteraction(GameObject p_gameObject, EInteractionType p_interactionType)
    {
        if (p_gameObject.TryGetComponent(out PlayerContextManager playercontextManager))
        {
            switch (_damageHitDirection)
            {
                case EDamageHitDirection.None:
                    playercontextManager.HitDirection = Vector3.zero;
                    break;
                case EDamageHitDirection.Left:
                    playercontextManager.HitDirection = Vector3.left * _hitMagnitude;
                    break;
                case EDamageHitDirection.Right:
                    playercontextManager.HitDirection = Vector3.right * _hitMagnitude;
                    break;
                case EDamageHitDirection.Both:
                    Vector3 direction = playercontextManager.transform.position.x > transform.position.x ? Vector3.right : Vector3.left;
                    playercontextManager.HitDirection = direction * _hitMagnitude;
                    break;
            }

            playercontextManager.Damaged = true;

            if (MovableObject)
            {
                _collider.enabled = false;
            }
        }
    }
    public void ConfirmInteraction()
    {

    }
}
