using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 6f;
    public Vector2 direction = Vector2.right;
    
    private int damage = 0;
    private string abilityName = "";
    private BattleManager battleManager;
    private UIBattleManager uiBattleManager;
    private bool hasHit = false;
    private string enemyTag = "Enemy";

    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {   
        if (hasHit) return;
        
        // Check if we hit an enemy
        if (collision.gameObject.CompareTag(enemyTag))
        {
            Debug.Log("Enemy hit!");
            hasHit = true;
            
            // Apply damage to enemy
            if (battleManager != null && damage > 0)
            {
                // Trigger hitflash animation
                if (uiBattleManager != null && battleManager.enemyStats != null)
                {
                    uiBattleManager.PlayEnemyHitflash(battleManager.enemyStats);
                }
                
                // Store enemy HP before damage to check if enemy will be defeated
                int enemyHPBeforeDamage = battleManager.enemyCurrentHP;
                
                battleManager.ApplyDamageToEnemy(damage);
                
                // Only show magic damage dialogue if enemy is NOT defeated
                // If enemy is defeated, victory dialogue will be shown instead
                if (battleManager.enemyCurrentHP > 0)
                {
                    battleManager.ShowCustomDamageDialogue(damage, abilityName);
                }
            }
            
            // Destroy the projectile
            Destroy(gameObject);
        }
    }
    
    public void SetDamage(int damageValue)
    {
        damage = damageValue;
    }
    
    public void SetAbilityInfo(string ability)
    {
        abilityName = ability;
    }
    
    public void SetBattleManager(BattleManager bm)
    {
        battleManager = bm;
        // Also get UIBattleManager reference
        uiBattleManager = FindObjectOfType<UIBattleManager>();
    }
}
