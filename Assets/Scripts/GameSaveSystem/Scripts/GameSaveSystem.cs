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


    #endregion

    #region PRIVATE FIELDS
    private GameContextManager _gameContextManager;
    private GameUIManager _gameUIManager;

    private int _currentSlotIndex;
    private string _currentProfile;
    private SaveFile _currentSaveFile;

    private const string PLAYER_PROFILE = "Player_ProfileData";
    #endregion

    #region PROPERTIES
    public bool SlotIsSelected { get; private set; }
    
    #endregion

    #region CALLBACKS
    [HideInInspector] public UnityEvent OnLaunchGame = new UnityEvent();
    #endregion

    #region UNITY DEFAULT METHODS
    public void Initialize(GameUIManager gameUIManager, GameContextManager gameContextManager)
    {
        _gameContextManager = gameContextManager;
        _gameUIManager = gameUIManager;

        for (int i = 0; i < _gameUIManager.SaveSlots.Length; i++)
        {
            _gameUIManager.SaveSlots[i].SetSlotIndex(i);
        }

        if (SaveStorage.instance.saveCount == 0)
        {
            SetSaveSlots();
        }

        _gameUIManager.DeleteButton.onClick.AddListener(() =>
        {
            _gameContextManager.GameAudioManager.PlaySFX("Menu_Back");

            System.Action action = () =>
            {
                DeleteSaveGame();
                HideOptions();
                _gameContextManager.GameManagerEventSystem.SetSelectedGameObject(_gameUIManager.SaveSlots[0].slotButton.gameObject);
                _currentProfile = null;
            };

            _gameContextManager.WaitSeconds(action, _gameContextManager.GameAudioManager.AudioClipLength("Menu_Back"));
        });
    }
    private void OnDestroy()
    {
        if (!_gameUIManager)
        {
            return;
        }

        foreach (var item in _gameUIManager.SaveSlots)
        {
            item.slotButton.onClick.RemoveAllListeners();
        }
    }
    #endregion

    #region SAVE SLOTS METHODS
    private void SetSaveSlots()
    {
        _saveFileSetup.GenerateAESTokens();

        for (int i = 0; i < _gameUIManager.SaveSlots.Length; i++)
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
    public void CheckSaveSlots(bool selectObject = true)
    {
        for (int i = 0; i < _gameUIManager.SaveSlots.Length; i++)
        {
            GameUIManager.GameSaveSlot slot = _gameUIManager.SaveSlots[i];

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
                        _gameContextManager.GameAudioManager.PlaySFX("Menu_Click");

                        _currentSlotIndex = slot.GetSlotIndex();
                        _gameUIManager.SelectSaveButton.GetComponentInChildren<Text>().text = "Continue";

                        _currentProfile = data.ProfileName;

                        _gameContextManager.GameManagerEventSystem.SetSelectedGameObject(_gameUIManager.SelectSaveButton.gameObject);

                        System.Action action = () =>
                        {
                            ShowOptions();
                            slot.slotButton.gameObject.SetActive(true);
                            slot.slotButton.interactable = false;
                            _gameUIManager.DeleteButton.gameObject.SetActive(true);
                        };

                        _gameContextManager.WaitSeconds(action, _gameContextManager.GameAudioManager.AudioClipLength("Menu_Click"));

                        _gameUIManager.SelectSaveButton.onClick.RemoveAllListeners();
                        _gameUIManager.SelectSaveButton.onClick.AddListener(() =>
                        {
                            HideOptions(true);

                            _gameContextManager.GameUIManager.BackButton.gameObject.SetActive(false);

                            _gameContextManager.GameAudioManager.PlaySFX("Menu_Start");

                            _gameContextManager.GameAudioManager.StopFadedBGM(0.0f, 1.5f);

                            System.Action action = () =>
                            {
                                LoadAndLaunch();
                            };

                            _gameContextManager.GameAudioManager.StopFadedBGM(0.0f, 1.5f);

                            _gameContextManager.WaitSeconds(action, _gameContextManager.GameAudioManager.AudioClipLength("Menu_Start"));
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
            _gameUIManager.SaveSlots[_currentSlotIndex].slotButton.gameObject.SetActive(selectObject);
            _gameUIManager.SaveSlots[_currentSlotIndex].SetLabelText(_currentProfile);
        }
    }
    private void SetNewGameSlot(GameUIManager.GameSaveSlot slot)
    {
        slot.SetLabelText("New Game");

        slot.slotButton.onClick.RemoveAllListeners();
        slot.slotButton.onClick.AddListener(() =>
        {
            _gameContextManager.GameAudioManager.PlaySFX("Menu_Click");

            _currentSlotIndex = slot.GetSlotIndex();
            _gameUIManager.SelectSaveButton.GetComponentInChildren<Text>().text = "Start";
            _gameUIManager.DeleteButton.gameObject.SetActive(false);

            System.Action action = () =>
            {
                ShowOptions();
                slot.slotButton.gameObject.SetActive(true);
                slot.slotButton.interactable = false;
                slot.SetLabelText($"Profile 0{_currentSlotIndex + 1}");
                _currentProfile = $"Profile 0{_currentSlotIndex + 1}";
                _gameContextManager.GameManagerEventSystem.SetSelectedGameObject(_gameUIManager.SelectSaveButton.gameObject);
            };

            _gameContextManager.WaitSeconds(action, _gameContextManager.GameAudioManager.AudioClipLength("Menu_Click"));

            _gameUIManager.SelectSaveButton.onClick.RemoveAllListeners();
            _gameUIManager.SelectSaveButton.onClick.AddListener(() =>
            {
                _gameContextManager.GameAudioManager.PlaySFX("Menu_Start");
                _gameContextManager.GameUIManager.BackButton.gameObject.SetActive(false);

                _gameContextManager.GameAudioManager.StopFadedBGM(0.0f, 1.5f);

                if (string.IsNullOrEmpty(_currentProfile))
                {
                    _currentProfile = $"Profile 0{_currentSlotIndex + 1}";
                }

                HideOptions(true);

                System.Action action = () =>
                {
                    CreateSaveGame();
                };

                _gameContextManager.WaitSeconds(action, _gameContextManager.GameAudioManager.AudioClipLength("Menu_Start"));
            });
        });
    }
    #endregion

    #region OPTIONS METHODS
    public void ShowSlots()
    {
        for (int i = 0; i < _gameUIManager.SaveSlots.Length; i++)
        {
            _gameUIManager.SaveSlots[i].slotButton.gameObject.SetActive(true);
        }

        CheckSaveSlots(false);
    }
    public void ShowOptions()
    {
        SlotIsSelected = true;

        _gameUIManager.OptionsParent.gameObject.SetActive(true);

        for (int i = 0; i < _gameUIManager.SaveSlots.Length; i++)
        {
            _gameUIManager.SaveSlots[i].slotButton.gameObject.SetActive(false);
        }
    }
    public void HideOptions(bool selectObject = false)
    {
        SlotIsSelected = false;

        _gameUIManager.OptionsParent.gameObject.SetActive(false);

        if (!selectObject)
        {
            _currentProfile = null;
        }

        for (int i = 0; i < _gameUIManager.SaveSlots.Length; i++)
        {
            _gameUIManager.SaveSlots[i].slotButton.gameObject.SetActive(!selectObject);
            _gameUIManager.SaveSlots[i].slotButton.interactable = true;
        }

        CheckSaveSlots(selectObject);
    }
    #endregion

    #region SAVES METHODS
    private void CreateSaveGame()
    {
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
    public void LoadProfileDataToContext(GameContextManager gameContextManager)
    {
        gameContextManager.CharacterContextManager.HasInfinityAirJump = ProfileData.CharacterHasAirJump;
        gameContextManager.CharacterContextManager.HasInfinityDash = ProfileData.CharacterHasDash;
        gameContextManager.CharacterContextManager.HasInfinityWallMove = ProfileData.CharacterHasWallMove;
        gameContextManager.ScoreManager.MasterScore = ProfileData.MasterScore;

        for (int i = 0; i < gameContextManager.GameLevelManagers.Count; i++)
        {
            gameContextManager.GameLevelManagers[i].State = ProfileData.GameLevelManager[i].State;
            gameContextManager.GameLevelManagers[i].CurrentGemScore = ProfileData.GameLevelManager[i].CurrentGemScore;
            gameContextManager.GameLevelManagers[i].CurrentHourglassScore = ProfileData.GameLevelManager[i].CurrentHourglassScore;
            gameContextManager.GameLevelManagers[i].MaxGemScoreReached = ProfileData.GameLevelManager[i].MaxGemScoreReached;
            gameContextManager.GameLevelManagers[i].MaxHourglassScoreReached = ProfileData.GameLevelManager[i].MaxHourglassScoreReached;
            gameContextManager.GameLevelManagers[i].MaxLevelScoreReached = ProfileData.GameLevelManager[i].MaxLevelScoreReached;
            gameContextManager.GameLevelManagers[i].ClassficationTierReached = ProfileData.GameLevelManager[i].ClassficationTierReached;
        }
    }
    public void SaveGame()
    {
        StartCoroutine(SaveGameAsync());
    }
    private IEnumerator SaveGameAsync()
    {
        _gameUIManager.SavingScreen.SetActive(true);

        PrepareProfileDataToSave(_currentProfile, _gameContextManager.CharacterContextManager, _gameContextManager.ScoreManager);

        _currentSaveFile = SaveStorage.instance.GetSaveAtIndex(_currentSlotIndex);

        _currentSaveFile.AddOrUpdateData(PLAYER_PROFILE, GetProfileData());

        _currentSaveFile.Save();

        yield return new WaitForSeconds(3);

        _gameUIManager.SavingScreen.SetActive(false);
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
        ProfileData.GameLevelManager = _gameContextManager.GameLevelManagers.ToArray();
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
        ProfileData = new PlayerProfileData(profileName, false, false, false, 0, _gameContextManager.GameLevelManagers.ToArray());

        return ProfileData;
    }
    private PlayerProfileData GetProfileData()
    {
        return ProfileData;
    }
    #endregion
}
