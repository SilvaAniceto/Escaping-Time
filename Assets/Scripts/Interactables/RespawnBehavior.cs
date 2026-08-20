using System.Linq;
using UnityEngine;

public class RespawnBehavior : MonoBehaviour, IInteractableBehavior
{
    private EInteractionType[] _interactionType = new[] { EInteractionType.Enter };
    public EInteractionType[] InteractionType { get => _interactionType; }

    void IInteractableBehavior.Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        if (!_interactionType.Contains(interactionType))
        {
            return;
        }

        context.DamageManager.SpawningPosition = transform.position;
    }
}
