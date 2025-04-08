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
    private Animator _animator;

    public List<EInteractionType> Interactions { get; set; } = new List<EInteractionType>();
    public bool Activated { get; set; }

    private void Awake()
    {
        Interactions.Add(EInteractionType.Enter);

        _collider = transform.GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();

        GameManagerContext.OnRunOrPauseStateChanged.AddListener(InteractablePauseState);
    }

    private void OnEnable()
    {
        _collider.enabled = true;
    }

    private void OnDestroy()
    {
        
    }

    public void SetInteraction(CharacterContextManager characterContextManager)
    {
        if (characterContextManager.TakingDamage) return; 

        float currentDirection = 0;
        
        switch (_damageHitDirection)
        {
            case EDamageHitDirection.None:
                currentDirection = 0.00f;
                break;
            case EDamageHitDirection.Left:
                currentDirection = -1.00f * _hitMagnitude;
                break;
            case EDamageHitDirection.Right:
                currentDirection = 1.00f * _hitMagnitude;
                break;
            case EDamageHitDirection.Both:
                float direction = characterContextManager.transform.position.x > transform.position.x ? 1.00f : -1.00f;
                currentDirection = direction * _hitMagnitude;
                break;
        }

        characterContextManager.ApplyDamage(currentDirection);
    }
    public void ConfirmInteraction()
    {

    }

    public void InteractablePauseState(bool value)
    {
        if (_animator != null)
        {
            _animator.enabled = value;
        }            

        this.enabled = value;
    }
}
