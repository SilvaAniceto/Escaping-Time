using Esper.ESave;
using Esper.ESave.SavableObjects;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerContext : MonoBehaviour
{
    #region STATIC FIELDS
    public static GameManagerContext Instance;

    private static PlayerCharacterData CharacterData = new PlayerCharacterData(new SavableVector(0.00f, 0.00f, 0.00f, 0.00f), false, false, false);

    public static UnityEvent<bool> OnRunOrPauseStateChanged = new UnityEvent<bool>();
    public static UnityEvent OnLoadSceneEnd = new UnityEvent();
    #endregion

    #region INTERNAL CLASSES
    [System.Serializable]
    public class PlayerCharacterData
    {
        public SavableVector CharacterSpawningPosition;
        public bool CharacterHasDash;
        public bool CharacterHasAirJump;
        public bool CharacterHasWallMove;

        public PlayerCharacterData(SavableVector characterSpawningPosition, bool characterHasDash, bool characterHasAirJump, bool characterHasWallMove)
        {
            this.CharacterSpawningPosition = characterSpawningPosition;
            this.CharacterHasDash = characterHasDash;
            this.CharacterHasAirJump = characterHasAirJump;
            this.CharacterHasWallMove = characterHasWallMove;
        }
    }
    #endregion

    #region INSPECTOR FIELDS
    [Header("Game Config")]
    [SerializeField] private GameConfig _gameConfig;

    [Header("Game Save System")]
    [SerializeField] private GameSaveSystem _gameSaveSystem;

    [Header("UI Objects")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _saveMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _characterUI;
    [SerializeField] private GameObject _loadingScren;
    [SerializeField] private GameObject _confirmPanel;

    [Header("UI Button")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _confirmMainMenuButton;
    #endregion

    #region PRIVATE FIELDS
    private GameManagerAbstractState _exitState;
    private GameManagerAbstractState _currentState;
    private CharacterContextManager _characterContextManager;
    #endregion

    #region PROPERTIES
    public GameManagerAbstractState ExitState { get { return _exitState; } set { _exitState = value; } }
    public GameManagerAbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public CharacterContextManager CharacterContextManager { get => _characterContextManager; private set => _characterContextManager = value; }

    public GameConfig GameConfig { get => _gameConfig; }
    public GameSaveSystem GameSaveSystem { get => _gameSaveSystem; }
    public string TargetScene { get; set; }

    public GameObject MainMenu { get => _mainMenu; }
    public GameObject SaveMenu { get => _saveMenu; }
    public GameObject PauseMenu { get => _pauseMenu; }
    public GameObject CharacterUI { get => _characterUI; }
    public GameObject LoadingScreen { get => _loadingScren; }
    public GameObject ConfirmPanel { get => _confirmPanel; }

    public Button StartButton { get => _startButton; }
    public Button QuitButton { get => _quitButton; }
    public Button BackButton { get => _backButton; }
    public Button ContinueButton { get => _continueButton; }
    public Button ConfirmMainMenuButton { get => _confirmMainMenuButton; }

    public EventSystem GameManagerEventSystem {  get => EventSystem.current; }

    public bool InstantiateCharacter { get => CharacterContextManager == null && TargetScene != GameConfig.MainMenuScene; }
    public bool PauseInput 
    {
        get
        {
            var inputModule = (InputSystemUIInputModule)EventSystem.current.currentInputModule;
            return inputModule.cancel.action.WasCompletedThisFrame();
        }
    }
    #endregion

    #region DEFAULT METHODS
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);

        _currentState = new GameManagerStateFactory(this).GameMainMenuState();
    }
    private void Start()
    {
        _currentState.EnterState();
    }
    private void Update()
    {
       _currentState.UpdateStates();
    }
    void OnGUI()
    {
#if UNITY_EDITOR
        GUILayout.Label("Exit State: " + (ExitState == null ? "" : ExitState.ToString()));
        GUILayout.Label("Current State: " + CurrentState.ToString());
#endif

    }
    #endregion

    public void AfterLoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        StartCoroutine(BeforeLoadEnd());
    }
    IEnumerator BeforeLoadEnd()
    {
        if (InstantiateCharacter)
        {
            CharacterContextManager = Instantiate(GameConfig.CharacterContextManager, CharacterData.CharacterSpawningPosition.vector3Value, Quaternion.identity);

            _characterContextManager.HasAirJump = CharacterData.CharacterHasAirJump;
            _characterContextManager.HasDash = CharacterData.CharacterHasDash;
            _characterContextManager.HasWallMove = CharacterData.CharacterHasWallMove;

            DontDestroyOnLoad(_characterContextManager.gameObject);
        }

        yield return new WaitForEndOfFrame();

        OnLoadSceneEnd?.Invoke();
    }

    #region CHARACTER DATA MANAGEMENT
    public static void PrepareCharacterDataToSave(CharacterContextManager characterContextManager)
    {
        CharacterData.CharacterSpawningPosition = characterContextManager.transform.position.ToSavable();
        CharacterData.CharacterHasAirJump = characterContextManager.HasAirJump;
        CharacterData.CharacterHasDash = characterContextManager.HasDash;
        CharacterData.CharacterHasWallMove = characterContextManager.HasWallMove;
    }
    public static void ApplyLoadedCharacterData(PlayerCharacterData data)
    {
        CharacterData.CharacterSpawningPosition = data.CharacterSpawningPosition;
        CharacterData.CharacterHasAirJump = data.CharacterHasAirJump;
        CharacterData.CharacterHasDash = data.CharacterHasDash;
        CharacterData.CharacterHasWallMove = data.CharacterHasWallMove;
    }
    public static PlayerCharacterData CreateCharacterData()
    {
        return new PlayerCharacterData(new SavableVector(0.00f, 3.00f, 0.00f, 0.00f), false, false, false);
    }
    public static PlayerCharacterData GetCharacterData()
    {
        return CharacterData;
    }
    #endregion

    public void QuitGame()
    {
        Application.Quit();
    }
}
