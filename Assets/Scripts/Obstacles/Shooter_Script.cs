using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter_Script : MonoBehaviour
{
    [SerializeField] GameObject fireball;
    [SerializeField] float auxTime;
    float shootTime;
    Vector2 fireDir;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        shootTime = auxTime; 
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.rotation == 0)
            fireDir = Vector2.left;
        else if (rb.rotation == 180)
            fireDir = Vector2.right;
        else if (rb.rotation == 90)
            fireDir = Vector2.down;
        else if (rb.rotation == -90)
            fireDir = Vector2.up;

        shootTime -= Time.deltaTime;
        if (shootTime <= 0)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject fire = Instantiate(fireball, transform.position, transform.rotation);

        Fireball_Script f = fire.GetComponent<Fireball_Script>();
        f.FireShot(fireDir);

        shootTime = auxTime;
    }
}
