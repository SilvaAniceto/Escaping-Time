using UnityEngine;
using UnityEngine.UI;

public class HubDoor : InteractableItem
{
    [Header("Leve lScene Name")]
    [SerializeField] private SceneIdentifier _levelSceneName;

    [Header("Text Objects")]
    [SerializeField] private Text _gemScoreText;
    [SerializeField] private Text _hourglassScoreText;
    [SerializeField] private Text _levelScoreText;

    [Header("Classification Object")]
    [SerializeField] private Image _trophy;
    [SerializeField] private Sprite _goldenTrophy;
    [SerializeField] private Sprite _silverTrophy;
    [SerializeField] private Sprite _brassTrophy;

    public GameLevelRuntimeData GameLevelsRuntimeData { get; private set; }

    public override void Awake()
    {
        base.Awake();

        Interactions.Add(EInteractionType.Stay);
        Interactions.Add(EInteractionType.Exit);

        GameEventsManager.OnHubEntered.AddListener(SetHubDoor);
    }
    private void OnDestroy()
    {
        GameEventsManager.OnHubEntered.RemoveListener(SetHubDoor);
    }
    private void SetHubDoor()
    {
        GameLevelsRuntimeData = GameContextManager.Instance.GameLevelsRuntimeData.Find(x => x.LevelSceneName == _levelSceneName);

        _trophy.transform.parent.gameObject.SetActive(false);
        _gemScoreText.transform.parent.gameObject.SetActive(false);
        _hourglassScoreText.transform.parent.gameObject.SetActive(false);

        switch (GameLevelsRuntimeData.ClassficationTierReached)
        {
            case EClassficationTier.None:

                break;
            case EClassficationTier.Tier1:

                _trophy.sprite = _brassTrophy;

                break;
            case EClassficationTier.Tier2:

                _trophy.sprite = _silverTrophy;

                break;
            case EClassficationTier.Tier3:

                _trophy.sprite = _goldenTrophy;

                break;
        }

        switch (GameLevelsRuntimeData.State)
        {
            case ELevelState.Closed:
                Animator.Play("Closed");
                break;
            case ELevelState.Open:
                Animator.Play("Opened");
                break;
            case ELevelState.Finished:
                Animator.Play("Opened");
                break;
        }
    }

    public override void ConfirmInteraction()
    {
        if (GameLevelsRuntimeData.State == ELevelState.Open || GameLevelsRuntimeData.State == ELevelState.Finished)
        {
            GameEventsManager.OnTargetSceneUpdated?.Invoke(GameLevelsRuntimeData.LevelSceneName);
            GameContextManager.Instance.CharacterContextManager.DisableCharacterContext();
            GameContextManager.Instance.CharacterHubStartPosition = transform.position;
            GameContextManager.Instance.ScoreManager.CurrentLevelRuntimeData = GameLevelsRuntimeData;
            GameContextManager.Instance.ScoreManager.ResetPlayerScorePoints();

            GameStateTransitionManager.OnFadeInEnd += (() =>
            {
                GameContextManager.Instance.CharacterContextManager.CurrentState.CharacterAnimationManager.SetIdleAnimation();
            });

            GameStateTransitionManager.OnFadeOutEnd += (() =>
            {
                GameContextManager.Instance.LoadLevel = true;
                GameContextManager.Instance.CharacterContextManager.transform.position = Vector2.zero;
            });

            GameStateTransitionManager.FadeOut();

            GameContextManager.Instance.AudioManager.StopFadedBGM(0.0f, 1.5f);
        }
        else
        {
            if (GameContextManager.Instance.ScoreManager.MasterScore >= GameLevelsRuntimeData.Config.LevelUnlockScore)
            {
                Animator.Play("Opening");
            }
        }
    }

    public override void InteractablePauseState(bool value)
    {
        base.InteractablePauseState(value);
    }

    public override void SetInteraction(CharacterContextManager characterContextManager, EInteractionType interactionType)
    {
        switch (interactionType)
        {
            case EInteractionType.Enter:
                break;
            case EInteractionType.Stay:
                if (GameLevelsRuntimeData.State == ELevelState.Open)
                {
                    _gemScoreText.text = $"???";
                    _hourglassScoreText.text = $"???";
                    _levelScoreText.text = $"???";

                    _trophy.transform.parent.gameObject.SetActive(true);
                    _trophy.color = Color.black;
                    _gemScoreText.transform.parent.gameObject.SetActive(true);
                    _hourglassScoreText.transform.parent.gameObject.SetActive(true);
                }

                if (GameLevelsRuntimeData.State == ELevelState.Finished)
                {
                    _gemScoreText.text = $"{GameLevelsRuntimeData.MaxGemScoreReached}/ {GameLevelsRuntimeData.Config.MaxGemScore}";
                    _hourglassScoreText.text = $"{GameLevelsRuntimeData.MaxHourglassScoreReached}/ {GameLevelsRuntimeData.Config.MaxHourglassScore}";
                    _levelScoreText.text = $"{GameLevelsRuntimeData.MaxLevelScoreReached}";

                    _trophy.transform.parent.gameObject.SetActive(true);
                    _trophy.color = Color.white;
                    _gemScoreText.transform.parent.gameObject.SetActive(true);
                    _hourglassScoreText.transform.parent.gameObject.SetActive(true);
                }

                break;
            case EInteractionType.Exit:

                _trophy.transform.parent.gameObject.SetActive(false);
                _gemScoreText.transform.parent.gameObject.SetActive(false);
                _hourglassScoreText.transform.parent.gameObject.SetActive(false);

                break;
        }
    }

    public void SetOpenState()
    {
        GameLevelsRuntimeData.State = ELevelState.Open;
    }

    public void SetDoorSFX()
    {
        GameContextManager.Instance.AudioManager.PlaySFX("Door");
    }
}
