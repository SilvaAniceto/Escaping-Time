using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerContext : MonoBehaviour
{
    public enum Environment
    {
        Development,
        GameContext
    }

    #region STATIC FIELDS
    public static GameManagerContext Instance;

    public static UnityEvent<bool> OnRunOrPauseStateChanged = new UnityEvent<bool>();
    public static UnityEvent OnLoadSceneEnd = new UnityEvent();
    #endregion

    #region INSPECTOR FIELDS
    [Header("Enviroment Settings")]
    [SerializeField] private Environment _environment = Environment.GameContext;

    [Header("Game Config")]
    [SerializeField] private PlayeableCharacterSet _playeableCharacterSet;
    [SerializeField] private GameLevelManager[] _gameLevelManager;

    [Header("Game Save System")]
    [SerializeField] private GameSaveSystem _gameSaveSystem;

    [Header("Score Manager System")]
    [SerializeField] private GameScoreManager _scoreManager;

    [Header("UI Objects")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _saveMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private CharacterUIManager _characterUI;
    [SerializeField] private GameObject _scorePanel;
    [SerializeField] private GameObject _loadingScren;
    [SerializeField] private GameObject _confirmPanel;

    [Header("UI Button")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _confirmActionButton;
    [SerializeField] private Button _exitLevelButton;
    [SerializeField] private Button _confirmMainMenuButton;
    #endregion

    #region PRIVATE FIELDS
    private GameManagerAbstractState _exitState;
    private GameManagerAbstractState _currentState;
    private CharacterContextManager _characterContextManager;
    private CameraBehaviourController _cameraBehaviourController;
    #endregion

    #region PROPERTIES
    public Environment CurrentEnvironment { get => _environment; }
    public PlayeableCharacterSet PlayeableCharacterSet { get => _playeableCharacterSet; }
    public GameManagerAbstractState ExitState { get { return _exitState; } set { _exitState = value; } }
    public GameManagerAbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public CharacterContextManager CharacterContextManager { get => _characterContextManager; private set => _characterContextManager = value; }
    public CameraBehaviourController CameraBehaviourController { get => _cameraBehaviourController; private set => _cameraBehaviourController = value; }

    public GameSaveSystem SaveSystem { get => _gameSaveSystem; }
    public GameScoreManager ScoreManager { get => _scoreManager; private set => _scoreManager = value; }
    public GameLevelManager[] GameLevelManager { get => _gameLevelManager; set => _gameLevelManager = value; }
    public string TargetScene { get; set; }

    public GameObject MainMenu { get => _mainMenu; }
    public GameObject SaveMenu { get => _saveMenu; }
    public GameObject PauseMenu { get => _pauseMenu; }
    public CharacterUIManager CharacterUI { get => _characterUI; }
    public GameObject ScorePanel { get => _scorePanel; }
    public GameObject LoadingScreen { get => _loadingScren; }
    public GameObject ConfirmPanel { get => _confirmPanel; }

    public Button StartButton { get => _startButton; }
    public Button QuitButton { get => _quitButton; }
    public Button BackButton { get => _backButton; }
    public Button ContinueButton { get => _continueButton; }
    public Button ConfirmActionButton { get => _confirmActionButton; }
    public Text ExitLevelButtonText { get => _exitLevelButton.GetComponentInChildren<Text>(); }
    public Button ConfirmMainMenuButton { get => _confirmMainMenuButton; }

    public EventSystem GameManagerEventSystem {  get => EventSystem.current; }

    public bool InstantiateCharacter { get => CharacterContextManager == null && TargetScene != "MainMenu"; }
    public bool SetTimer { get; set; } = false;
    public bool LoadLevel { get; set; } = false;
    public bool SetLevelScore { get; set; } = false;
    public bool FinishSetLevelScore { get; set; } = false;
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
        
        if (_environment == Environment.Development) return;

        DontDestroyOnLoad(gameObject);

        _currentState = new GameManagerStateFactory(this, GetComponent<GameUIInputsManager>()).GameMainMenuState();

        OnRunOrPauseStateChanged.AddListener((value) => { SetTimer = value; });
    }
    private void Start()
    {
        if (_environment == Environment.Development) return;

        _currentState.EnterState();
    }
    private void Update()
    {
        if (_environment == Environment.GameContext)
        {
            _currentState.UpdateStates();
        }

        if (SetTimer)
        {
            ScoreManager.SetCurrentTimer();
        }
    }
    void OnGUI()
    {
#if UNITY_EDITOR
        //GUILayout.Label("Exit State: " + (ExitState == null ? "" : ExitState.ToString()));
        //GUILayout.Label("Current State: " + CurrentState.ToString());
#endif

    }
    #endregion

    #region SCENE MANAGEMENT
    public void AfterLoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        StartCoroutine(BeforeLoadEnd(scene));
    }
    IEnumerator BeforeLoadEnd(Scene scene)
    {
        if (InstantiateCharacter)
        {
            yield return StartCoroutine(StartInstantiateCharacter(scene));
        }

        yield return new WaitForSeconds(5.00f);

        OnLoadSceneEnd?.Invoke();

        SaveSystem.LoadedProfileDataToContext();

        List<Door> doors = FindObjectsByType<Door>(FindObjectsSortMode.None).ToList();

        foreach (Door door in doors)
        {
            door.CharacterContextManager = CharacterContextManager;
            door.SetOpeningAnimation();
        }

        bool active = scene.name.Equals("Level_Hub") ? true : false;

        if (CharacterContextManager != null)
        {
            CharacterContextManager.EnableCharacterGraphics = active;
            CharacterContextManager.CurrentState.PlayerInputManager.enabled = active;
            CharacterContextManager.transform.position = Vector3.zero;

            if (!active)
            {
                ScoreManager.ResetPlayerScorePoints();
                CharacterUI.SetScoreDisplay(ScoreManager.CurrentScore);
            }
            else
            {
                CharacterUI.SetScoreDisplay(ScoreManager.MasterScore);
            }
        }
    }
    IEnumerator StartInstantiateCharacter(Scene scene)
    {
        CharacterContextManager = Instantiate(PlayeableCharacterSet.CharacterContextManager, Vector3.zero, Quaternion.identity);

        DontDestroyOnLoad(CharacterContextManager.gameObject);

        if (CameraBehaviourController == null)
        {
            CameraBehaviourController = Instantiate(PlayeableCharacterSet.CameraBehaviourController);
            CameraBehaviourController.SetCinemachineTarget(CharacterContextManager.CameraTarget);
            DontDestroyOnLoad(CameraBehaviourController.gameObject);
        }

        yield return new WaitForEndOfFrame();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}
