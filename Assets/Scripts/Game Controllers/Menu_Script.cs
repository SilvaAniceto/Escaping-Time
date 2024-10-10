using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu_Script : MonoBehaviour
{
    public void StartGame()
    {
        EventSystem eventSystem = EventSystem.current;
        DontDestroyOnLoad(eventSystem);

        SceneManager.LoadScene("Fase_0");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void RestartGame()
    {
        SceneManager.LoadScene("Menu");
    }
}
