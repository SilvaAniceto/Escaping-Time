using UnityEngine;

public class TargetChangerBehavior : MonoBehaviour, IInteractableBehavior
{
    private EInteractionType[] _interactionType = new[] { EInteractionType.Enter, EInteractionType.Exit };
    public EInteractionType[] InteractionType { get => _interactionType; }

    public void Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        switch (interactionType)
        {
            case EInteractionType.Enter:
                context.CameraTarget.SetParent(null);
                context.CameraTarget.position = transform.position;
                break;
            case EInteractionType.Exit:
                if (context.CameraTarget.parent == null)
                {
                    context.CameraTarget.SetParent(context.transform.GetChild(0));
                    context.CameraTarget.localPosition = Vector3.zero;
                    context.CameraTarget.rotation = context.CameraTarget.parent.rotation;
                }
                break;
        }
    }
}