using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour
{
    public static Game_Controller controllerInstance;
    public static float timer;

    [SerializeField] Text clock;
    string sceneName;

    TimeSpan time;

    private void Awake()
    {
        controllerInstance = this;
    }

    void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Fase_0")
            timer = 30;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        time = TimeSpan.FromSeconds(timer);
        clock.text = time.ToString(@"mm\:ss"); 
        
        if(timer < 0)
            SceneManager.LoadScene("Game_Over");
           
    }

    public void AddTime(int added)
    {
        timer += added;
    }
}
