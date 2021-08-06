using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hourglass_Script : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Game_Controller.controllerInstance.AddTime(30);
            Destroy(gameObject);
        }
    }
}
