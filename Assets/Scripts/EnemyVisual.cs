using UnityEngine;

public class EnemyVisual : MonoBehaviour
{
    [Header("Components")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public AutoCollider autoCollider;
    
    void Awake()
    {
        // Auto-assign components if not set
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        if (animator == null)
            animator = GetComponent<Animator>();
            
        if (autoCollider == null)
        {
            autoCollider = GetComponent<AutoCollider>();
            // Create AutoCollider if it doesn't exist
            if (autoCollider == null)
            {
                autoCollider = gameObject.AddComponent<AutoCollider>();
                Debug.Log($"AutoCollider component added to {gameObject.name}");
            }
        }
    }
    
    public void SetupEnemy(EnemyStats enemyData)
    {
        if (spriteRenderer != null && enemyData.enemySprite != null)
        {
            spriteRenderer.sprite = enemyData.enemySprite;
            
            // Update collider bounds to fit the new sprite
            if (autoCollider != null)
                autoCollider.UpdateColliderToSprite();
        }
        
        if (animator != null && enemyData.animatorController != null)
        {
            animator.runtimeAnimatorController = enemyData.animatorController;
        }
        
        // Apply scale
        transform.localScale = enemyData.scale;
        
        // Apply custom collider settings if available
        if (autoCollider != null)
        {
            autoCollider.SetColliderSize(enemyData.colliderSizeMultiplier, enemyData.colliderOffset);
        }
    }
}
