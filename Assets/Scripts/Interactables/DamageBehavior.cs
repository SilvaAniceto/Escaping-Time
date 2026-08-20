using System.Linq;
using UnityEngine;

public class DamageBehavior : MonoBehaviour, IInteractableBehavior
{
    enum EDamageHitDirection
    {
        None,
        Left,
        Right,
        Both
    }

    [SerializeField] private bool _ignoreDashState = false;
    [SerializeField] private EDamageHitDirection _damageHitDirection;
    [SerializeField, Range(1, 5)] private int _hitMagnitude = 1;

    private EInteractionType[] _interactionType = new[] { EInteractionType.Enter };
    public EInteractionType[] InteractionType { get => _interactionType; }

    public void Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        if (!_interactionType.Contains(interactionType))
        {
            return;
        }

        if (context.CurrentState == context.CurrentState.CharacterStateFactory.DamagedState() ||
            context.CurrentState == context.CurrentState.CharacterStateFactory.ResetState())
        {
            return;
        }

        if (_ignoreDashState && context.CurrentState == context.CurrentState.CharacterStateFactory.DashState())
        {
            return;
        }

        ServiceLocator.AudioManager.StopCharacterSFX();
        ServiceLocator.AudioManager.PlayCharacterSFX("Damage");

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
                float direction = context.transform.position.x > transform.position.x ? 1.00f : -1.00f;
                currentDirection = direction * _hitMagnitude;
                break;
        }

        context.DamageManager.ApplyDamage(currentDirection);
    }
}