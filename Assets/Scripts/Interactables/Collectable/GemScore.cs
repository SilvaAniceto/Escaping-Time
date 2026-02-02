using UnityEngine;

public class GemScore : InteractableItem
{
    [SerializeField] private int _scoreValue;

    public int ScoreValue { get => _scoreValue; }

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
        characterContextManager.GameContextManager.GameAudioManager.StopSFX();
        characterContextManager.GameContextManager.GameAudioManager.PlaySFX("Gem_Collect");

        characterContextManager.GameContextManager.ScoreManager.AddGemScore(_scoreValue);

        gameObject.SetActive(false);
    }
}
