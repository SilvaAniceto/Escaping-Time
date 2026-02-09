using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUIManager : MonoBehaviour
{
    [System.Serializable]
    public class GameSaveSlot
    {
        public Button slotButton;
        public Text slotLabel;

        private int slotIndex;

        public void SetLabelText(string text)
        {
            this.slotLabel.text = text;
        }
        public void SetSlotIndex(int slotIndex)
        {
            this.slotIndex = slotIndex;
        }
        public int GetSlotIndex()
        {
            return this.slotIndex;
        }
    }

    #region INSPECTOR FIELDS
    [Header("Game Context Manager UI Objects")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _saveMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _scorePanel;
    [SerializeField] private GameObject _loadingScren;
    [SerializeField] private GameObject _confirmPanel;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _confirmActionButton;
    [SerializeField] private Button _exitLevelButton;
    [SerializeField] private Button _confirmMainMenuButton;
    [SerializeField] private Text _confirmPanelText;

    [Header("Game Save Slot UI Objects")]
    [SerializeField] private GameSaveSlot[] _saveSlots;

    [Header("Game Save System UI Objects")]
    [SerializeField] private GameObject _savingScreen;
    [SerializeField] private GameObject _optionsParent;
    [SerializeField] private Button _selectSaveButton;
    [SerializeField] private Button _deleteButton;

    [Header("Character UI Objects")]
    [SerializeField] private GameObject _characterUIManager;

    [Header("Game Score UI Objects")]
    [SerializeField] private GameObject _masterScoreIcon;
    [SerializeField] private GameObject _levelScoreIcon;
    [SerializeField] private GameObject _timer;
    [SerializeField] private Image _fill;
    [SerializeField] private Image _brass;
    [SerializeField] private Image _silver;
    [SerializeField] private Image _gold;

    [Header("Game Score Text Fields")]
    [SerializeField] private Text _gemScore;
    [SerializeField] private Text _hourglassScore;
    [SerializeField] private Text _timeScore;
    [SerializeField] private Text _masterScore;
    [SerializeField] private Text _timerDisplay;
    [SerializeField] private Text _scoreDisplay;

    [Header("Power Ups")]
    [SerializeField] private Animator _airJumpAnimatorPowerUp;
    [SerializeField] private Image _airJumpUIpowerUp;
    [SerializeField] private Animator _dashAnimatorPowerUp;
    [SerializeField] private Image _dashUIpowerUp;
    [SerializeField] private Animator _wallMoveAnimatorPowerUp;
    [SerializeField] private Image _wallMoveUIpowerUp;

    [Header("Hourglass")]
    [SerializeField] private Transform _hourglassParent;
    #endregion

    #region PROPERTIES
    public GameContextManager GameContextManager { get; private set; }
    public GameObject SavingScreen { get => _savingScreen; }
    public GameObject OptionsParent { get => _optionsParent; }
    public Button SelectSaveButton { get => _selectSaveButton; }
    public Button DeleteButton { get => _deleteButton; }
    public GameSaveSlot[] SaveSlots { get => _saveSlots; }

    public GameObject MainMenu { get => _mainMenu; }
    public GameObject SaveMenu { get => _saveMenu; }
    public GameObject PauseMenu { get => _pauseMenu; }
    public GameObject CharacterUIManager { get => _characterUIManager; }
    public GameObject ScorePanel { get => _scorePanel; }
    public GameObject LoadingScreen { get => _loadingScren; }
    public GameObject ConfirmPanel { get => _confirmPanel; }
    public Text ConfirmPanelText { get => _confirmPanelText; }

    public Button StartButton { get => _startButton; }
    public Button QuitButton { get => _quitButton; }
    public Button BackButton { get => _backButton; }
    public Button ContinueButton { get => _continueButton; }
    public Button ConfirmActionButton { get => _confirmActionButton; }
    public Button ExitLevelButton { get => _exitLevelButton; }
    public Text ExitLevelButtonText { get => _exitLevelButton.GetComponentInChildren<Text>(); }
    public Button ConfirmMainMenuButton { get => _confirmMainMenuButton; }
    #endregion

    private GameManagerUIActions GameManagerUIActions { get; set; }

    public bool Navigating { get => GameManagerUIActions.UIActions.Navigation.triggered; }
    public bool Confirm { get; private set; }
    public bool Start { get => GameManagerUIActions.UIActions.Start.triggered; }

    private void Awake()
    {
        if (GameManagerUIActions == null)
        {
            GameManagerUIActions = new GameManagerUIActions();
        }
    }
    private void OnEnable()
    {
        GameManagerUIActions.Enable();
    }

    private void OnDisable()
    {
        GameManagerUIActions.Disable();
    }
    private void OnDestroy()
    {
        _selectSaveButton.onClick.RemoveAllListeners();
        _deleteButton.onClick.RemoveAllListeners();
    }
    public void SetConfirmAction()
    {
        Confirm = !Confirm;
    }
    public void Initialize(GameContextManager gameContextManager)
    {
        GameContextManager = gameContextManager;
    }

    #region CHARACTER UI 
    public void SetHubUIObjects()
    {
        _masterScoreIcon.SetActive(true);
        _levelScoreIcon.SetActive(false);
        _timer.SetActive(false);
    }
    public void SetLevelUIObjects()
    {
        _masterScoreIcon.SetActive(false);
        _levelScoreIcon.SetActive(true);
        _timer.SetActive(true);
    }
    public void SetTimerDisplay(string time)
    {
        _timerDisplay.text = $"time: {time}";
    }
    public void SetScoreDisplay(int points)
    {
        _scoreDisplay.text = points.ToString();
    }
    public void SetHourglassDisplay(int count)
    {
        if (count == 0)
        {
            for (int i = 0; i < _hourglassParent.childCount; i++)
            {
                _hourglassParent.GetChild(i).gameObject.SetActive(false);
            }

            return;
        }

        int value = count - 1;

        value = Mathf.Clamp(value, 0, 4);

        _hourglassParent.GetChild(value).gameObject.SetActive(true);
    }
    public void SetAirJumpPowerUpUI(string clip)
    {
        _airJumpAnimatorPowerUp.gameObject.SetActive(true);

        _airJumpAnimatorPowerUp.Play(clip);
        _airJumpUIpowerUp.fillAmount = 1;
    }
    public void SetOvertimeAirJumpPowerUpUI(float value, CharacterContextManager characterContextManager)
    {
        StartCoroutine(FillAmount(value, _airJumpUIpowerUp, _airJumpAnimatorPowerUp, characterContextManager));
    }
    public void SetDashPowerUpUI(string clip)
    {
        _dashAnimatorPowerUp.gameObject.SetActive(true);

        _dashAnimatorPowerUp.Play(clip);
        _dashUIpowerUp.fillAmount = 1;
    }
    public void SetOvertimeDashPowerUpUI(float value, CharacterContextManager characterContextManager)
    {
        StartCoroutine(FillAmount(value, _dashUIpowerUp, _dashAnimatorPowerUp, characterContextManager));
    }
    public void SetWallMovePowerUpUI(string clip)
    {
        _wallMoveAnimatorPowerUp.gameObject.SetActive(true);

        _wallMoveAnimatorPowerUp.Play(clip);
        _wallMoveUIpowerUp.fillAmount = 1;
    }
    public void SetOvertimeWallMovePowerUpUI(float value, CharacterContextManager characterContextManager)
    {
        StartCoroutine(FillAmount(value, _wallMoveUIpowerUp, _wallMoveAnimatorPowerUp, characterContextManager));
    }

    IEnumerator FillAmount(float value, Image sourceImage, Animator animator, CharacterContextManager characterContextManager)
    {
        float currentTime = value;

        characterContextManager.GameAudioManager.CreateEnqueuedPowerUpSFX("TimeCount", value, sourceImage, true);

        while (currentTime >= 0.00f)
        {
            currentTime -= Time.deltaTime;

            sourceImage.fillAmount = Mathf.Clamp01(currentTime / value);

            yield return null;
        }

        characterContextManager.GameAudioManager.StopEnqueuedPowerUpSFX();
        characterContextManager.GameAudioManager.PlaySFX("EndTimeCount");

        System.Action action = () =>
        {
            animator.gameObject.SetActive(false);
            characterContextManager.DispatchPowerUpInteractableRecharge();
        };

        GameContextManager.Instance.WaitSeconds(action, characterContextManager.GameAudioManager.AudioClipLength("EndTimeCount"));
    }
    #endregion

    #region SCORE UI
    public void ResetScoreUI()
    {
        _brass.color = Color.black;
        _silver.color = Color.black;
        _gold.color = Color.black;
        _fill.fillAmount = 0;
    }
    public void SetTrophyUIPosition()
    {
        _silver.rectTransform.anchoredPosition = new Vector2(_fill.rectTransform.rect.width * GameContextManager.ScoreManager.SilverScorePercentage, _silver.rectTransform.anchoredPosition.y);
        _brass.rectTransform.anchoredPosition = new Vector2(_fill.rectTransform.rect.width * GameContextManager.ScoreManager.BrassScorePercentage, _brass.rectTransform.anchoredPosition.y);
    }
    public void SetGemScoreText(string text)
    {
        _gemScore.text = text;
    }
    public void SetHourglassText(string text)
    {
        _hourglassScore.text = text;
    }
    public void SetTimeScoreText(string text)
    {
        _timeScore.text = text;
    }
    public void SetMasterScoreText(string text)
    {
        _masterScore.text = text;
    }
    public void SetFillAmount(float value)
    {
        _fill.fillAmount = value;
    }
    public void ResetBrassTrophy()
    {
        _brass.color = Color.white;
    }
    public void ResetSilverTrophy()
    {
        _silver.color = Color.white;
    }
    public void ResetGoldTrophy()
    {
        _gold.color = Color.white;
    }
    #endregion
}
