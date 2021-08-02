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

    TimeSpan time;

    void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
    }

    void Update()
    {        
        if (sceneName != "Fase_10")
        {          
            timer += Time.deltaTime;
            time = TimeSpan.FromSeconds(timer);
            clock.text = time.ToString(@"hh\:mm\:ss") + " AM";            
            if(timer < 0)
            {
                SceneManager.LoadScene("Game_Over");
            }
        }
        else
        {
            timer = timer;
            time = TimeSpan.FromSeconds(timer);
            clock.text ="AT " + time.ToString(@"hh\:mm\:ss") + " AM";
        }
    }
}
