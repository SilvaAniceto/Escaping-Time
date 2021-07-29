using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FakeDoor_Script : MonoBehaviour
{    
    [SerializeField] Door_Script door;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            door.ResetDoor();
            SceneManager.LoadScene("Fase_0");
            Game_Controller.timer = 0;
        }

    }
}
