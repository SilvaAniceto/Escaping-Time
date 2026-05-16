using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameContextManager : MonoBehaviour
{
    public enum Environment
    {
        Development,
        GameContext
    }

    #region STATIC FIELDS
    public static GameContextManager Instance { get; private set; }

    public static UnityEvent OnLoadSceneEnd = new UnityEvent();
    #endregion

    #region INSPECTOR FIELDS
    [Header("Enviroment Settings")]
    [SerializeField] private Environment _environment = Environment.GameContext;

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

    [Header("Debug Settings")]
    [SerializeField] private bool _debugOnGui = false;
    #endregion

    #region PRIVATE FIELDS
    private AudioListener _gameContextAudiolistener;

    private GameScoreManager _gameScoreManager;

    private GameManagerAbstractState _exitState;
    private GameManagerAbstractState _currentState;

    private CharacterContextManager _characterContextManager;
    private CameraBehaviourController _cameraBehaviourController;

    private PlayerInputManager _playerInputManager;
    #endregion

    #region PROPERTIES
    public PlayeableCharacterSet PlayeableCharacterSet { get => _playeableCharacterSet; }
    public GameManagerAbstractState ExitState { get { return _exitState; } set { _exitState = value; } }
    public GameManagerAbstractState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public CharacterContextManager CharacterContextManager { get => _characterContextManager; }
    public Vector2 CharacterHubStartPosition { get; set; }

    public PlayerInputManager PlayerInputManager { get => _playerInputManager; }

    public List<GameLevelRuntimeData> GameLevelsRuntimeData { get; private set; } = new List<GameLevelRuntimeData>();
    public SceneIdentifier TargetScene { get; set; }

    public EventSystem GameManagerEventSystem {  get => EventSystem.current; }

    public bool InstantiateCharacter { get => _characterContextManager == null && TargetScene != SceneIdentifier.MainMenu; }
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

        InstantiateLevelManagers();

        switch (_environment)
        {
            case Environment.Development:
                StartDevelopmentEnvironment();
                break;
            case Environment.GameContext:
                StartGameContextEnvironment();
                break;
        }

        GameEventsManager.OnSceneLoadRequested.AddListener(OnSceneLoadRequested);
    }
    private void Start()
    {
        if (_environment == Environment.Development) return;

        _currentState.EnterState();
    }
    private void Update()
    {
        _cameraBehaviourController?.CameraVerticalOffset();
        _playerInputManager?.UpdatePlayerInputManager();

        if (_environment == Environment.GameContext)
        {
            _currentState.UpdateStates();
        }

        if (SetTimer)
        {
            _gameScoreManager.SetCurrentTimer();
        }
    }
    private void OnDestroy()
    {
        GameEventsManager.OnSceneLoadRequested.RemoveListener(OnSceneLoadRequested);
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
    private void OnSceneLoadRequested(SceneIdentifier sceneId)
    {
        TargetScene = sceneId;
    }
    public void AfterLoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
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

        OnLoadSceneEnd?.Invoke();
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
    public void OnQuitToMainMenu()
    {
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

        _gameUIManager.CharacterUIManager.gameObject.SetActive(false);
        _gameUIManager.LoadingScreen.SetActive(true);

        SceneManager.LoadSceneAsync(TargetScene.ToString());

        OnLoadSceneEnd.RemoveAllListeners();
        OnLoadSceneEnd.AddListener(() =>
        {
            WaitFrameEnd(() =>
            {
                _currentState.SwitchState(_exitState);
            });
        });

        SceneManager.sceneLoaded += AfterLoadSceneEnd;

        GameManagerEventSystem.SetSelectedGameObject(null);
    }
    public void OnExitLoadingState()
    {
        SceneManager.sceneLoaded -= AfterLoadSceneEnd;
        _gameUIManager.LoadingScreen.SetActive(false);
        GameStateTransitionManager.FadeIn();
    }
    #endregion

    #region HUB STATE
    public void OnEnterHubState()
    {
        _gameUIManager.SetHubUIObjects();

        _gameUIManager.SetScoreDisplay(_gameScoreManager.MasterScore);

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

        _gameUIManager.CharacterUIManager.gameObject.SetActive(true);

        _gameUIManager.SetScoreDisplay(_gameScoreManager.CurrentScore);

        GameManagerEventSystem.SetSelectedGameObject(null);
    }
    public void OnExitRunState()
    {
        _gameUIManager.CharacterUIManager.gameObject.SetActive(false);
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

        _gameScoreManager.SetScoreManager();

        TargetScene = SceneIdentifier.Level_Hub;
    }
    public void OnExitScoreState()
    {
        _gameUIManager.ScorePanel.SetActive(false);
        LoadLevel = false;
        _gameUIManager.ConfirmActionButton.onClick.RemoveAllListeners();
        _gameUIManager.ConfirmActionButton.gameObject.SetActive(false);
        _gameUIManager.SetConfirmAction();
        _gameSaveSystem.SaveGame();
        _gameScoreManager.ResetPlayerScorePoints();
    }
    #endregion

    private void InstantiateLevelManagers()
    {
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
        _transitionScreen.Initialize();

        _gameContextAudiolistener.enabled = false;

        _gameScoreManager = new GameScoreManager();

        _gameAudioManager.Initialize();

        _gameUIManager.Initialize(this);

        _gameScoreManager.Initialize(this, false);

        _characterContextManager = FindAnyObjectByType<CharacterContextManager>();

        _cameraBehaviourController = FindAnyObjectByType<CameraBehaviourController>();

        _playerInputManager = new PlayerInputManager(this, _characterContextManager, _cameraBehaviourController, new PlayerInputActions());

        _characterContextManager?.InitializeCharacterContextManager(_playerInputManager, _cameraBehaviourController, false);

        _playerInputManager.Initialize();

        _characterContextManager.EnableCharacterContext();
    }
    private void StartGameContextEnvironment()
    {
        _transitionScreen.Initialize();

        _gameScoreManager = new GameScoreManager();

        _gameScoreManager.Initialize(this);

        _gameAudioManager.Initialize();

        _gameUIManager.Initialize(this);

        _gameSaveSystem.Initialize(this);

        _currentState = new GameManagerStateFactory(this).GameMainMenuState();

        GameEventsManager.OnPauseStateChanged.AddListener((value) =>
        {
            if (TargetScene != SceneIdentifier.Level_Hub)
            {
                SetTimer = value;
            }

            _cameraBehaviourController.enabled = value;
        });

        DontDestroyOnLoad(gameObject);
    }
}
