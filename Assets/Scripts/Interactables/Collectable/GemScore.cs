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
        GameAudioManager.Instance.StopSFX();
        GameAudioManager.Instance.PlaySFX("Gem_Collect");

        GameScoreManager.Instance.AddGemScore(_scoreValue);

        gameObject.SetActive(false);
    }
}
