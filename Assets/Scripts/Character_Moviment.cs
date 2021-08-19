using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Moviment : MonoBehaviour
{
    public static Character_Moviment moveInstance;

    [Header("Movement")]
    [SerializeField] float speed;
    [HideInInspector] public float moveInput;
    bool isFlip = false;
    public Vector2 facingSide = new Vector2(1, 0);

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float groundRaySize;
    [SerializeField] LayerMask whatIsGround;
    [HideInInspector] public bool grounded;

    Rigidbody2D rb;
    [SerializeField] Vector3 offSet;

    void Awake()
    {
        moveInstance = this;
        rb = this.GetComponent<Rigidbody2D>();        
    }
    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        CheckRayCasts();

        if (moveInput != 0)
            Move();
        else
            rb.velocity = new Vector2(0, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && grounded)
            Jump();

    }

    void CheckRayCasts()
    {
        grounded = Physics2D.Raycast(transform.position + offSet, Vector2.down, groundRaySize, whatIsGround) || Physics2D.Raycast(transform.position - offSet, Vector2.down, groundRaySize, whatIsGround);        
    }
    void Move()
    {
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if (moveInput < 0 && !isFlip)
            Flip();
        else if (moveInput > 0 && isFlip)
            Flip();
    }
    void Flip()
    {
        isFlip = !isFlip;
        facingSide = -facingSide;
        transform.Rotate(0, 180, 0);
    }

    void Jump()
    { 
        rb.AddForce(new Vector2(rb.velocity.x ,jumpForce), ForceMode2D.Impulse);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position + offSet, new Vector3(transform.position.x + offSet.x, transform.position.y - groundRaySize, transform.position.z));
        Gizmos.DrawLine(transform.position - offSet, new Vector3(transform.position.x - offSet.x, transform.position.y - groundRaySize, transform.position.z));        
    }
}