using System.Linq;
using UnityEngine;

public class FakeWallBehavior : MonoBehaviour, IInteractableBehavior
{
    private EInteractionType[] _interactionType = new[] { EInteractionType.Enter };
    public EInteractionType[] InteractionType { get => _interactionType; }

    public void Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        if (!_interactionType.Contains(interactionType))
        {
            return;
        }

        gameObject.SetActive(false);
    }
}