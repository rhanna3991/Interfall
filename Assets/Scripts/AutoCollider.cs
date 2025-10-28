using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCollider : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Create BoxCollider2D if it doesn't exist
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        UpdateColliderToSprite();
    }

    public void UpdateColliderToSprite()
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null || boxCollider == null) return;

        // Convert sprite bounds to collider size/offset
        boxCollider.size = spriteRenderer.sprite.bounds.size;
        boxCollider.offset = spriteRenderer.sprite.bounds.center;
    }
    
    // Method to set custom collider size for specific enemies
    public void SetColliderSize(float multiplier, Vector2 offset)
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null || boxCollider == null) return;

        // Convert sprite bounds to collider size/offset with multiplier and offset
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        boxCollider.size = spriteSize * multiplier;
        boxCollider.offset = (Vector2)spriteRenderer.sprite.bounds.center + offset;
    }
}
