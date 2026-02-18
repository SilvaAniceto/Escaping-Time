public class Hourglass : InteractableItem
{
    public override void Awake()
    {
        base.Awake();

        Interactions.Add(EInteractionType.Enter);
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
        gameObject.SetActive(false);

        GameScoreManager.Instance.AddCollectedHourglass();

        GameAudioManager.Instance.StopSFX();
        GameAudioManager.Instance.PlaySFX("Hourglass_Collect");
    }
}
