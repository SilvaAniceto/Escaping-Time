using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool_Script : MonoBehaviour
{
    [SerializeField] Transform teleportTarget;
    [SerializeField] GameObject player;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.transform.position = teleportTarget.transform.position;
            Game_Controller.controllerInstance.AddTime(-10);
        }
    }
}
