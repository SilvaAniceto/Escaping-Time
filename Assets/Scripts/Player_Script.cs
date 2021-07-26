using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Script : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpHeight;
    [SerializeField] float groundRay;
    [SerializeField] LayerMask whatIsGround;
    Vector2 facingSide = new Vector2(1, 0);
    bool isflip = false;
    
    Rigidbody2D rb2d;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position, Vector2.down, groundRay, whatIsGround);
        RaycastHit2D hitWall = Physics2D.Raycast(transform.position, facingSide, groundRay, whatIsGround);

        if (hitWall.collider == null)
            if (Input.GetAxis("Horizontal") != 0)       
                rb2d.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb2d.velocity.y);        

        if (hitGround.collider != null)
            if (Input.GetButtonDown("Jump"))
                rb2d.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);

        if (Input.GetAxis("Horizontal") < 0 && !isflip)
            Flip();
        else if (Input.GetAxis("Horizontal") > 0 && isflip)
            Flip();
    }

    void Flip()
    {
        isflip = !isflip;
        facingSide = -facingSide;
        transform.Rotate(0, 180, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundRay, transform.position.z));
    }
}
