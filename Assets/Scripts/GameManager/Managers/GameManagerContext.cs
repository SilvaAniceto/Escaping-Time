using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerContext : MonoBehaviour
{
    public static GameManagerContext GameManagerInstance;

    [Header("Escaping Time Game Config")]
    [SerializeField] private GameConfig _gameConfig;

    [Header("UI Objects")]
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _characterUI;
    [SerializeField] private GameObject _loadingScren;
    [SerializeField] private GameObject _confirmPanel;

    [Header("UI Button")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _confirmMainMenuButton;

    AsyncOperation asyncOperation;

    private GameManagerAbstractState _exitState;
    private GameManagerAbstractState _currentState;

    public GameManagerAbstractState ExitState { get { return _exitState; } set { _exitState = value; } }
    public GameManagerAbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }

    public GameConfig GameConfig { get => _gameConfig; }
    public string TargetScene { get; set; }
    public AsyncOperation LoadingScenes { get; set; }

    public GameObject MainMenu { get => _mainMenu; }
    public GameObject PauseMenu { get => _pauseMenu; }
    public GameObject CharacterUI { get => _characterUI; }
    public GameObject LoadingScreen { get => _loadingScren; }
    public GameObject ConfirmPanel { get => _confirmPanel; }

    public Button StartButton { get => _startButton; }
    public Button ContinueButton { get => _continueButton; }
    public Button ConfirmMainMenuButton { get => _confirmMainMenuButton; }

    public EventSystem GameManagerEventSystem {  get => EventSystem.current; }

    public bool PauseInput 
    {
        get
        {
            var inputModule = (InputSystemUIInputModule)EventSystem.current.currentInputModule;
            return inputModule.cancel.action.WasPressedThisFrame();
        }
    }

    public static UnityEvent<bool> OnRunOrPauseStateChanged = new UnityEvent<bool>();
 
    private void Awake()
    {
        if (GameManagerInstance == null)
        {
            GameManagerInstance = this;
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

    public void QuitGame()
    {
        Application.Quit();
    }

}
