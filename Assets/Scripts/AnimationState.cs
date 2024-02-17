using System;
using UnityEngine;

enum PlayerState
{
    Idle,
    Walk,
    Run,
    Jump,
    Fall
};

public class AnimationState : MonoBehaviour
{
    private Vector3 _inputMovement;
    private float verticalInput;
    private float horizontalInput;
    private float lastHorizontalInput;
    private Rigidbody2D rb;
    private Animator animator;
    
    private PlayerState currentState = PlayerState.Idle;

    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        
        UpdatePlayerState();
        HandleInput();
        UpdateAnimation();
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    
    void HandleInput()
    {
        switch (currentState)
        {
            case PlayerState.Walk:
                rb.velocity = new Vector2(horizontalInput * walkSpeed, rb.velocity.y);
                break;
            case PlayerState.Run:
                rb.velocity = new Vector2(horizontalInput * runSpeed, rb.velocity.y);
                break;
            case PlayerState.Jump:
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                break;
            default:
                rb.velocity = new Vector2(0f, rb.velocity.y);
                break;
        }

        Flip();
    }

    void Flip()
    {
        if (horizontalInput != 0f)
        {
            transform.localScale = new Vector3(Mathf.Sign(horizontalInput), 1f, 1f);
            lastHorizontalInput = horizontalInput;
        }
        else
        {
            transform.localScale = new Vector3(Mathf.Sign(lastHorizontalInput), 1f, 1f);
        }
    }

    void UpdatePlayerState()
    {
        bool isGrounded = IsGrounded();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            currentState = PlayerState.Jump;
        }
        else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            currentState = PlayerState.Run;
        }
        else if (Mathf.Abs(horizontalInput) > 0f)
        {
            currentState = PlayerState.Walk;
        }
        else if (!isGrounded && rb.velocity.y < -0.1f)
        {
            currentState = PlayerState.Fall;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            animator.SetTrigger("Die");
        }
        else
        {
            currentState = PlayerState.Idle;
        }
    }

    void UpdateAnimation()
    {
        animator.SetBool("IsIdle", currentState == PlayerState.Idle);
        animator.SetBool("IsWalking", currentState == PlayerState.Walk);
        animator.SetBool("IsRunning", currentState == PlayerState.Run);
        animator.SetBool("IsJumping", currentState == PlayerState.Jump);
        animator.SetBool("IsFalling", currentState == PlayerState.Fall);
    }

}