using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform_Script : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player_Checker")
            StartCoroutine("SetPlatform");
    }

    IEnumerator SetPlatform()
    {
        yield return new WaitForSeconds(0.5f);
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().color = Color.gray;
        yield return new WaitForSeconds(0.8f);
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;        
    }
}
