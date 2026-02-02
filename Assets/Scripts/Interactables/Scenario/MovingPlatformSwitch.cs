public class MovingPlatformSwitch : InteractableItem
{
    public MovingPlatform MovingPlatform { get; set; }

    public override void Awake()
    {
        base.Awake();

        Interactions.Add(EInteractionType.Enter);
        Interactions.Add(EInteractionType.Exit);
    }
    public override void ConfirmInteraction()
    {

    }

    public override void InteractablePauseState(bool value)
    {
        base.InteractablePauseState(value);
    }

    public override void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {
        if (MovingPlatform.MovingType == MovingPlatform.MoveType.OneWay)
        {
            return;
        }

        switch (interactionType)
        {
            case EInteractionType.Enter:
                if (MovingPlatform.Activated)
                {
                    return;
                }
                MovingPlatform.ScheduleStart();
                break;
            case EInteractionType.Stay:
                break;
            case EInteractionType.Exit:
                if (!MovingPlatform.Activated)
                {
                    return;
                }

                MovingPlatform.ScheduleStop();
                break;
        }
    }
}