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
    public static int flames;
    [HideInInspector] public int auxFlames;

    [SerializeField] Text clock;
    [SerializeField] Text flameText;
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
        {
            timer = 60;
            flames = 3;
        }
    }

    void Update()
    {
        auxFlames = flames;
        flameText.text = "x " + auxFlames.ToString();

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

    public void ChangeFlames(int changed)
    {
        flames += changed;
    }
}
