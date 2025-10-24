using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Player Data")]
    public CharacterStats playerStats;
    public int playerLevel = 1;
    
    [Header("Enemy Data")]
    public EnemyStats enemyStats;
    public int enemyCurrentHP;
    public int enemyMaxHP;
    
    public int currentPartyMemberIndex = 0; // Which party member is currently attacking
    
    // Reference to UIBattleManager for communication
    private UIBattleManager uiBattleManager;
    
    void Start()
    {
        // Get reference to UIBattleManager
        uiBattleManager = FindObjectOfType<UIBattleManager>();
        
        // Initialize enemy HP
        if (enemyStats != null)
        {
            enemyMaxHP = enemyStats.baseMaxHP;
            enemyCurrentHP = enemyMaxHP;
        }
        
        // Start battle sequence
        StartBattleSequence();
    }
    
    void StartBattleSequence()
    {
        // Start transition sequence
        if (uiBattleManager != null)
        {
            uiBattleManager.TransitionSequence();
        }
        
        // Start battle dialogue sequence
        StartCoroutine(BattleStartSequence());
    }
    
    public void HandlePlayerAttack()
    {
        if (playerStats == null || enemyStats == null) return;
        
        // Play slash animation for current party member
        if (uiBattleManager != null)
        {
            uiBattleManager.PlaySlashAnimation(currentPartyMemberIndex);
        }
        
        // Calculate and apply damage
        int damage = CalculatePlayerDamage();
        ApplyDamageToEnemy(damage);
        
        // Start delayed damage display
        StartCoroutine(DelayedDamageDisplay(damage));
    }
    
    IEnumerator DelayedDamageDisplay(int damage)
    {
        // Small delay before showing damage dialogue
        yield return new WaitForSeconds(0.05f);
        
        // Update UI and check battle end
        if (uiBattleManager != null)
        {
            uiBattleManager.UpdateBattleUI(damage, playerStats, enemyStats);
        }
        CheckBattleEnd();
    }
    
    int CalculatePlayerDamage()
    {
        int playerAttack = playerStats.GetStatAtLevel(StatType.Attack, playerLevel);
        int enemyDefense = enemyStats.baseDefense;
        return Mathf.Max(1, playerAttack - enemyDefense); // Minimum 1 damage
    }
    
    void ApplyDamageToEnemy(int damage)
    {
        enemyCurrentHP -= damage;
        enemyCurrentHP = Mathf.Max(0, enemyCurrentHP); // Don't go below 0
        
        // Log HP update to console
        Debug.Log($"{enemyStats.enemyName} HP: {enemyCurrentHP}/{enemyMaxHP}");
    }
    
    
    void CheckBattleEnd()
    {
        if (enemyCurrentHP <= 0)
        {
            Debug.Log(enemyStats.enemyName + " is defeated!");
        }
    }
    
    
    // Method to switch between party members
    public void SetCurrentPartyMember(int index)
    {
        currentPartyMemberIndex = index;
    }
    
    IEnumerator BattleStartSequence()
    {
        // Start battle sequence through UI manager
        if (uiBattleManager != null && enemyStats != null)
        {
            uiBattleManager.StartBattleSequence(enemyStats);
        }
        else
        {
            // Fallback if UI system isn't available
            Debug.Log($"{enemyStats.enemyName} dares to challenge you!");
            yield return new WaitForSeconds(2f);
        }
        
        Debug.Log($"{enemyStats.enemyName} HP: {enemyCurrentHP}/{enemyMaxHP}");
    }
    
}
