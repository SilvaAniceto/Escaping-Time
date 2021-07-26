using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Script : MonoBehaviour
{
    public static Door_Script instance;

    public string password = "";
    [SerializeField] GameObject fakeDoor;

    void Awake()
    {
        instance = this;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (password.Length == 3)
            if (password == "123")
                Destroy(gameObject);
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
    }
}
