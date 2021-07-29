using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour
{
    public static float timer;

    [SerializeField] Text clock;
    string sceneName;

    void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        if (sceneName != "Fase_0")
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
                TimeSpan time = TimeSpan.FromSeconds(timer);
                clock.text = time.ToString(@"hh\:mm\:ss") + " AM";
            }
        }        
    }
}
