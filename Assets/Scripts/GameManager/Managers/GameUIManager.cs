using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private CharacterUIManager _characterUIManager;
    #endregion

    #region PROPERTIES
    public GameObject SavingScreen { get => _savingScreen; }
    public GameObject OptionsParent { get => _optionsParent; }
    public Button SelectSaveButton { get => _selectSaveButton; }
    public Button DeleteButton { get => _deleteButton; }
    public GameSaveSlot[] SaveSlots { get => _saveSlots; }

    public GameObject MainMenu { get => _mainMenu; }
    public GameObject SaveMenu { get => _saveMenu; }
    public GameObject PauseMenu { get => _pauseMenu; }
    public CharacterUIManager CharacterUIManager { get => _characterUIManager; }
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
}
