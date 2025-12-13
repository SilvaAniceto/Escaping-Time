using Esper.ESave;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(SaveStorage))]
public class GameSaveSystem : MonoBehaviour
{
    #region STATIC FIELDS
    private static PlayerProfileData _profileData;
    public static PlayerProfileData ProfileData
    {
        get
        {
            if (_profileData == null)
            {
                _profileData = new PlayerProfileData();
                return _profileData;
            }

            return _profileData;
        }

        private set
        {
            _profileData = value;
        }
    }
    #endregion

    #region INTERNAL CLASSES
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
    [System.Serializable]
    public class PlayerProfileData
    {
        public string ProfileName;
        public bool CharacterHasDash;
        public bool CharacterHasAirJump;
        public bool CharacterHasWallMove;
        public int MasterScore;
        public GameLevelManager[] GameLevelManager;

        public PlayerProfileData(string profileName, bool characterHasDash, bool characterHasAirJump, bool characterHasWallMove, int masterScore, GameLevelManager[] gameLevelManager)
        {
            this.ProfileName = profileName;
            this.CharacterHasDash = characterHasDash;
            this.CharacterHasAirJump = characterHasAirJump;
            this.CharacterHasWallMove = characterHasWallMove;
            this.MasterScore = masterScore;
            this.GameLevelManager = gameLevelManager;
        }

        public PlayerProfileData() { }
    }
    #endregion

    #region INSPECTOR FIELD
    [Header("Save File Setup")]
    [SerializeField] private SaveFileSetup _saveFileSetup;

    [Header("Save Slot Object")]
    [SerializeField] private GameSaveSlot[] _saveSlots;

    [Header("Save Options Objects")]
    [SerializeField] private GameObject _savingScreen;
    [SerializeField] private GameObject _optionsParent;
    [SerializeField] private Button _selectSaveButton;
    [SerializeField] private Button _deleteButton;
    #endregion

    #region PRIVATE FIELDS
    private int _currentSlotIndex;
    private string _currentProfile;
    private SaveFile _currentSaveFile;

    private const string PLAYER_PROFILE = "Player_ProfileData";
    #endregion

    #region PROPERTIES
    public bool SlotIsSelected { get; private set; }
    public Button SelectSaveButton { get => _selectSaveButton; }
    public GameSaveSlot[] SaveSlots { get => _saveSlots; }
    #endregion

    #region CALLBACKS
    [HideInInspector] public UnityEvent OnLaunchGame = new UnityEvent();
    #endregion

    #region UNITY DEFAULT METHODS
    private void Awake()
    {
        for (int i = 0; i < _saveSlots.Length; i++)
        {
            _saveSlots[i].SetSlotIndex(i);
        }

        if (SaveStorage.instance.saveCount == 0)
        {
            SetSaveSlots();
        }
    }
    private void Start ()
    {
        _deleteButton.onClick.AddListener(() =>
        {
            GameAudioManager.Instance.PlaySFX("Menu_Back");

            System.Action action = () =>
            {
                DeleteSaveGame();
                HideOptions();
                GameContextManager.Instance.GameManagerEventSystem.SetSelectedGameObject(_saveSlots[0].slotButton.gameObject);
                _currentProfile = null;
            };

            GameContextManager.Instance.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Back"));
        });
    }
    private void OnEnable()
    {
        CheckSaveSlots(false);
    }
    private void OnDestroy()
    {
        _selectSaveButton.onClick.RemoveAllListeners();
        _deleteButton.onClick.RemoveAllListeners();
        foreach (var item in _saveSlots)
        {
            item.slotButton.onClick.RemoveAllListeners();
        }
    }
    #endregion

    #region SAVE SLOTS METHODS
    private void SetSaveSlots()
    {
        _saveFileSetup.GenerateAESTokens();

        for (int i = 0; i < _saveSlots.Length; i++)
        {
            SaveFileSetupData saveFileSetupData = new SaveFileSetupData
            {
                fileName = $"Profile 0{(i + 1).ToString()}",
            #if UNITY_EDITOR
                saveLocation = SaveFileSetupData.SaveLocation.DataPath,
            #else
                saveLocation = SaveFileSetupData.SaveLocation.PersistentDataPath,
            #endif
                filePath = _saveFileSetup.saveFileData.filePath,
                fileType = _saveFileSetup.saveFileData.fileType,
                encryptionMethod = SaveFileSetupData.EncryptionMethod.AES,
                aesKey = _saveFileSetup.saveFileData.aesKey,
                aesIV = _saveFileSetup.saveFileData.aesIV,
                addToStorage = true,
                backgroundTask = _saveFileSetup.saveFileData.backgroundTask,
            };

            SaveFile saveFile = new SaveFile(saveFileSetupData);
        }
    }
    private void CheckSaveSlots(bool selectObject = true)
    {
        for (int i = 0; i < _saveSlots.Length; i++)
        {
            GameSaveSlot slot = _saveSlots[i];

            SaveFile save = SaveStorage.instance.GetSaveAtIndex(slot.GetSlotIndex());

            if (save != null)
            {
                if (save.HasData(PLAYER_PROFILE))
                {
                    PlayerProfileData data = save.GetData<PlayerProfileData>(PLAYER_PROFILE);

                    slot.SetLabelText(data.ProfileName);

                    slot.slotButton.onClick.RemoveAllListeners();
                    slot.slotButton.onClick.AddListener(() => 
                    {
                        GameAudioManager.Instance.PlaySFX("Menu_Click");

                        _currentSlotIndex = slot.GetSlotIndex();
                        _selectSaveButton.GetComponentInChildren<Text>().text = "Continue";

                        _currentProfile = data.ProfileName;

                        GameContextManager.Instance.GameManagerEventSystem.SetSelectedGameObject(_selectSaveButton.gameObject);

                        System.Action action = () =>
                        {
                            ShowOptions();
                            slot.slotButton.gameObject.SetActive(true);
                            slot.slotButton.interactable = false;
                            _deleteButton.gameObject.SetActive(true);
                        };

                        GameContextManager.Instance.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Click"));

                        _selectSaveButton.onClick.RemoveAllListeners();
                        _selectSaveButton.onClick.AddListener(() =>
                        {
                            HideOptions(true);

                            GameContextManager.Instance.BackButton.gameObject.SetActive(false);

                            GameAudioManager.Instance.PlaySFX("Menu_Start");

                            GameAudioManager.Instance.StopFadedBGM(0.0f, 1.5f);

                            System.Action action = () =>
                            {
                                LoadAndLaunch();
                            };

                            GameAudioManager.Instance.StopFadedBGM(0.0f, 1.5f);

                            GameContextManager.Instance.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Start"));
                        });
                    });
                }
                else
                {
                    SetNewGameSlot(slot);
                }
            }
            else
            {
                SetNewGameSlot(slot);
            }
        }

        if (selectObject) 
        {
            _saveSlots[_currentSlotIndex].slotButton.gameObject.SetActive(selectObject);
            _saveSlots[_currentSlotIndex].SetLabelText(_currentProfile);
        }
    }
    private void SetNewGameSlot(GameSaveSlot slot)
    {
        slot.SetLabelText("New Game");

        slot.slotButton.onClick.RemoveAllListeners();
        slot.slotButton.onClick.AddListener(() =>
        {
            GameAudioManager.Instance.PlaySFX("Menu_Click");

            _currentSlotIndex = slot.GetSlotIndex();
            _selectSaveButton.GetComponentInChildren<Text>().text = "Start";
            _deleteButton.gameObject.SetActive(false);

            System.Action action = () =>
            {
                ShowOptions();
                slot.slotButton.gameObject.SetActive(true);
                slot.slotButton.interactable = false;
                slot.SetLabelText($"Profile 0{_currentSlotIndex + 1}");
                _currentProfile = $"Profile 0{_currentSlotIndex + 1}";
                GameContextManager.Instance.GameManagerEventSystem.SetSelectedGameObject(_selectSaveButton.gameObject);
            };

            GameContextManager.Instance.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Click"));

            _selectSaveButton.onClick.RemoveAllListeners();
            _selectSaveButton.onClick.AddListener(() =>
            {
                GameAudioManager.Instance.PlaySFX("Menu_Start");
                GameContextManager.Instance.BackButton.gameObject.SetActive(false);

                GameAudioManager.Instance.StopFadedBGM(0.0f, 1.5f);

                if (string.IsNullOrEmpty(_currentProfile))
                {
                    _currentProfile = $"Profile 0{_currentSlotIndex + 1}";
                }

                HideOptions(true);

                System.Action action = () =>
                {
                    CreateSaveGame();
                };

                GameContextManager.Instance.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Start"));
            });
        });
    }
    #endregion

    #region OPTIONS METHODS
    public void ShowSlots()
    {
        for (int i = 0; i < _saveSlots.Length; i++)
        {
            _saveSlots[i].slotButton.gameObject.SetActive(true);
        }
    }
    public void ShowOptions()
    {
        SlotIsSelected = true;

        _optionsParent.gameObject.SetActive(true);

        for (int i = 0; i < _saveSlots.Length; i++)
        {
            _saveSlots[i].slotButton.gameObject.SetActive(false);
        }
    }
    public void HideOptions(bool selectObject = false)
    {
        SlotIsSelected = false;

        _optionsParent.gameObject.SetActive(false);

        if (!selectObject)
        {
            _currentProfile = null;
        }

        for (int i = 0; i < _saveSlots.Length; i++)
        {
            _saveSlots[i].slotButton.gameObject.SetActive(!selectObject);
            _saveSlots[i].slotButton.interactable = true;
        }

        CheckSaveSlots(selectObject);
    }
    #endregion

    #region SAVES METHODS
    private void CreateSaveGame()
    {
        foreach (GameLevelManager level in GameContextManager.Instance.GameLevelManager)
        {
            //level.State = GameLevelManager.EState.Closed;
            level.ClassficationTierReached = EClassficationTier.None;
            level.CurrentGemScore = 0;
            level.CurrentHourglassScore = 0;
            level.MaxGemScoreReached = 0;
            level.MaxHourglassScoreReached = 0;
            level.MaxLevelScoreReached = 0;
        }

        _currentSaveFile = SaveStorage.instance.GetSaveAtIndex(_currentSlotIndex);

        _currentSaveFile.AddOrUpdateData(PLAYER_PROFILE, CreateProfileData(_currentProfile));

        _currentSaveFile.Save();

        PlayerProfileData data = _currentSaveFile.GetData<PlayerProfileData>(PLAYER_PROFILE);

        UpdateLoadedProfileData(data);

        OnLaunchGame?.Invoke();
    }
    private void LoadAndLaunch()
    {
        LoadGame();

        OnLaunchGame?.Invoke();
    }
    public void LoadGame()
    {
        _currentSaveFile = SaveStorage.instance.GetSaveAtIndex(_currentSlotIndex);

        var data = _currentSaveFile.GetData<PlayerProfileData>(PLAYER_PROFILE);

        _currentProfile = data.ProfileName;

        UpdateLoadedProfileData(data);
    }
    public void LoadProfileDataToContext()
    {
        GameContextManager.Instance.CharacterContextManager.HasInfinityAirJump = ProfileData.CharacterHasAirJump;
        GameContextManager.Instance.CharacterContextManager.HasInfinityDash = ProfileData.CharacterHasDash;
        GameContextManager.Instance.CharacterContextManager.HasInfinityWallMove = ProfileData.CharacterHasWallMove;
        GameContextManager.Instance.ScoreManager.MasterScore = ProfileData.MasterScore;

        for (int i = 0; i < GameContextManager.Instance.GameLevelManager.Length; i++)
        {
            GameContextManager.Instance.GameLevelManager[i].State = ProfileData.GameLevelManager[i].State;
            GameContextManager.Instance.GameLevelManager[i].CurrentGemScore = ProfileData.GameLevelManager[i].CurrentGemScore;
            GameContextManager.Instance.GameLevelManager[i].CurrentHourglassScore = ProfileData.GameLevelManager[i].CurrentHourglassScore;
            GameContextManager.Instance.GameLevelManager[i].MaxGemScoreReached = ProfileData.GameLevelManager[i].MaxGemScoreReached;
            GameContextManager.Instance.GameLevelManager[i].MaxHourglassScoreReached = ProfileData.GameLevelManager[i].MaxHourglassScoreReached;
            GameContextManager.Instance.GameLevelManager[i].MaxLevelScoreReached = ProfileData.GameLevelManager[i].MaxLevelScoreReached;
            GameContextManager.Instance.GameLevelManager[i].ClassficationTierReached = ProfileData.GameLevelManager[i].ClassficationTierReached;
        }
    }
    public void SaveGame()
    {
        StartCoroutine(SaveGameAsync());
    }
    private IEnumerator SaveGameAsync()
    {
        _savingScreen.SetActive(true);

        PrepareProfileDataToSave(_currentProfile, GameContextManager.Instance.CharacterContextManager, GameContextManager.Instance.ScoreManager);

        _currentSaveFile = SaveStorage.instance.GetSaveAtIndex(_currentSlotIndex);

        _currentSaveFile.AddOrUpdateData(PLAYER_PROFILE, GetProfileData());

        _currentSaveFile.Save();

        yield return new WaitForSeconds(3);

        _savingScreen.SetActive(false);
    }
    public void DeleteSaveGame()
    {
        _currentSaveFile = SaveStorage.instance.GetSaveAtIndex(_currentSlotIndex);

        _currentSaveFile.DeleteData(PLAYER_PROFILE);

        _currentSaveFile.Save();
    }
    #endregion

    #region PROFILE DATA MANAGEMENT
    private void PrepareProfileDataToSave(string profile, CharacterContextManager characterContextManager, GameScoreManager scoreManager)
    {
        ProfileData.ProfileName = profile;
        ProfileData.CharacterHasAirJump = characterContextManager.HasInfinityAirJump;
        ProfileData.CharacterHasDash = characterContextManager.HasInfinityDash;
        ProfileData.CharacterHasWallMove = characterContextManager.HasInfinityWallMove;
        ProfileData.MasterScore = scoreManager.MasterScore;
        ProfileData.GameLevelManager = GameContextManager.Instance.GameLevelManager;
    }
    private void UpdateLoadedProfileData(PlayerProfileData data)
    {
        ProfileData.ProfileName = data.ProfileName;
        ProfileData.CharacterHasAirJump = data.CharacterHasAirJump;
        ProfileData.CharacterHasDash = data.CharacterHasDash;
        ProfileData.CharacterHasWallMove = data.CharacterHasWallMove;
        ProfileData.MasterScore = data.MasterScore;
        ProfileData.GameLevelManager = data.GameLevelManager;
    }
    private PlayerProfileData CreateProfileData(string profileName)
    {
        ProfileData = new PlayerProfileData(profileName, false, false, false, 0, GameContextManager.Instance.GameLevelManager);

        return ProfileData;
    }
    private PlayerProfileData GetProfileData()
    {
        return ProfileData;
    }
    #endregion
}
