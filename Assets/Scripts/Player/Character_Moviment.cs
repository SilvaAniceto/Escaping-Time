using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Moviment : MonoBehaviour
{
    public static Character_Moviment moveInstance;

    [Header("Movement")]
    [SerializeField] float speed;
    [HideInInspector] public float moveInput;
    bool _jumpInput;
    bool isFlip = false;
    [HideInInspector] public Vector2 facingSide = new Vector2(1, 0);

    [Header("Jump")]
    [SerializeField] float jumpForce;
    [SerializeField] float groundRaySize;
    [SerializeField] LayerMask whatIsGround;
    public bool grounded;
 
    public Rigidbody2D rb;
    [HideInInspector] public bool isHit;
    [SerializeField] Vector3 offSet;
    private bool _allowJump = true;
    private bool _isJumping = false;

    public PlayerInputActions _inputActions;
    private bool Grounded
    {
        set
        {
            if (grounded == value)
            {
                return;
            }

            grounded = value;

            if (grounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.drag = 0;
                _isJumping = false;
                _allowJump = true;
                rb.gravityScale = 1;
            }
            else if (!grounded)
            {
                rb.drag = 1.5f;
                if (!_isJumping)
                {
                    StopCoroutine("CoyoteTime");
                    StartCoroutine("CoyoteTime");
                }
            }
        }
    }

    public bool Interact { get; set; }

    public bool CountingTime
    {
        get
        {
            return moveInput != 0 ? true : false;
        }
    }

    IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(0.2f);

        _allowJump = false;
    }

    void Awake()
    {
        moveInstance = this;
        rb = this.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();
    }

    void Update()
    {
        ReadInputs();

        if (_jumpInput && _allowJump)
            Jump();
    }

    private void FixedUpdate()
    {
        CheckRayCasts();

        if (!isHit)
        {
            if (moveInput != 0)
                Move();
        }
    }

    private void ReadInputs()
    {
        moveInput = _inputActions.PlayerActionMap.Move.ReadValue<float>();
        _jumpInput = _inputActions.PlayerActionMap.Jump.WasPressedThisFrame();
        Interact = _inputActions.PlayerActionMap.Interact.WasPressedThisFrame();
    }

    void CheckRayCasts()
    {
        Grounded = Physics2D.Raycast(transform.position + offSet, Vector2.down, groundRaySize, whatIsGround) || Physics2D.Raycast(transform.position - offSet, Vector2.down, groundRaySize, whatIsGround);
    }
    void Move()
    {
        rb.MovePosition(new Vector2(rb.position.x, rb.position.y) + speed * facingSide * Time.deltaTime);

        rb.velocity = Vector2.ClampMagnitude(rb.velocity, speed);
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
        _isJumping = true;
        _allowJump = false;
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
        rb.gravityScale = 3.14f;
    }

    public void ThrowBack()
    {
        rb.velocity = Vector2.zero;
        isHit = true;
        rb.AddForce(new Vector2(-facingSide.x * 8.5f, 7f), ForceMode2D.Force);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position + offSet, new Vector3(transform.position.x + offSet.x, transform.position.y - groundRaySize, transform.position.z));
        Gizmos.DrawLine(transform.position - offSet, new Vector3(transform.position.x - offSet.x, transform.position.y - groundRaySize, transform.position.z));        
    }
    void OnGUI()
    {
        GUILayout.Label("Y VELOCITY: " + rb.velocity.y);
    }
}