using UnityEngine;
using UnityEngine.UI;

public class HubDoorBehavior : MonoBehaviour, IInteractableBehavior, IConfirmable
{
    [Header("Level Scene Name")]
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

    private GameLevelRuntimeData _gameLevelsRuntimeData;
    private Animator _animator;

    private EInteractionType[] _interactionType = new[] { EInteractionType.Stay, EInteractionType.Exit };
    public EInteractionType[] InteractionType { get => _interactionType; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        GameEventsManager.OnHubEntered.AddListener(SetHubDoor);
    }

    private void OnDestroy()
    {
        GameEventsManager.OnHubEntered.RemoveListener(SetHubDoor);
    }

    private void SetHubDoor()
    {
        _gameLevelsRuntimeData = ServiceLocator.GameFlowManager.GameLevelsRuntimeData.Find(x => x.LevelSceneName == _levelSceneName);

        _trophy.transform.parent.gameObject.SetActive(false);
        _gemScoreText.transform.parent.gameObject.SetActive(false);
        _hourglassScoreText.transform.parent.gameObject.SetActive(false);

        switch (_gameLevelsRuntimeData.ClassficationTierReached)
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

        switch (_gameLevelsRuntimeData.State)
        {
            case ELevelState.Closed:
                _animator.Play("Closed");
                break;
            case ELevelState.Open:
                _animator.Play("Opened");
                break;
            case ELevelState.Finished:
                _animator.Play("Opened");
                break;
        }
    }

    public void Execute(CharacterContextManager context, EInteractionType interactionType)
    {
        switch (interactionType)
        {
            case EInteractionType.Enter:
                break;
            case EInteractionType.Stay:
                if (_gameLevelsRuntimeData.State == ELevelState.Open)
                {
                    _gemScoreText.text = "???";
                    _hourglassScoreText.text = "???";
                    _levelScoreText.text = "???";

                    _trophy.transform.parent.gameObject.SetActive(true);
                    _trophy.color = Color.black;
                    _gemScoreText.transform.parent.gameObject.SetActive(true);
                    _hourglassScoreText.transform.parent.gameObject.SetActive(true);
                }

                if (_gameLevelsRuntimeData.State == ELevelState.Finished)
                {
                    _gemScoreText.text = $"{_gameLevelsRuntimeData.MaxGemScoreReached}/ {_gameLevelsRuntimeData.Config.MaxGemScore}";
                    _hourglassScoreText.text = $"{_gameLevelsRuntimeData.MaxHourglassScoreReached}/ {_gameLevelsRuntimeData.Config.MaxHourglassScore}";
                    _levelScoreText.text = $"{_gameLevelsRuntimeData.MaxLevelScoreReached}";

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

    public void ConfirmInteraction(CharacterContextManager context)
    {
        if (_gameLevelsRuntimeData.State == ELevelState.Open || _gameLevelsRuntimeData.State == ELevelState.Finished)
        {
            GameEventsManager.OnTargetSceneUpdated?.Invoke(_gameLevelsRuntimeData.LevelSceneName);
            context.DisableCharacterContext();
            ServiceLocator.GameFlowManager.CharacterHubStartPosition = transform.position;
            ServiceLocator.ScoreManager.CurrentLevelRuntimeData = _gameLevelsRuntimeData;
            ServiceLocator.ScoreManager.ResetPlayerScorePoints();

            GameStateTransitionManager.OnFadeInEnd += (() =>
            {
                context.CurrentState.CharacterAnimationManager.SetIdleAnimation();
            });

            GameStateTransitionManager.OnFadeOutEnd += (() =>
            {
                ServiceLocator.GameFlowManager.LoadLevel = true;
                context.transform.position = Vector2.zero;
            });

            GameStateTransitionManager.FadeOut();
            ServiceLocator.AudioManager.StopFadedBGM(0.0f, 1.5f);
        }
        else
        {
            if (ServiceLocator.ScoreManager.MasterScore >= _gameLevelsRuntimeData.Config.LevelUnlockScore)
            {
                _animator.Play("Opening");
            }
        }
    }

    public void SetOpenState()
    {
        _gameLevelsRuntimeData.State = ELevelState.Open;
    }

    public void SetDoorSFX()
    {
        ServiceLocator.AudioManager.PlaySFX("Door");
    }
}