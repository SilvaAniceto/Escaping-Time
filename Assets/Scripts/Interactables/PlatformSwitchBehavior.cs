using UnityEngine;

public class PlatformSwitchBehavior : MonoBehaviour, IInteractableBehavior
{
    [SerializeField] private MovingPlatformBehavior _movingPlatform;

    private EInteractionType[] _interactionType = new[] { EInteractionType.Enter, EInteractionType.Exit };
    public EInteractionType[] InteractionType { get => _interactionType; }

    public void Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        if (_movingPlatform == null)
        {
            return;
        }

        if (_movingPlatform.MovingType == MovingPlatformBehavior.MoveType.OneWay && _movingPlatform.ReachTargetPoint())
        {
            return;
        }

        switch (interactionType)
        {
            case EInteractionType.Enter:
                if (_movingPlatform.Activated)
                {
                    return;
                }
                _movingPlatform.ScheduleStart();
                break;
            case EInteractionType.Stay:
                break;
            case EInteractionType.Exit:
                if (!_movingPlatform.Activated)
                {
                    return;
                }
                _movingPlatform.ScheduleStop();
                break;
        }
    }
}