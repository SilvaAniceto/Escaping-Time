using Esper.ESave;
using System;
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
        public GameLevelRuntimeData[] GameLevelRuntimeData;

        public PlayerProfileData(string profileName, bool characterHasDash, bool characterHasAirJump, bool characterHasWallMove, int masterScore, GameLevelRuntimeData[] gameLevelRuntimeData)
        {
            this.ProfileName = profileName;
            this.CharacterHasDash = characterHasDash;
            this.CharacterHasAirJump = characterHasAirJump;
            this.CharacterHasWallMove = characterHasWallMove;
            this.MasterScore = masterScore;
            this.GameLevelRuntimeData = gameLevelRuntimeData;
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

    private int _currentSlotIndex;
    private string _currentProfile;
    private SaveFile _currentSaveFile;

    private bool _slotIsSelected;

    private const string PLAYER_PROFILE = "Player_ProfileData";
    #endregion

    #region PROPERTIES
    public bool SlotIsSelected 
    { 
        get
        {
            return _slotIsSelected;
        } 
        private set
        {
            if (_slotIsSelected == value)
            {
                return;
            }

            _slotIsSelected = value;

            if (_slotIsSelected)
            {
                _gameContextManager.UIManager.BackButton.onClick.RemoveAllListeners();
                _gameContextManager.UIManager.BackButton.onClick.AddListener(() =>
                {
                    _gameContextManager.AudioManager.PlaySFX("Menu_Back");

                    Action action = () =>
                    {
                        HideOptions();
                    };

                    _gameContextManager.WaitSeconds(action, _gameContextManager.AudioManager.AudioClipLength("Menu_Back"));
                });
            }
            else
            {
                _gameContextManager.UIManager.BackButton.onClick.RemoveAllListeners();
                _gameContextManager.UIManager.BackButton.onClick.AddListener(() =>
                {
                    _gameContextManager.AudioManager.PlaySFX("Menu_Back");

                    Action action = () =>
                    {
                        _gameContextManager.CurrentState.SwitchState(_gameContextManager.CurrentState.GameManagerStateFactory.GameMainMenuState());
                    };

                    _gameContextManager.WaitSeconds(action, _gameContextManager.AudioManager.AudioClipLength("Menu_Back"));
                });
            }
        }
    }
    
    #endregion

    #region CALLBACKS
    [HideInInspector] public UnityEvent OnLaunchGame = new UnityEvent();
    #endregion

    #region UNITY DEFAULT METHODS
    public void Initialize(GameContextManager gameContextManager)
    {
        _gameContextManager = gameContextManager;

        for (int i = 0; i < _gameContextManager.UIManager.SaveSlots.Length; i++)
        {
            _gameContextManager.UIManager.SaveSlots[i].SetSlotIndex(i);
        }

        if (SaveStorage.instance.saveCount == 0)
        {
            SetSaveSlots();
        }

        _gameContextManager.UIManager.DeleteButton.onClick.AddListener(() =>
        {
            _gameContextManager.AudioManager.PlaySFX("Menu_Back");

            Action action = () =>
            {
                DeleteSaveGame();
                HideOptions();
                _gameContextManager.GameManagerEventSystem.SetSelectedGameObject(_gameContextManager.UIManager.SaveSlots[0].slotButton.gameObject);
                _currentProfile = null;
            };

            _gameContextManager.WaitSeconds(action, _gameContextManager.AudioManager.AudioClipLength("Menu_Back"));
        });

        OnLaunchGame.RemoveAllListeners();
        OnLaunchGame.AddListener(() =>
        {
            GameStateTransitionManager.OnFadeOutEnd += (() =>
            {
                System.Action action = () =>
                {
                    _gameContextManager.CurrentState.SwitchState(_gameContextManager.CurrentState.GameManagerStateFactory.GameLoadingState());
                };

                _gameContextManager.WaitSeconds(action, _gameContextManager.AudioManager.AudioClipLength("Menu_Start"));
            });

            GameStateTransitionManager.FadeOut();
        });
    }
    private void OnDestroy()
    {
        if (!_gameContextManager || !_gameContextManager.UIManager)
        {
            return;
        }

        foreach (var item in _gameContextManager.UIManager.SaveSlots)
        {
            item.slotButton.onClick.RemoveAllListeners();
        }
    }
    #endregion

    #region SAVE SLOTS METHODS
    private void SetSaveSlots()
    {
        _saveFileSetup.GenerateAESTokens();

        for (int i = 0; i < _gameContextManager.UIManager.SaveSlots.Length; i++)
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
        for (int i = 0; i < _gameContextManager.UIManager.SaveSlots.Length; i++)
        {
            GameUIManager.GameSaveSlot slot = _gameContextManager.UIManager.SaveSlots[i];

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
                        _gameContextManager.AudioManager.PlaySFX("Menu_Click");

                        _currentSlotIndex = slot.GetSlotIndex();
                        _gameContextManager.UIManager.SelectSaveButton.GetComponentInChildren<Text>().text = "Continue";

                        _currentProfile = data.ProfileName;

                        _gameContextManager.GameManagerEventSystem.SetSelectedGameObject(_gameContextManager.UIManager.SelectSaveButton.gameObject);

                        Action action = () =>
                        {
                            ShowOptions();
                            slot.slotButton.gameObject.SetActive(true);
                            slot.slotButton.interactable = false;
                            _gameContextManager.UIManager.DeleteButton.gameObject.SetActive(true);
                        };

                        _gameContextManager.WaitSeconds(action, _gameContextManager.AudioManager.AudioClipLength("Menu_Click"));

                        _gameContextManager.UIManager.SelectSaveButton.onClick.RemoveAllListeners();
                        _gameContextManager.UIManager.SelectSaveButton.onClick.AddListener(() =>
                        {
                            HideOptions(true);

                            _gameContextManager.UIManager.BackButton.gameObject.SetActive(false);

                            _gameContextManager.AudioManager.PlaySFX("Menu_Start");

                            _gameContextManager.AudioManager.StopFadedBGM(0.0f, 1.5f);

                            Action action = () =>
                            {
                                LoadAndLaunch();
                            };

                            _gameContextManager.AudioManager.StopFadedBGM(0.0f, 1.5f);

                            _gameContextManager.WaitSeconds(action, _gameContextManager.AudioManager.AudioClipLength("Menu_Start"));
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
            _gameContextManager.UIManager.SaveSlots[_currentSlotIndex].slotButton.gameObject.SetActive(selectObject);
            _gameContextManager.UIManager.SaveSlots[_currentSlotIndex].SetLabelText(_currentProfile);
        }
    }
    private void SetNewGameSlot(GameUIManager.GameSaveSlot slot)
    {
        slot.SetLabelText("New Game");

        slot.slotButton.onClick.RemoveAllListeners();
        slot.slotButton.onClick.AddListener(() =>
        {
            _gameContextManager.AudioManager.PlaySFX("Menu_Click");

            _currentSlotIndex = slot.GetSlotIndex();
            _gameContextManager.UIManager.SelectSaveButton.GetComponentInChildren<Text>().text = "Start";
            _gameContextManager.UIManager.DeleteButton.gameObject.SetActive(false);

            Action action = () =>
            {
                ShowOptions();
                slot.slotButton.gameObject.SetActive(true);
                slot.slotButton.interactable = false;
                slot.SetLabelText($"Profile 0{_currentSlotIndex + 1}");
                _currentProfile = $"Profile 0{_currentSlotIndex + 1}";
                _gameContextManager.GameManagerEventSystem.SetSelectedGameObject(_gameContextManager.UIManager.SelectSaveButton.gameObject);
            };

            _gameContextManager.WaitSeconds(action, _gameContextManager.AudioManager.AudioClipLength("Menu_Click"));

            _gameContextManager.UIManager.SelectSaveButton.onClick.RemoveAllListeners();
            _gameContextManager.UIManager.SelectSaveButton.onClick.AddListener(() =>
            {
                _gameContextManager.AudioManager.PlaySFX("Menu_Start");
                _gameContextManager.UIManager.BackButton.gameObject.SetActive(false);

                _gameContextManager.AudioManager.StopFadedBGM(0.0f, 1.5f);

                if (string.IsNullOrEmpty(_currentProfile))
                {
                    _currentProfile = $"Profile 0{_currentSlotIndex + 1}";
                }

                HideOptions(true);

                Action action = () =>
                {
                    CreateSaveGame();
                };

                _gameContextManager.WaitSeconds(action, _gameContextManager.AudioManager.AudioClipLength("Menu_Start"));
            });
        });
    }
    #endregion

    #region OPTIONS METHODS
    public void ShowSlots()
    {
        for (int i = 0; i < _gameContextManager.UIManager.SaveSlots.Length; i++)
        {
            _gameContextManager.UIManager.SaveSlots[i].slotButton.gameObject.SetActive(true);
        }

        CheckSaveSlots(false);
    }
    public void ShowOptions()
    {
        SlotIsSelected = true;

        _gameContextManager.UIManager.OptionsParent.gameObject.SetActive(true);

        for (int i = 0; i < _gameContextManager.UIManager.SaveSlots.Length; i++)
        {
            _gameContextManager.UIManager.SaveSlots[i].slotButton.gameObject.SetActive(false);
        }
    }
    public void HideOptions(bool selectObject = false)
    {
        SlotIsSelected = false;

        _gameContextManager.UIManager.OptionsParent.gameObject.SetActive(false);

        if (!selectObject)
        {
            _currentProfile = null;
        }

        for (int i = 0; i < _gameContextManager.UIManager.SaveSlots.Length; i++)
        {
            _gameContextManager.UIManager.SaveSlots[i].slotButton.gameObject.SetActive(!selectObject);
            _gameContextManager.UIManager.SaveSlots[i].slotButton.interactable = true;
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
        gameContextManager.CharacterContextManager.PowerUpManager.HasInfinityAirJump = ProfileData.CharacterHasAirJump;
        gameContextManager.CharacterContextManager.PowerUpManager.HasInfinityDash = ProfileData.CharacterHasDash;
        gameContextManager.CharacterContextManager.PowerUpManager.HasInfinityWallMove = ProfileData.CharacterHasWallMove;
        gameContextManager.ScoreManager.MasterScore = ProfileData.MasterScore;

        for (int i = 0; i < gameContextManager.GameLevelsRuntimeData.Count; i++)
        {
            gameContextManager.GameLevelsRuntimeData[i].State = ProfileData.GameLevelRuntimeData[i].State;
            gameContextManager.GameLevelsRuntimeData[i].CurrentGemScore = ProfileData.GameLevelRuntimeData[i].CurrentGemScore;
            gameContextManager.GameLevelsRuntimeData[i].CurrentHourglassScore = ProfileData.GameLevelRuntimeData[i].CurrentHourglassScore;
            gameContextManager.GameLevelsRuntimeData[i].MaxGemScoreReached = ProfileData.GameLevelRuntimeData[i].MaxGemScoreReached;
            gameContextManager.GameLevelsRuntimeData[i].MaxHourglassScoreReached = ProfileData.GameLevelRuntimeData[i].MaxHourglassScoreReached;
            gameContextManager.GameLevelsRuntimeData[i].MaxLevelScoreReached = ProfileData.GameLevelRuntimeData[i].MaxLevelScoreReached;
            gameContextManager.GameLevelsRuntimeData[i].ClassficationTierReached = ProfileData.GameLevelRuntimeData[i].ClassficationTierReached;
        }
    }
    public void SaveGame()
    {
        StartCoroutine(SaveGameAsync());
    }
    private IEnumerator SaveGameAsync()
    {
        _gameContextManager.UIManager.SavingScreen.SetActive(true);

        PrepareProfileDataToSave(_currentProfile, _gameContextManager.CharacterContextManager, _gameContextManager.ScoreManager);

        _currentSaveFile = SaveStorage.instance.GetSaveAtIndex(_currentSlotIndex);

        _currentSaveFile.AddOrUpdateData(PLAYER_PROFILE, GetProfileData());

        _currentSaveFile.Save();

        yield return new WaitForSeconds(3);

        _gameContextManager.UIManager.SavingScreen.SetActive(false);
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
        ProfileData.CharacterHasAirJump = characterContextManager.PowerUpManager.HasInfinityAirJump;
        ProfileData.CharacterHasDash = characterContextManager.PowerUpManager.HasInfinityDash;
        ProfileData.CharacterHasWallMove = characterContextManager.PowerUpManager.HasInfinityWallMove;
        ProfileData.MasterScore = scoreManager.MasterScore;
        ProfileData.GameLevelRuntimeData = _gameContextManager.GameLevelsRuntimeData.ToArray();
    }
    private void UpdateLoadedProfileData(PlayerProfileData data)
    {
        ProfileData.ProfileName = data.ProfileName;
        ProfileData.CharacterHasAirJump = data.CharacterHasAirJump;
        ProfileData.CharacterHasDash = data.CharacterHasDash;
        ProfileData.CharacterHasWallMove = data.CharacterHasWallMove;
        ProfileData.MasterScore = data.MasterScore;
        ProfileData.GameLevelRuntimeData = data.GameLevelRuntimeData;
    }
    private PlayerProfileData CreateProfileData(string profileName)
    {
        ProfileData = new PlayerProfileData(profileName, false, false, false, 0, _gameContextManager.GameLevelsRuntimeData.ToArray());

        return ProfileData;
    }
    private PlayerProfileData GetProfileData()
    {
        return ProfileData;
    }
    #endregion
}
