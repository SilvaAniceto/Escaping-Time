using UnityEngine;
using UnityEngine.UI;

public class HubDoor : InteractableItem
{
    [Header("Leve lScene Name")]
    [SerializeField] private string _levelSceneName;

    [Header("Text Objects")]
    [SerializeField] private Text _gemScoreText;
    [SerializeField] private Text _hourglassScoreText;
    [SerializeField] private Text _levelScoreText;

    [Header("Classification Object")]
    [SerializeField] private Image _trophy;
    [SerializeField] private Sprite _goldenTrophy;
    [SerializeField] private Sprite _silverTrophy;
    [SerializeField] private Sprite _brassTrophy;

    public GameLevelManager LevelManager { get; private set; }

    public override void Awake()
    {
        base.Awake();

        Interactions.Add(EInteractionType.Stay);
        Interactions.Add(EInteractionType.Exit);

        GameContextManager.OnHubState.AddListener(SetHubDoor);
    }
    private void SetHubDoor()
    {
        LevelManager = GameContextManager.Instance.GameLevelManagers.Find(x => x.LevelSceneName == _levelSceneName);

        _trophy.transform.parent.gameObject.SetActive(false);
        _gemScoreText.transform.parent.gameObject.SetActive(false);
        _hourglassScoreText.transform.parent.gameObject.SetActive(false);

        switch (LevelManager.ClassficationTierReached)
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

        switch (LevelManager.State)
        {
            case GameLevelManager.EState.Closed:
                Animator.Play("Closed");
                break;
            case GameLevelManager.EState.Open:
                Animator.Play("Opened");
                break;
            case GameLevelManager.EState.Finished:
                Animator.Play("Opened");
                break;
        }
    }

    public override void ConfirmInteraction()
    {
        if (LevelManager.State == GameLevelManager.EState.Open || LevelManager.State == GameLevelManager.EState.Finished)
        {
            GameContextManager.Instance.CharacterContextManager.DisableCharacterContext();
            GameContextManager.Instance.TargetScene = LevelManager.LevelSceneName;
            GameScoreManager.Instance.LevelManager = LevelManager;
            GameContextManager.Instance.CharacterHubStartPosition = transform.position;
            GameScoreManager.Instance.ResetPlayerScorePoints();

            GameStateTransitionManager.OnFadeInEnd.AddListener(() =>
            {
                GameContextManager.Instance.CharacterContextManager.CurrentState.CharacterAnimationManager.SetIdleAnimation();
            });

            GameStateTransitionManager.OnFadeOutEnd.AddListener(() =>
            {
                GameContextManager.Instance.LoadLevel = true;
                GameContextManager.Instance.CharacterContextManager.transform.position = Vector2.zero;
            });

            GameStateTransitionManager.FadeOut();

            GameAudioManager.Instance.StopFadedBGM(0.0f, 1.5f);
        }
        else
        {
            if (GameScoreManager.Instance.MasterScore >= LevelManager.LevelUnlockScore)
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
                if (LevelManager.State == GameLevelManager.EState.Open)
                {
                    _gemScoreText.text = $"???";
                    _hourglassScoreText.text = $"???";
                    _levelScoreText.text = $"???";

                    _trophy.transform.parent.gameObject.SetActive(true);
                    _trophy.color = Color.black;
                    _gemScoreText.transform.parent.gameObject.SetActive(true);
                    _hourglassScoreText.transform.parent.gameObject.SetActive(true);
                }

                if (LevelManager.State == GameLevelManager.EState.Finished)
                {
                    _gemScoreText.text = $"{LevelManager.MaxGemScoreReached}/ {LevelManager.MaxGemScore}";
                    _hourglassScoreText.text = $"{LevelManager.MaxHourglassScoreReached}/ {LevelManager.MaxHourglassScore}";
                    _levelScoreText.text = $"{LevelManager.MaxLevelScoreReached}";

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
        LevelManager.State = GameLevelManager.EState.Open;
    }

    public void SetDoorSFX()
    {
        GameAudioManager.Instance.PlaySFX("Door");
    }
}
