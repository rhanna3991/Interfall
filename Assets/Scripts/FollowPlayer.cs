using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform followCharacter;      // The player or the previous party member
    public float followDistance = 0.6f;    // Distance to stay behind
    public float moveSpeed = 4f;           // Movement speed

    private SpriteRenderer spriteRenderer;
    private bool isFacingRight = true;

    private Vector2 facingDirection = Vector2.down;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (followCharacter == null) return;

        // Try to get facing direction from the player
        PlayerMovement playerMove = followCharacter.GetComponent<PlayerMovement>();
        if (playerMove != null)
        {
            facingDirection = playerMove.lastMoveDirection; // Use the player's facing direction
        }

        // Compute a position directly behind the player
        Vector3 targetPosition = followCharacter.position - (Vector3)facingDirection * followDistance;

        // Smoothly move to that position
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Flip the sprite based on direction
        if (spriteRenderer != null)
        {
            if (facingDirection.x > 0.1f && !isFacingRight)
            {
                spriteRenderer.flipX = false;
                isFacingRight = true;
            }
            else if (facingDirection.x < -0.1f && isFacingRight)
            {
                spriteRenderer.flipX = true;
                isFacingRight = false;
            }
        }
    }
}
