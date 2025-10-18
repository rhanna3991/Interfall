using UnityEngine;
using System.Collections.Generic;

public class FollowPlayer : MonoBehaviour
{
    public PlayerMovement leader;
    public float followDistance = 0.6f;
    public float moveSpeed = 6f;
    
    [Header("Sprite Positioning")]
    public Vector2 spriteOffset = Vector2.zero;
    
    [Header("Movement Smoothing")]
    public float smoothTime = 0.1f; // How fast to reach target
    
    private Vector3 velocity = Vector3.zero; // Makes movement smoother

    private SpriteRenderer spriteRenderer;
    private bool isFacingRight = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (leader == null || leader.breadcrumbs.Count < 2) return;

        Vector3 target = GetTrailPosition(followDistance, leader.breadcrumbs);
        
        // Apply sprite offset to the target position
        Vector3 offsetTarget = target + new Vector3(spriteOffset.x, spriteOffset.y, 0);
        
        transform.position = Vector3.SmoothDamp(transform.position, offsetTarget, ref velocity, smoothTime);

        // Flip sprite based on movement direction
        Vector3 moveDirection = offsetTarget - transform.position;
        if (spriteRenderer != null && moveDirection.magnitude > 0.01f)
        {
            if (moveDirection.x > 0.05f && !isFacingRight)
            {
                spriteRenderer.flipX = false;
                isFacingRight = true;
            }
            else if (moveDirection.x < -0.05f && isFacingRight)
            {
                spriteRenderer.flipX = true;
                isFacingRight = false;
            }
        }
    }

    // Finds the position on the breadcrumb path exactly distanceBehind units from the leader
    Vector3 GetTrailPosition(float distanceBehind, List<Vector3> trail)
    {
        float accumulatedDistance = 0f;

        for (int i = 0; i < trail.Count - 1; i++)
        {
            Vector3 start = trail[i];
            Vector3 end = trail[i + 1];
            float segmentLength = Vector3.Distance(start, end);
            accumulatedDistance += segmentLength;

            if (accumulatedDistance >= distanceBehind)
            {
                float overshoot = accumulatedDistance - distanceBehind;
                float t = 1f - (overshoot / segmentLength);
                return Vector3.Lerp(end, start, t);
            }
        }

        // If not enough trails yet return the last breadcrumb
        return trail[trail.Count - 1];
    }
}
