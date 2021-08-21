using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball_Script : MonoBehaviour
{
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void FireShot(Vector2 dir)
    {
        rb.AddForce(dir * 300f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
            Destroy(gameObject);
        else if (collision.gameObject.tag == "Player")
        {
            Game_Controller.controllerInstance.AddTime(-5);
            collision.gameObject.GetComponent<Character_Moviment>().ThrowBack();
        }
    }
}
