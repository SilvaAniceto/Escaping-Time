using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakeDoor_Script : MonoBehaviour
{
    [SerializeField] Transform teleportTarget;
    [SerializeField] GameObject player;

    void OnTriggerExit2D(Collider2D collision)
    {        
            player.transform.position = teleportTarget.transform.position;
            Door_Script.instance.ResetDoor();
            Switch_Script.instance.ResetSwitch();
        
    }
}
