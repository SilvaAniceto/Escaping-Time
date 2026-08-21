using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameContextManager : MonoBehaviour, IGameStateManager, IGameFlowManager
{
    private static GameContextManager Instance;

    #region INSPECTOR FIELDS
    [Header("Enviroment Settings")]
    [SerializeField] private GameEnvironmentManager _environment;

    [Header("Playeable Character Set")]
    [SerializeField] private PlayeableCharacterSet _playeableCharacterSet;

    [Header("Game Level Config")]
    [SerializeField] private GameLevelConfig[] _gameLevelConfigs;

    [Header("Game Save System")]
    [SerializeField] private GameSaveSystem _gameSaveSystem;

    [Header("Game UI Manager")]
    [SerializeField] private GameUIManager _gameUIManager;
    [SerializeField] private GameStateTransitionManager _transitionScreen;

    [Header("Game Audio Manager")]
    [SerializeField] private GameAudioManager _gameAudioManager;

    [Header("Game Scene Loader")]
    [SerializeField] private GameSceneLoader _gameSceneLoader;

    [Header("Debug Settings")]
    [SerializeField] private bool _debugOnGui = false;
    #endregion

    #region PRIVATE FIELDS
    private AudioListener _gameContextAudiolistener;

    private GameManagerAbstractState _exitState;
    private GameManagerAbstractState _currentState;

    private CharacterContextManager _characterContextManager;
    private CameraBehaviourController _cameraBehaviourController;

    private PlayerInputManager _playerInputManager;
    #endregion

    #region PROPERTIES
    public GameSaveSystem SaveSystem => _gameSaveSystem;
    public GameUIManager UIManager => _gameUIManager;
    public GameAudioManager AudioManager => _gameAudioManager;
    public GameScoreManager ScoreManager { get; private set; }
    public List<GameLevelRuntimeData> GameLevelsRuntimeData { get; private set; } = new List<GameLevelRuntimeData>();
    public PlayeableCharacterSet PlayeableCharacterSet { get => _playeableCharacterSet; }
    public GameManagerAbstractState ExitState { get { return _exitState; } set { _exitState = value; } }
    public GameManagerAbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public CharacterContextManager CharacterContextManager { get => _characterContextManager; }
    public EventSystem GameManagerEventSystem {  get => EventSystem.current; }
    public Vector2 CharacterHubStartPosition { get; set; }
    public bool InstantiateCharacter { get => _characterContextManager == null && _gameSceneLoader.TargetScene != SceneIdentifier.MainMenu; }
    public bool SetTimer { get; set; } = false;
    public bool LoadLevel { get; set; } = false;
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
            Destroy(gameObject);
            return;
        }

#if !UNITY_EDITOR
        _environment = Environment.GameContext;
        Screen.SetResolution(1920, 1080, true);
        Application.targetFrameRate = 60;
#endif

        _gameContextAudiolistener = GetComponentInChildren<AudioListener>();

        _environment.ApplyGameEnvironmentSettings();

        switch (_environment.CurrentEnvironment)
        {
            case GameEnvironmentManager.Environment.Development:
                StartDevelopmentEnvironment();
                break;
            case GameEnvironmentManager.Environment.GameContext:
                StartGameContextEnvironment();
                break;
        }

        _gameSceneLoader.InitializeGameSceneLoader();
    }
    private void Start()
    {
        if (_environment.CurrentEnvironment == GameEnvironmentManager.Environment.Development) return;

        _currentState.EnterState();
    }
    private void Update()
    {
        _cameraBehaviourController?.CameraVerticalOffset();
        _playerInputManager?.UpdatePlayerInputManager();

        if (_environment.CurrentEnvironment == GameEnvironmentManager.Environment.GameContext)
        {
            _currentState.UpdateStates();
        }

        if (SetTimer)
        {
            ScoreManager.SetCurrentTimer();
        }
    }
    private void OnDestroy()
    {

    }
    void OnGUI()
    {
#if UNITY_EDITOR
        if (_debugOnGui)
        {
            GUILayout.Label("FPS: " + Mathf.RoundToInt(1f / Time.deltaTime));
            GUILayout.Label("Exit State: " + (ExitState == null ? "" : ExitState.ToString()));
            GUILayout.Label("Current State: " + (CurrentState == null ? "" : CurrentState.ToString()));
            GUILayout.Label("-----------------------------------------------");
            if (_characterContextManager != null)
            {
                GUILayout.Label("Current State: " + _characterContextManager.CurrentState.ToString());
                GUILayout.Label("Current Sub State: " + (_characterContextManager.CurrentState.CurrentSubState != null ? _characterContextManager.CurrentState.CurrentSubState.ToString() : ""));
            }
        }
#endif
    }
    #endregion

    #region SCENE MANAGEMENT
    public void SceneHandler(Scene scene, LoadSceneMode loadSceneMode)
    {
        StartCoroutine(BeforeLoadEnd(scene));
    }
    IEnumerator BeforeLoadEnd(Scene scene)
    {
        if (InstantiateCharacter)
        {
            yield return StartCoroutine(StartInstantiateCharacter());
        }

        yield return new WaitForSeconds(5.00f);

        _gameSaveSystem.LoadProfileDataToContext(this);

        GameEventsManager.OnSceneLoaded?.Invoke();
    }
    IEnumerator StartInstantiateCharacter()
    {
        _characterContextManager = Instantiate(PlayeableCharacterSet.CharacterContextManager, Vector3.zero, Quaternion.identity);

        _gameContextAudiolistener.enabled = false;

        DontDestroyOnLoad(_characterContextManager.gameObject);

        if (_cameraBehaviourController == null)
        {
            _cameraBehaviourController = Instantiate(PlayeableCharacterSet.CameraBehaviourController);
            _cameraBehaviourController.SetCinemachineTarget(_characterContextManager.CameraTarget);
            DontDestroyOnLoad(_cameraBehaviourController.gameObject);
        }

        _playerInputManager = new PlayerInputManager(this, _characterContextManager, _cameraBehaviourController, new PlayerInputActions());

        _characterContextManager.InitializeCharacterContextManager(_playerInputManager, _cameraBehaviourController);

        CharacterHubStartPosition = Vector2.zero;

        GameStateTransitionManager.OnFadeInStart += (() =>
        {
            _characterContextManager.EnableCharacterContext();
        });

        GameEventsManager.OnPauseStateChanged.AddListener((value) =>
        {
            _characterContextManager.enabled = value;
            _characterContextManager.Rigidbody.bodyType = value ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
            _characterContextManager.CurrentState.CharacterAnimationManager.CharacterAnimator.enabled = value;
        });

        yield return new WaitForEndOfFrame();

        _playerInputManager.Initialize();
    }
    public void WaitFrameEnd(Action action)
    {
        StartCoroutine(OnWaitFrameEnd(action));
    }
    IEnumerator OnWaitFrameEnd(Action action)
    {
        yield return new WaitForEndOfFrame();
        if (action != null)
        {
            action();
        }
    }
    public void WaitSeconds(Action action, float waitTime)
    {
        StartCoroutine(OnWaitSeconds(action, waitTime));
    }
    IEnumerator OnWaitSeconds(Action action, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (action != null)
        {
            action();
        }
    }
    public void QuitToMainMenu()
    {
        OnQuitToMainMenu();
    }
    public void OnQuitToMainMenu()
    {
        _playerInputManager.ClearPlayerActionsCallback();
        _playerInputManager = null;
        GameEventsManager.OnPauseStateChanged.RemoveAllListeners();
        Destroy(_cameraBehaviourController.gameObject);
        Destroy(_characterContextManager.gameObject);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion

    #region MAIN MENU STATE
    public void OnEnterMainMenuState()
    {
        InstantiateLevelManagers();

        _gameContextAudiolistener.enabled = true;

        GameEventsManager.OnPauseStateChanged.RemoveAllListeners();

        _gameUIManager.MainMenu.SetActive(true);

        _gameUIManager.StartButton.gameObject.SetActive(true);
        _gameUIManager.QuitButton.gameObject.SetActive(true);

        GameManagerEventSystem.SetSelectedGameObject(_gameUIManager.StartButton.gameObject);

        _exitState = null;

        _gameAudioManager.PlayFadedBGM("Main_Menu", 2.0f);
    }
    public void OnExitMainMenuState()
    {
        _gameUIManager.MainMenu.SetActive(false);
    }
    #endregion

    #region SAVE MENU STATE
    public void OnEnterSaveMenuState()
    {
        _gameSaveSystem.ShowSlots();

        _gameUIManager.SaveMenu.SetActive(true);

        _gameUIManager.BackButton.gameObject.SetActive(true);

        _exitState = _currentState.GameManagerStateFactory.GameHubState();

        GameManagerEventSystem.SetSelectedGameObject(_gameUIManager.SaveSlots[0].slotButton.gameObject);
    }
    public void OnExitSaveMenuState()
    {
        _gameUIManager.SaveMenu.SetActive(false);

        _gameUIManager.BackButton.gameObject.SetActive(false);

        GameManagerEventSystem.SetSelectedGameObject(null);
    }
    #endregion

    #region LOADING STATE
    public void OnEnterLoadingState()
    {
        GameStateTransitionManager.FadeOff();

        _gameUIManager.CharacterUIManager.SetActive(false);
        _gameUIManager.LoadingScreen.SetActive(true);

        _gameSceneLoader.LoadTargetScene();

        GameEventsManager.OnSceneLoaded.RemoveAllListeners();
        GameEventsManager.OnSceneLoaded.AddListener(() =>
        {
            WaitFrameEnd(() =>
            {
                _currentState.SwitchState(_exitState);
            });
        });

        SceneManager.sceneLoaded += SceneHandler;

        GameManagerEventSystem.SetSelectedGameObject(null);
    }
    public void OnExitLoadingState()
    {
        SceneManager.sceneLoaded -= SceneHandler;
        _gameUIManager.LoadingScreen.SetActive(false);
        GameStateTransitionManager.FadeIn();
    }
    #endregion

    #region HUB STATE
    public void OnEnterHubState()
    {
        _gameUIManager.SetHubUIObjects();

        _gameUIManager.SetScoreDisplay(ScoreManager.MasterScore);

        _gameUIManager.CharacterUIManager.SetActive(true);

        GameEventsManager.OnHubEntered?.Invoke();

        GameManagerEventSystem.SetSelectedGameObject(null);

        _gameAudioManager.PlayFadedBGM("Hub_Loop", 1.6f);
    }
    #endregion

    #region RUN STATE
    public void OnEnterRunState()
    {
        _exitState = null;

        _gameUIManager.SetLevelUIObjects();

        _gameUIManager.CharacterUIManager.SetActive(true);

        _gameUIManager.SetScoreDisplay(ScoreManager.CurrentScore);

        GameManagerEventSystem.SetSelectedGameObject(null);
    }
    public void OnExitRunState()
    {
        _gameUIManager.CharacterUIManager.SetActive(false);
    }
    #endregion

    #region PAUSE STATE
    public void OnEnterPauseState()
    {
        GameEventsManager.OnPauseStateChanged?.Invoke(false);

        _gameUIManager.PauseMenu.SetActive(true);

        GameManagerEventSystem.SetSelectedGameObject(_gameUIManager.ContinueButton.gameObject);
    }
    public void OnExitPauseState()
    {
        _gameUIManager.PauseMenu.SetActive(false);
        GameEventsManager.OnPauseStateChanged?.Invoke(true);
    }
    public void PauseGameOnHubState()
    {
        _exitState = _currentState.GameManagerStateFactory.GameHubState();

        _gameUIManager.ExitHubButton.gameObject.SetActive(true);
        _gameUIManager.ExitLevelButton.gameObject.SetActive(false);

        _gameUIManager.QuitToMainMenuPanelText.SetActive(true);
        _gameUIManager.QuitToHubPanelText.SetActive(false);

        _gameUIManager.ConfirmMainMenuButton.gameObject.SetActive(true);
        _gameUIManager.ConfirmHubButton.gameObject.SetActive(false);

        _currentState.SwitchState(_currentState.GameManagerStateFactory.GamePauseState());
    }
    public void PauseOnRunState()
    {
        _exitState = _currentState.GameManagerStateFactory.GameRunState();

        _gameUIManager.ExitHubButton.gameObject.SetActive(false);
        _gameUIManager.ExitLevelButton.gameObject.SetActive(true);

        _gameUIManager.QuitToMainMenuPanelText.SetActive(false);
        _gameUIManager.QuitToHubPanelText.SetActive(true);

        _gameUIManager.ConfirmMainMenuButton.gameObject.SetActive(false);
        _gameUIManager.ConfirmHubButton.gameObject.SetActive(true);

        _currentState.SwitchState(_currentState.GameManagerStateFactory.GamePauseState());
    }
    #endregion

    #region SCORE STATE
    public void StartScoreState()
    {
        _currentState = _currentState.GameManagerStateFactory.GameScoreState();
        _exitState = _currentState.GameManagerStateFactory.GameHubState();
        _currentState.EnterState();
    }
    public void OnEnterScoreState()
    {
        _gameUIManager.ScorePanel.SetActive(true);

        ScoreManager.SetScoreManager();

        _gameSceneLoader.TargetScene = SceneIdentifier.Level_Hub;
    }
    public void OnExitScoreState()
    {
        _gameUIManager.ScorePanel.SetActive(false);
        LoadLevel = false;
        _gameUIManager.ConfirmActionButton.onClick.RemoveAllListeners();
        _gameUIManager.ConfirmActionButton.gameObject.SetActive(false);
        _gameUIManager.SetConfirmAction();
        _gameSaveSystem.SaveGame();
        ScoreManager.ResetPlayerScorePoints();
    }
    #endregion

    private void RegisterServices()
    {
        ServiceLocator.AudioManager = _gameAudioManager;

        ScoreManager = new GameScoreManager();
        ServiceLocator.ScoreManager = ScoreManager;

        ServiceLocator.UIManager = _gameUIManager;
        ServiceLocator.GameFlowManager = this;
        ServiceLocator.SaveSystem = _gameSaveSystem;
    }
    private void InstantiateLevelManagers()
    {
        GameLevelsRuntimeData.Clear();

        foreach (GameLevelConfig config in _gameLevelConfigs)
        {
            GameLevelRuntimeData runtimeData = new GameLevelRuntimeData
            {
                State = ELevelState.Closed,
                LevelSceneName = config.LevelSceneName,
                Config = config,
                CurrentGemScore = 0,
                CurrentHourglassScore = 0,
                MaxGemScoreReached = 0,
                MaxHourglassScoreReached = 0,
                MaxLevelScoreReached = 0,
                ClassficationTierReached = EClassficationTier.None
            };
            GameLevelsRuntimeData.Add(runtimeData);
        }
    }
    private void StartDevelopmentEnvironment()
    {
        RegisterServices();

        _transitionScreen.Initialize();

        _gameContextAudiolistener.enabled = false;

        _gameUIManager.Initialize(this);

        ScoreManager.Initialize(this, false);

        _characterContextManager = FindAnyObjectByType<CharacterContextManager>();

        _cameraBehaviourController = FindAnyObjectByType<CameraBehaviourController>();

        _playerInputManager = new PlayerInputManager(this, _characterContextManager, _cameraBehaviourController, new PlayerInputActions());

        _characterContextManager?.InitializeCharacterContextManager(_playerInputManager, _cameraBehaviourController, false);

        _playerInputManager.Initialize();

        _characterContextManager.EnableCharacterContext();
    }
    private void StartGameContextEnvironment()
    {
        RegisterServices();

        _transitionScreen.Initialize();

        ScoreManager.Initialize(this);

        _gameUIManager.Initialize(this);

        _gameSaveSystem.Initialize(this);

        _currentState = new GameManagerStateFactory(this).GameMainMenuState();

        GameEventsManager.OnPauseStateChanged.AddListener((value) =>
        {
            if (_gameSceneLoader.TargetScene != SceneIdentifier.Level_Hub)
            {
                SetTimer = value;
            }

            _cameraBehaviourController.enabled = value;
        });

        DontDestroyOnLoad(gameObject);
    }
}
