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
     
    public static UnityEvent<bool> OnRunOrPauseStateChanged = new UnityEvent<bool>();
    public static UnityEvent OnLoadSceneEnd = new UnityEvent();
    public static UnityEvent<GameContextManager> OnHubState = new UnityEvent<GameContextManager>();
    #endregion

    #region INSPECTOR FIELDS
    [Header("Enviroment Settings")]
    [SerializeField] private Environment _environment = Environment.GameContext;

    [Header("Playeable Character Set")]
    [SerializeField] private PlayeableCharacterSet _playeableCharacterSet;

    [Header("Game Level Manager")]
    [SerializeField] private GameLevelManager[] _gameLevelManagers;

    [Header("Game Save System")]
    [SerializeField] private GameSaveSystem _gameSaveSystem;

    [Header("Game UI Manager")]
    [SerializeField] private GameUIManager _gameUIManager;

    [Header("Game Audio Manager")]
    [SerializeField] private GameAudioManager _gameAudioManager;  
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

    public GameSaveSystem SaveSystem { get => _gameSaveSystem; }
    public GameUIManager GameUIManager { get => _gameUIManager; }
    public GameAudioManager GameAudioManager { get => _gameAudioManager; }
    public GameScoreManager ScoreManager { get => _gameScoreManager; }
    public List<GameLevelManager> GameLevelManagers { get; private set; } = new List<GameLevelManager>();    
    public string TargetScene { get; set; }

    public EventSystem GameManagerEventSystem {  get => EventSystem.current; }
    public AudioListener GameContextAudioListener { get => _gameContextAudiolistener; }

    public bool InstantiateCharacter { get => _characterContextManager == null && TargetScene != "MainMenu"; }
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
        }
#if !UNITY_EDITOR
        _environment = Environment.GameContext;
        Screen.SetResolution(1920, 1080, true);
        Application.targetFrameRate = 60;
#endif

        InstantiateLevelManagers();

        _gameContextAudiolistener = GetComponentInChildren<AudioListener>();

        _gameScoreManager = new GameScoreManager();

        _gameUIManager.Initialize(this);

        if (_environment == Environment.Development)
        {
            _characterContextManager = FindAnyObjectByType<CharacterContextManager>();

            _cameraBehaviourController = FindAnyObjectByType<CameraBehaviourController>();

            _playerInputManager = new PlayerInputManager(_characterContextManager, _cameraBehaviourController, new PlayerInputActions());

            _characterContextManager?.InitializeCharacterContextManager(_playerInputManager, _cameraBehaviourController, _gameAudioManager, false);

            _characterContextManager.SetPowerUpCallBack(_gameUIManager);

            _gameScoreManager.Initialize(this, false);

            _playerInputManager.Initialize();

            return;
        }

        _gameScoreManager.Initialize(this);

        _gameSaveSystem.Initialize(_gameUIManager, this);

        _currentState = new GameManagerStateFactory(this, _gameUIManager).GameMainMenuState();

        OnRunOrPauseStateChanged.AddListener((value) => 
        {
            if (TargetScene != "Level_Hub")
            {
                SetTimer = value; 
            }

            _cameraBehaviourController.enabled = value;
        });

        DontDestroyOnLoad(gameObject);
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
//    void OnGUI()
//    {
//        GUILayout.Label("FPS: " + Mathf.RoundToInt(1f / Time.deltaTime));
//#if UNITY_EDITOR
//        GUILayout.Label("Exit State: " + (ExitState == null ? "" : ExitState.ToString()));
//        GUILayout.Label("Current State: " + CurrentState.ToString());
//        GUILayout.Label("-----------------------------------------------");
//        if (CharacterContextManager != null)
//        {
//            GUILayout.Label("Current State: " + CharacterContextManager.CurrentState.ToString());
//            GUILayout.Label("Current Sub State: " + (CharacterContextManager.CurrentState.CurrentSubState != null ? CharacterContextManager.CurrentState.CurrentSubState.ToString() : ""));
//        }
//#endif
//    }
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

        _playerInputManager = new PlayerInputManager(_characterContextManager, _cameraBehaviourController, new PlayerInputActions());

        _characterContextManager.InitializeCharacterContextManager(_playerInputManager, _cameraBehaviourController, _gameAudioManager);

        _characterContextManager.SetPowerUpCallBack(_gameUIManager);

        CharacterHubStartPosition = Vector2.zero;

        GameStateTransitionManager.OnFadeInStart.AddListener(() =>
        {
            _characterContextManager.EnableCharacterContext();
        });

        OnRunOrPauseStateChanged.AddListener((value) =>
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
        Destroy(_cameraBehaviourController.gameObject);
        Destroy(_characterContextManager.gameObject);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
    
    public void InstantiateLevelManagers()
    {
        foreach (GameLevelManager levelManager in _gameLevelManagers)
        {
            ScriptableObject level = ScriptableObject.CreateInstance(typeof(GameLevelManager));

            ((GameLevelManager)level).State = GameLevelManager.EState.Closed;

            ((GameLevelManager)level).LevelSceneName = levelManager.LevelSceneName;

            ((GameLevelManager)level).LevelUnlockScore = levelManager.LevelUnlockScore;

            ((GameLevelManager)level).Tier3TargetScore = levelManager.Tier3TargetScore;
            ((GameLevelManager)level).Tier2TargetScore = levelManager.Tier2TargetScore;
            ((GameLevelManager)level).Tier1TargetScore = levelManager.Tier1TargetScore;

            ((GameLevelManager)level).ClassficationTierReached = EClassficationTier.None;

            ((GameLevelManager)level).MaxGemScore = levelManager.MaxGemScore;
            ((GameLevelManager)level).MaxHourglassScore = levelManager.MaxHourglassScore;

            ((GameLevelManager)level).CurrentGemScore = 0;
            ((GameLevelManager)level).CurrentHourglassScore = 0;

            ((GameLevelManager)level).MaxGemScoreReached = 0;
            ((GameLevelManager)level).MaxHourglassScoreReached = 0;
            ((GameLevelManager)level).MaxLevelScoreReached = 0;

            GameLevelManagers.Add((GameLevelManager)level);
        }
    }
    public void StartScoreState()
    {
        _currentState = new GameManagerStateFactory(this, _gameUIManager).GameScoreState();
        _exitState = new GameManagerStateFactory(this, _gameUIManager).GameHubState();
        _currentState.EnterState();
    }
}
