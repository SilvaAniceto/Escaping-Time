using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager ManagerInstance;

    [SerializeField] public GameObject _mainMenu;
    [SerializeField] public GameObject _mainMenuDefaultObject;
    [SerializeField] public GameObject _pauseMenu;
    [SerializeField] public GameObject _pauseMenuDefaultObject;

    public bool GameIsPaused { get; set; } = false;

    private bool PauseInput 
    {
        get
        {
            var inputModule = (InputSystemUIInputModule)EventSystem.current.currentInputModule;
            return inputModule.cancel.action.WasPressedThisFrame();
        }
    }

    private void Awake()
    {
        if (ManagerInstance == null)
        {
            ManagerInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (PauseInput)
        {
            GameIsPaused = !GameIsPaused;
            _pauseMenu.SetActive(GameIsPaused);
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
        SceneManager.sceneUnloaded += (Scene) => 
        {
            GameIsPaused = false;
            _mainMenu.SetActive(false);
            EventSystem.current.SetSelectedGameObject(_pauseMenuDefaultObject);
        };
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        SceneManager.sceneUnloaded += (Scene) => 
        {
            _pauseMenu.SetActive(false);
            _mainMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(_mainMenuDefaultObject);
        };
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
