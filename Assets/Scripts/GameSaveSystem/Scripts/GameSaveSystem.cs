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
    public static GameSaveSystem Instance;

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
                GameUIManager.Instance.BackButton.onClick.RemoveAllListeners();
                GameUIManager.Instance.BackButton.onClick.AddListener(() =>
                {
                    GameAudioManager.Instance.PlaySFX("Menu_Back");

                    Action action = () =>
                    {
                        HideOptions();
                    };

                    _gameContextManager.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Back"));
                });
            }
            else
            {
                GameUIManager.Instance.BackButton.onClick.RemoveAllListeners();
                GameUIManager.Instance.BackButton.onClick.AddListener(() =>
                {
                    GameAudioManager.Instance.PlaySFX("Menu_Back");

                    Action action = () =>
                    {
                        _gameContextManager.CurrentState.SwitchState(_gameContextManager.CurrentState.GameManagerStateFactory.GameMainMenuState());
                    };

                    _gameContextManager.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Back"));
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
        if (Instance ==  null)
        {
            Instance = this;
        }
        _gameContextManager = gameContextManager;

        for (int i = 0; i < GameUIManager.Instance.SaveSlots.Length; i++)
        {
            GameUIManager.Instance.SaveSlots[i].SetSlotIndex(i);
        }

        if (SaveStorage.instance.saveCount == 0)
        {
            SetSaveSlots();
        }

        GameUIManager.Instance.DeleteButton.onClick.AddListener(() =>
        {
            GameAudioManager.Instance.PlaySFX("Menu_Back");

            Action action = () =>
            {
                DeleteSaveGame();
                HideOptions();
                _gameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameUIManager.Instance.SaveSlots[0].slotButton.gameObject);
                _currentProfile = null;
            };

            _gameContextManager.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Back"));
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

                _gameContextManager.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Start"));
            });

            GameStateTransitionManager.FadeOut();
        });
    }
    private void OnDestroy()
    {
        if (!GameUIManager.Instance)
        {
            return;
        }

        foreach (var item in GameUIManager.Instance.SaveSlots)
        {
            item.slotButton.onClick.RemoveAllListeners();
        }
    }
    #endregion

    #region SAVE SLOTS METHODS
    private void SetSaveSlots()
    {
        _saveFileSetup.GenerateAESTokens();

        for (int i = 0; i < GameUIManager.Instance.SaveSlots.Length; i++)
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
        for (int i = 0; i < GameUIManager.Instance.SaveSlots.Length; i++)
        {
            GameUIManager.GameSaveSlot slot = GameUIManager.Instance.SaveSlots[i];

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
                        GameUIManager.Instance.SelectSaveButton.GetComponentInChildren<Text>().text = "Continue";

                        _currentProfile = data.ProfileName;

                        _gameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameUIManager.Instance.SelectSaveButton.gameObject);

                        Action action = () =>
                        {
                            ShowOptions();
                            slot.slotButton.gameObject.SetActive(true);
                            slot.slotButton.interactable = false;
                            GameUIManager.Instance.DeleteButton.gameObject.SetActive(true);
                        };

                        _gameContextManager.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Click"));

                        GameUIManager.Instance.SelectSaveButton.onClick.RemoveAllListeners();
                        GameUIManager.Instance.SelectSaveButton.onClick.AddListener(() =>
                        {
                            HideOptions(true);

                            GameUIManager.Instance.BackButton.gameObject.SetActive(false);

                            GameAudioManager.Instance.PlaySFX("Menu_Start");

                            GameAudioManager.Instance.StopFadedBGM(0.0f, 1.5f);

                            Action action = () =>
                            {
                                LoadAndLaunch();
                            };

                            GameAudioManager.Instance.StopFadedBGM(0.0f, 1.5f);

                            _gameContextManager.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Start"));
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
            GameUIManager.Instance.SaveSlots[_currentSlotIndex].slotButton.gameObject.SetActive(selectObject);
            GameUIManager.Instance.SaveSlots[_currentSlotIndex].SetLabelText(_currentProfile);
        }
    }
    private void SetNewGameSlot(GameUIManager.GameSaveSlot slot)
    {
        slot.SetLabelText("New Game");

        slot.slotButton.onClick.RemoveAllListeners();
        slot.slotButton.onClick.AddListener(() =>
        {
            GameAudioManager.Instance.PlaySFX("Menu_Click");

            _currentSlotIndex = slot.GetSlotIndex();
            GameUIManager.Instance.SelectSaveButton.GetComponentInChildren<Text>().text = "Start";
            GameUIManager.Instance.DeleteButton.gameObject.SetActive(false);

            Action action = () =>
            {
                ShowOptions();
                slot.slotButton.gameObject.SetActive(true);
                slot.slotButton.interactable = false;
                slot.SetLabelText($"Profile 0{_currentSlotIndex + 1}");
                _currentProfile = $"Profile 0{_currentSlotIndex + 1}";
                _gameContextManager.GameManagerEventSystem.SetSelectedGameObject(GameUIManager.Instance.SelectSaveButton.gameObject);
            };

            _gameContextManager.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Click"));

            GameUIManager.Instance.SelectSaveButton.onClick.RemoveAllListeners();
            GameUIManager.Instance.SelectSaveButton.onClick.AddListener(() =>
            {
                GameAudioManager.Instance.PlaySFX("Menu_Start");
                GameUIManager.Instance.BackButton.gameObject.SetActive(false);

                GameAudioManager.Instance.StopFadedBGM(0.0f, 1.5f);

                if (string.IsNullOrEmpty(_currentProfile))
                {
                    _currentProfile = $"Profile 0{_currentSlotIndex + 1}";
                }

                HideOptions(true);

                Action action = () =>
                {
                    CreateSaveGame();
                };

                _gameContextManager.WaitSeconds(action, GameAudioManager.Instance.AudioClipLength("Menu_Start"));
            });
        });
    }
    #endregion

    #region OPTIONS METHODS
    public void ShowSlots()
    {
        for (int i = 0; i < GameUIManager.Instance.SaveSlots.Length; i++)
        {
            GameUIManager.Instance.SaveSlots[i].slotButton.gameObject.SetActive(true);
        }

        CheckSaveSlots(false);
    }
    public void ShowOptions()
    {
        SlotIsSelected = true;

        GameUIManager.Instance.OptionsParent.gameObject.SetActive(true);

        for (int i = 0; i < GameUIManager.Instance.SaveSlots.Length; i++)
        {
            GameUIManager.Instance.SaveSlots[i].slotButton.gameObject.SetActive(false);
        }
    }
    public void HideOptions(bool selectObject = false)
    {
        SlotIsSelected = false;

        GameUIManager.Instance.OptionsParent.gameObject.SetActive(false);

        if (!selectObject)
        {
            _currentProfile = null;
        }

        for (int i = 0; i < GameUIManager.Instance.SaveSlots.Length; i++)
        {
            GameUIManager.Instance.SaveSlots[i].slotButton.gameObject.SetActive(!selectObject);
            GameUIManager.Instance.SaveSlots[i].slotButton.interactable = true;
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
        GameScoreManager.Instance.MasterScore = ProfileData.MasterScore;

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
        GameUIManager.Instance.SavingScreen.SetActive(true);

        PrepareProfileDataToSave(_currentProfile, _gameContextManager.CharacterContextManager, GameScoreManager.Instance);

        _currentSaveFile = SaveStorage.instance.GetSaveAtIndex(_currentSlotIndex);

        _currentSaveFile.AddOrUpdateData(PLAYER_PROFILE, GetProfileData());

        _currentSaveFile.Save();

        yield return new WaitForSeconds(3);

        GameUIManager.Instance.SavingScreen.SetActive(false);
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
