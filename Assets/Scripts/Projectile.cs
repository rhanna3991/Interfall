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
                if (uiBattleManager != null)
                {
                    uiBattleManager.PlayEnemyHitflash();
                }
                
                battleManager.ApplyDamageToEnemy(damage);
                
                // Show custom dialogue with ability name
                battleManager.ShowCustomDamageDialogue(damage, abilityName);
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
