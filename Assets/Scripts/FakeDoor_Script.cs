using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeDoor_Script : MonoBehaviour
{
    [SerializeField] Transform teleportTarget;
    [SerializeField] GameObject player;
    [SerializeField] Door_Script door;

    void OnTriggerExit2D(Collider2D collision)
    {        
            player.transform.position = teleportTarget.transform.position;
            door.ResetDoor();
            Switch_Script.instance.ResetSwitch();
            Door_Script.instance.anim.Play("New State");
        
    }
}
