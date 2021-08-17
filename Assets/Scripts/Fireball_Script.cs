using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball_Script : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] Vector2 fireDir;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.rotation == 0)
            fireDir = Vector2.left;
        else if (rb.rotation == 180)
            fireDir = Vector2.right;
        else if (rb.rotation == 90)
            fireDir = Vector2.down;
        else if (rb.rotation == -90)
            fireDir = Vector2.up;

        rb.AddForce(fireDir * 0.10f, ForceMode2D.Impulse);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
            Destroy(gameObject);
        else if (collision.gameObject.tag == "Player")
            Game_Controller.controllerInstance.AddTime(-5);
    }
}
