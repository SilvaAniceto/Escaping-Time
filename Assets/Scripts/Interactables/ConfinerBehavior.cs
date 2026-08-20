using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ConfinerBehavior : MonoBehaviour, IInteractableBehavior
{
    [SerializeField] private Collider2D _confinerCollider;
    [SerializeField] private UnityEvent OnConfinerChanged = new UnityEvent();

    private EInteractionType[] _interactionType = new[] { EInteractionType.Enter };
    public EInteractionType[] InteractionType { get => _interactionType; }

    public void Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        if (!_interactionType.Contains(interactionType))
        {
            return;
        }

        context.CameraBehaviourController.SetCameraConfiner2D(_confinerCollider);
        OnConfinerChanged?.Invoke();
    }
}