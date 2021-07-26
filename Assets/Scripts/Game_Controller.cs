using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_Controller : MonoBehaviour
{
    public static Game_Controller instance;

    [SerializeField] Text timer;
    [SerializeField] float time;

    void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //time = time;
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;
        int time2 = (int)time;
        timer.text = "TIME: " + time2.ToString();
        
    }

    public void NextFase(string whatDoor)
    {
        if (whatDoor == "FakeDoor")
            SceneManager.LoadScene("SampleScene");
    }
}
