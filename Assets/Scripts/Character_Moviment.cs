using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Moviment : MonoBehaviour
{
    public static Character_Moviment moveInstance;

    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] float maxMoveAngle = 45f;
    [HideInInspector] public float moveInput;
    float groundAngle;
    bool isFlip = false;
    Vector2 moveDirection;
    public Vector2 facingSide = new Vector2(1, 0);

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float groundRaySize;
    [SerializeField] LayerMask whatIsGround;
    [HideInInspector] public bool grounded;

    Rigidbody2D rb;

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

        if (Input.GetButtonDown("Jump") && grounded)
            Jump();

        if (Input.GetAxisRaw("Vertical") < 0)
        {
            rb.AddForce(new Vector2(-facingSide.x * 2, 0), ForceMode2D.Impulse);
            Flip();
        }
    }

    void CheckRayCasts()
    {
        RaycastHit2D hitGround = Physics2D.Raycast(transform.position, Vector2.down, groundRaySize, whatIsGround);

        RaycastHit2D hitAngle = Physics2D.Raycast(transform.position, moveDirection, 0.5f, whatIsGround);
        groundAngle = Vector2.Angle(hitAngle.normal, Vector2.up);
        

        if (hitGround.collider != null)
        {
            grounded = true;
            /*if (moveInput >= 0)
                moveDirection = new Vector2(hitGround.normal.y, -hitGround.normal.x);
            else
                moveDirection = new Vector2(hitGround.normal.y, hitGround.normal.x);*/
        }
        else
            grounded = false;
    }
    void Move()
    {
        if (groundAngle < maxMoveAngle)
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
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - groundRaySize, transform.position.z));
    }
}