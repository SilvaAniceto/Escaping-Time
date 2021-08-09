using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform_Script : MonoBehaviour
{
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    Vector3 target;
    [SerializeField] float speed;
    // Start is called before the first frame update
    void Start()
    {
        target = pointA.position;
        pointA.gameObject.SetActive(false);
        pointB.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position == pointA.position)
        {
            target = pointB.position;
        }
        else if (transform.position == pointB.position)
        {
            target = pointA.position;
        }

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.collider.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.collider.transform.SetParent(null);
        }
    }
}
