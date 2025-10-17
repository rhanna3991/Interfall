using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float horizontalInput;
    private float verticalInput;

    // Track last movement direction for idle animations
    private float lastHorizontalInput = 0f;
    private float lastVerticalInput = 0f;
    private bool isFacingLeft = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        PlayerInput();
        FlipSprite();
        PlayerAnimations();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void PlayerInput()
    {
        // Get input from both WASD and Arrow keys
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Normalize diagonal movement
        Vector2 inputVector = new Vector2(horizontalInput, verticalInput).normalized;
        horizontalInput = inputVector.x;
        verticalInput = inputVector.y;

        if (verticalInput > 0.1f)
        {
            lastVerticalInput = 1f;  // Facing up
            lastHorizontalInput = 0f;
        }
        else if (verticalInput < -0.1f)
        {
            lastVerticalInput = -1f; // Facing down
            lastHorizontalInput = 0f;
        }
        else if (horizontalInput != 0)
        {
            lastHorizontalInput = horizontalInput;
            lastVerticalInput = 0f;
        }
    }

    void MovePlayer()
    {
        Vector2 movement = new Vector2(horizontalInput, verticalInput) * moveSpeed;
        rb.velocity = movement;
    }

    void FlipSprite()
    {
        // Flip sprite horizontally when facing left
        if (lastHorizontalInput < 0 && !isFacingLeft)
        {
            isFacingLeft = true;
            spriteRenderer.flipX = true;
        }
        else if (lastHorizontalInput > 0 && isFacingLeft)
        {
            isFacingLeft = false;
            spriteRenderer.flipX = false;
        }
    }

    void PlayerAnimations()
    {
        if (animator != null)
        {
            // Play BackIdle when facing up
            bool isFacingUp = lastVerticalInput > 0.1f;
            animator.SetBool("BackIdle", isFacingUp);

            // Play PlayerWalk when actively moving left or right
            bool isWalkingHorizontally = Mathf.Abs(horizontalInput) > 0.1f;
            animator.SetBool("PlayerWalk", isWalkingHorizontally);

            // Play BackWalk when actively walking up
            bool isWalkingUp = verticalInput > 0.1f;
            animator.SetBool("BackWalk", isWalkingUp);

            // Play ForwardWalk when actively walking down
            bool isWalkingDown = verticalInput < -0.1f;
            animator.SetBool("ForwardWalk", isWalkingDown);
        }
    }
}
