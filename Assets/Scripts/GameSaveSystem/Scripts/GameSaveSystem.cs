using Esper.ESave;
using Esper.ESave.SavableObjects;
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
        public InputField inputField;

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
        public SavableVector CharacterSpawningPosition;
        public bool CharacterHasDash;
        public bool CharacterHasAirJump;
        public bool CharacterHasWallMove;

        public PlayerProfileData(string profileName, SavableVector characterSpawningPosition, bool characterHasDash, bool characterHasAirJump, bool characterHasWallMove)
        {
            this.ProfileName = profileName;
            this.CharacterSpawningPosition = characterSpawningPosition;
            this.CharacterHasDash = characterHasDash;
            this.CharacterHasAirJump = characterHasAirJump;
            this.CharacterHasWallMove = characterHasWallMove;
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
            DeleteSaveGame();
            HideOptions();
        });
    }
    private void OnEnable()
    {
        CheckSaveSlots();
    }
    private void OnDestroy()
    {
        _selectSaveButton.onClick.RemoveAllListeners();
        _deleteButton.onClick.RemoveAllListeners();
        foreach (var item in _saveSlots)
        {
            item.slotButton.onClick.RemoveAllListeners();
            item.inputField.onSubmit.RemoveAllListeners();
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
                fileName = $"Profile_0{(i + 1).ToString()}",
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
                addToStorage = true
            };

            SaveFile saveFile = new SaveFile(saveFileSetupData);
        }
    }
    private void CheckSaveSlots()
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
                        ShowOptions();

                        _currentSlotIndex = slot.GetSlotIndex();
                        _selectSaveButton.GetComponentInChildren<Text>().text = "Continue";
                        _deleteButton.gameObject.SetActive(true);

                        slot.slotButton.gameObject.SetActive(true);
                        slot.slotButton.interactable = false;

                        GameManagerContext.Instance.GameManagerEventSystem.SetSelectedGameObject(_selectSaveButton.gameObject);

                        _selectSaveButton.onClick.RemoveAllListeners();
                        _selectSaveButton.onClick.AddListener(() =>
                        {
                            LoadAndLaunch();
                            HideOptions();
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

        GameManagerContext.Instance.GameManagerEventSystem.SetSelectedGameObject(_saveSlots[0].slotButton.gameObject);
    }
    private void SetNewGameSlot(GameSaveSlot slot)
    {
        slot.SetLabelText("New Game");

        slot.slotButton.onClick.RemoveAllListeners();
        slot.slotButton.onClick.AddListener(() =>
        {
            ShowOptions();

            _currentSlotIndex = slot.GetSlotIndex();
            _selectSaveButton.GetComponentInChildren<Text>().text = "Start";
            _deleteButton.gameObject.SetActive(false);

            slot.slotButton.gameObject.SetActive(true);
            slot.slotButton.interactable = false;
            slot.inputField.enabled = true;
            slot.inputField.text = $"Profile_0{_currentSlotIndex + 1}";

            slot.inputField.onSubmit.RemoveAllListeners();
            slot.inputField.onSubmit.AddListener((value) =>
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _currentProfile = value;
                }
                else
                {
                    _currentProfile = $"Profile_0{_currentSlotIndex + 1}";
                }

                GameManagerContext.Instance.GameManagerEventSystem.SetSelectedGameObject(_selectSaveButton.gameObject);
            });

            GameManagerContext.Instance.GameManagerEventSystem.SetSelectedGameObject(slot.inputField.gameObject);

            _selectSaveButton.onClick.RemoveAllListeners();
            _selectSaveButton.onClick.AddListener(() =>
            {
                _currentProfile = slot.inputField.text;

                if (string.IsNullOrEmpty(_currentProfile))
                {
                    _currentProfile = $"Profile_0{_currentSlotIndex + 1}";
                }

                CreateSaveGame();
                HideOptions();
            });
        });
    }
    #endregion

    #region OPTIONS METHODS
    private void ShowOptions()
    {
        SlotIsSelected = true;

        _optionsParent.gameObject.SetActive(true);

        for (int i = 0; i < _saveSlots.Length; i++)
        {
            _saveSlots[i].slotButton.gameObject.SetActive(false);
            _saveSlots[i].inputField.enabled = false;
        }
    }
    public void HideOptions()
    {
        SlotIsSelected = false;

        _optionsParent.gameObject.SetActive(false);

        for (int i = 0; i < _saveSlots.Length; i++)
        {
            _saveSlots[i].slotButton.gameObject.SetActive(true);
            _saveSlots[i].slotButton.interactable = true;
            _saveSlots[i].inputField.enabled = false;
        }

        CheckSaveSlots();
    }
    #endregion

    #region SAVES METHODS
    private void CreateSaveGame()
    {
        _currentSaveFile = SaveStorage.instance.GetSaveAtIndex(_currentSlotIndex);

        _currentSaveFile.AddOrUpdateData(PLAYER_PROFILE, CreateProfileData(_currentProfile));

        _currentSaveFile.Save();

        var data = _currentSaveFile.GetData<PlayerProfileData>(PLAYER_PROFILE);

        ApplyLoadedProfileData(data);

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

        ApplyLoadedProfileData(data);
    }
    public void SaveGame()
    {
        PrepareProfileDataToSave(_currentProfile, GameManagerContext.Instance.CharacterContextManager);

        _currentSaveFile = SaveStorage.instance.GetSaveAtIndex(_currentSlotIndex);

        _currentSaveFile.AddOrUpdateData(PLAYER_PROFILE, GetProfileData());

        _currentSaveFile.Save();

        ProfileData = null;
    }
    public void DeleteSaveGame()
    {
        _currentSaveFile = SaveStorage.instance.GetSaveAtIndex(_currentSlotIndex);

        _currentSaveFile.DeleteData(PLAYER_PROFILE);

        _currentSaveFile.Save();
    }
    #endregion

    #region PROFILE DATA MANAGEMENT
    private void PrepareProfileDataToSave(string profile, CharacterContextManager characterContextManager)
    {
        ProfileData.ProfileName = profile;
        ProfileData.CharacterSpawningPosition = characterContextManager.transform.position.ToSavable();
        ProfileData.CharacterHasAirJump = characterContextManager.HasAirJump;
        ProfileData.CharacterHasDash = characterContextManager.HasDash;
        ProfileData.CharacterHasWallMove = characterContextManager.HasWallMove;
    }
    private void ApplyLoadedProfileData(PlayerProfileData data)
    {
        ProfileData.ProfileName = data.ProfileName;
        ProfileData.CharacterSpawningPosition = data.CharacterSpawningPosition;
        ProfileData.CharacterHasAirJump = data.CharacterHasAirJump;
        ProfileData.CharacterHasDash = data.CharacterHasDash;
        ProfileData.CharacterHasWallMove = data.CharacterHasWallMove;
    }
    private PlayerProfileData CreateProfileData(string profileName)
    {
        ProfileData = new PlayerProfileData(profileName, new SavableVector(0.00f, 3.00f, 0.00f, 0.00f), false, false, false);

        return ProfileData;
    }
    private PlayerProfileData GetProfileData()
    {
        return ProfileData;
    }
    #endregion
}
