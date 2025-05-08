using Esper.ESave;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(SaveStorage))]
public class GameSaveSystem : MonoBehaviour
{
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
    #endregion

    #region PROPERTIES
    public SaveFile CurrentSave {  get; private set; }
    public bool SlotIsSelected { get; private set; }
    #endregion

    #region CALLBACKS
    [HideInInspector] public UnityEvent OnLaunchGame = new UnityEvent();
    #endregion

    #region UNITY DEFAULT METHODS
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
    private void SetSaveSlots(string profile, int index)
    {
        SaveFileSetupData saveFileSetupData = new SaveFileSetupData
        {
            fileName = $"{profile}",
            saveLocation = _saveFileSetup.saveFileData.saveLocation,
            filePath = _saveFileSetup.saveFileData.filePath,
            fileType = _saveFileSetup.saveFileData.fileType,
            encryptionMethod = _saveFileSetup.saveFileData.encryptionMethod,
            addToStorage = true
        };

        SaveFile saveFile = new SaveFile(saveFileSetupData);
    }
    private void CheckSaveSlots()
    {
        for (int i = 0; i < _saveSlots.Length; i++)
        {
            CurrentSave = null;

            if (i < SaveStorage.instance.saveCount)
            {
                CurrentSave = SaveStorage.instance.GetSaveAtIndex(i);
            }

            GameSaveSlot slot = _saveSlots[i];

            slot.SetSlotIndex(i);

            if (CurrentSave != null)
            {
                if (CurrentSave.HasData("CharacterData"))
                {
                    slot.SetLabelText(CurrentSave.fileName);

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
            }
            else
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
        }

        GameManagerContext.Instance.GameManagerEventSystem.SetSelectedGameObject(_saveSlots[0].slotButton.gameObject);
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
        SetSaveSlots(_currentProfile, _currentSlotIndex);

        CurrentSave = SaveStorage.instance.GetSaveAtIndex(_currentSlotIndex);

        CurrentSave.AddOrUpdateData("CharacterData", GameManagerContext.CreateCharacterData());

        CurrentSave.Save();

        var data = CurrentSave.GetData<GameManagerContext.PlayerCharacterData>("CharacterData");

        GameManagerContext.ApplyLoadedCharacterData(data);

        OnLaunchGame?.Invoke();
    }
    private void LoadAndLaunch()
    {
        LoadGame();

        OnLaunchGame?.Invoke();
    }
    public void LoadGame()
    {
        CurrentSave = SaveStorage.instance.GetSaveAtIndex(_currentSlotIndex);

        var data = CurrentSave.GetData<GameManagerContext.PlayerCharacterData>("CharacterData");

        GameManagerContext.ApplyLoadedCharacterData(data);
    }
    public void SaveGame()
    {
        GameManagerContext.PrepareCharacterDataToSave(GameManagerContext.Instance.CharacterContextManager);

        CurrentSave = SaveStorage.instance.GetSaveAtIndex(_currentSlotIndex);

        CurrentSave.AddOrUpdateData("CharacterData", GameManagerContext.GetCharacterData());

        CurrentSave.Save();
    }
    public void DeleteSaveGame()
    {
        CurrentSave = SaveStorage.instance.GetSaveAtIndex(_currentSlotIndex);

        CurrentSave.DeleteFile();
    }
    #endregion
}
