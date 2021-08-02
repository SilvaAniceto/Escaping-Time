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

    Animator anim;
    
    void Awake()
    {
        instance = GetComponent<Door_Script>();
    }
    void Start()
    {
        level = SceneManager.GetActiveScene().buildIndex;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (password.Length == 3)
            if (password == "123")
            {
                anim.SetBool("Open", true);
                gameObject.GetComponent<BoxCollider2D>().isTrigger = true;                
            }                
            else
            {
                fakeDoor.GetComponent<BoxCollider2D>().isTrigger = true;
                fakeDoor.GetComponent<FakeDoor_Script>().anim.SetBool("Open_Fake", true);
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
            SceneManager.LoadScene("Fase_" + (level).ToString());
        }
    }

}
