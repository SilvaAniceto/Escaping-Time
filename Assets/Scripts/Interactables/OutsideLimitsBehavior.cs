using System.Linq;
using UnityEngine;

public class OutsideLimitsBehavior : MonoBehaviour, IInteractableBehavior
{
    [SerializeField] private Transform _targetPosition;

    private EInteractionType[] _interactionType = new[] { EInteractionType.Enter };
    public EInteractionType[] InteractionType { get => _interactionType; }

    public void Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        if (!_interactionType.Contains(interactionType))
        {
            return;
        }

        context.DamageManager.SpawningPosition = _targetPosition.position;
        context.DamageManager.ResetCharacter();
    }
}