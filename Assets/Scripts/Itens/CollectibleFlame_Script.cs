using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleFlame_Script : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Game_Controller.controllerInstance.ChangeFlames(1);
            Destroy(gameObject);
        }
    }
}
