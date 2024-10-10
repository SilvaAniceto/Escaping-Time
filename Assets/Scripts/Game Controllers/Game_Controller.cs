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
    public static int continueTimes;
    [HideInInspector] public int auxFlames;

    [SerializeField] Text clock;
    [SerializeField] Text flameText;
    [SerializeField] Text continueText;
    [SerializeField] GameObject continuePanel;
    string sceneName;

    [SerializeField] private AudioSource _audioSource;

    TimeSpan time;

    private void Awake()
    {
        controllerInstance = this;
    }

    void Start()
    {
        continuePanel.SetActive(false);
        sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Fase_0")
        {
            continueTimes = 2;
            timer = 60;
            flames = 3;
        }
        _audioSource = Audio_Manager.instance._clockSource;
    }

    void Update()
    {
        auxFlames = flames;
        flameText.text = "x " + auxFlames.ToString();
        continueText.text = "x " + continueTimes.ToString();

        if (Character_Moviment.moveInstance.CountingTime)
        {
            timer -= Time.deltaTime;
            time = TimeSpan.FromSeconds(timer);
            clock.text = time.ToString(@"mm\:ss");
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
        else
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
        }
        
        if(timer < 0)
            if (continueTimes > 0)
            {
                Time.timeScale = 0;
                continuePanel.SetActive(true);
            }
            else 
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

    public void Continue()
    {
        if (continueTimes > 0)
        {
            timer = 30;
            Time.timeScale = 1;
            continueTimes -= 1;
            continuePanel.SetActive(false);
        }
    }

    public void GameOver()
    {
        Time.timeScale = 1;
        continueTimes = 2;
        timer = 60;
        SceneManager.LoadScene("Game_Over");
    }
}
