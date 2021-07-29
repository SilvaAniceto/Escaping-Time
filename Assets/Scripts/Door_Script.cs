using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door_Script : MonoBehaviour
{
    public static Door_Script instance;

    public string password = "";
    [SerializeField] GameObject fakeDoor;

    int level;                
    
    void Awake()
    {
        instance = GetComponent<Door_Script>();
    }
    void Start()
    {
        level = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        if (password.Length == 3)
            if (password == "123")
            {
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                gameObject.GetComponent<BoxCollider2D>().isTrigger = true;                
            }                
            else
            {
                fakeDoor.GetComponent<SpriteRenderer>().enabled = false;
                fakeDoor.GetComponent<BoxCollider2D>().isTrigger = true;
            }
    }

    public void ResetDoor()
    {       
        password = "";


        fakeDoor.GetComponent<SpriteRenderer>().enabled = true;
        fakeDoor.GetComponent<BoxCollider2D>().isTrigger = false;

        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        gameObject.GetComponent<BoxCollider2D>().isTrigger = false;        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            SceneManager.LoadScene("Fase_" + (level + 1).ToString());
            Game_Controller.timer += 60;
        }
    }

}
