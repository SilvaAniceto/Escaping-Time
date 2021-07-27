using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Script : MonoBehaviour
{
    public static Door_Script instance;

    public string password = "";
    [SerializeField] GameObject fakeDoor;

    public Animator anim;

    [SerializeField] Switch_Script[] switches;     
    
    void Awake()
    {
        instance = GetComponent<Door_Script>();
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        for (int i = 0; i < switches.Length; i++)
        {
            switches[i].GetComponent<Switch_Script>();
        }
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
        GameObject[] auxDoor = GameObject.FindGameObjectsWithTag("Doors");
        foreach (GameObject item in auxDoor)
        {
            password = "";

            fakeDoor.GetComponent<SpriteRenderer>().enabled = true;
            fakeDoor.GetComponent<BoxCollider2D>().isTrigger = false;

            item.GetComponent<SpriteRenderer>().enabled = true;
            item.GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        anim.Play("New State 0");
        ResetDoor();
    }
}
