using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door_Script : MonoBehaviour
{
    public static Door_Script instance;

    public int password ;

    int level;

    Animator anim;

    private bool canPass;
    
    void Awake()
    {
        instance = GetComponent<Door_Script>();
    }
    void Start()
    {
        level = SceneManager.GetActiveScene().buildIndex;
        anim = GetComponent<Animator>();
        canPass = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (password == 6)
        {
            canPass = true;
            anim.SetBool("Open", true);
            //gameObject.GetComponent<BoxCollider2D>().enabled = false;                
        }             
    }

    public void ResetDoor()
    {       
        password = 0;
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
        //gameObject.GetComponent<BoxCollider2D>().isTrigger = false;        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            if (canPass)
                SceneManager.LoadScene("Fase_" + (level).ToString());
    }
}
