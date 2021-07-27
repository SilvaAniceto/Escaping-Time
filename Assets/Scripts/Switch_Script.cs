using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch_Script : MonoBehaviour
{
    public static Switch_Script instance;

    [SerializeField] string code;
    public bool canClick;
    public bool isActive = false;
    

    void Awake()
    {
        instance = this;   
    }

    void Update()
    {
        if (canClick)
            if (!isActive)
                if (Input.GetButtonDown("Interact"))
                {
                    Door_Script.instance.password += code;
                    isActive = true;
                }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            canClick = true;            
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            canClick = false;
    }

    public void ResetSwitch()
    {
        GameObject[] auxSwitch = GameObject.FindGameObjectsWithTag("Switches");
        foreach (GameObject item in auxSwitch)
        {
            item.GetComponent<Switch_Script>().isActive = false;
        }        
    }
}
