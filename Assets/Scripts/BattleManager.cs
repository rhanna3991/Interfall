using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [Header("Player Data")]
    public CharacterStats playerStats;
    public int playerLevel = 1;
    
    [Header("Enemy Data")]
    public EnemyStats enemyStats;
    public int enemyCurrentHP;
    public int enemyMaxHP;
    
    public Button attackButton;
    
    public Animator[] slashAnimators; // Array to hold multiple slash attack animators
    public GameObject[] slashGameObjects; // Array to hold the slash GameObjects (for show/hide)
    public Animator enemyAnimator; 
    public int currentPartyMemberIndex = 0; // Which party member is currently attacking
    
    void Start()
    {
        // Initialize enemy HP
        if (enemyStats != null)
        {
            enemyMaxHP = enemyStats.baseMaxHP;
            enemyCurrentHP = enemyMaxHP;
        }
        
        // Set up attack button
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(OnAttackButton);
        }
        
        HideAllSlashEffects();
        
        // Log initial enemy HP
        Debug.Log($"{enemyStats.enemyName} HP: {enemyCurrentHP}/{enemyMaxHP}");
        
        // Show initial battle text
        Debug.Log("A wild " + enemyStats.enemyName + " appears!");
    }
    
    public void OnAttackButton()
    {
        HandlePlayerAttack();
    }
    
    void HandlePlayerAttack()
    {
        if (playerStats == null || enemyStats == null) return;
        
        // Play slash animation for current party member
        PlaySlashAnimation();
        
        // Calculate and apply damage
        int damage = CalculatePlayerDamage();
        ApplyDamageToEnemy(damage);
        
        // Update UI and check battle end
        UpdateBattleUI(damage);
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
    
    void UpdateBattleUI(int damage)
    {
        // Show damage text
        Debug.Log("Dante attacks for " + damage + " damage!");
    }
    
    void CheckBattleEnd()
    {
        if (enemyCurrentHP <= 0)
        {
            Debug.Log(enemyStats.enemyName + " is defeated!");
        }
    }
    
    void PlaySlashAnimation()
    {
        // Check if we have slash animators and current index is valid
        if (slashAnimators != null && slashAnimators.Length > 0 && 
            currentPartyMemberIndex >= 0 && currentPartyMemberIndex < slashAnimators.Length)
        {
            Animator currentAnimator = slashAnimators[currentPartyMemberIndex];
            GameObject currentSlashObject = null;
            
            if (slashGameObjects != null && currentPartyMemberIndex < slashGameObjects.Length)
            {
                currentSlashObject = slashGameObjects[currentPartyMemberIndex];
            }
            
            if (currentAnimator != null && currentSlashObject != null)
            {
                // Show the slash effect and trigger animation
                currentSlashObject.SetActive(true);
                currentAnimator.SetTrigger("FireSlash");
                
                // Trigger enemy hit flash
                if (enemyAnimator != null)
                {
                    enemyAnimator.SetTrigger("HitFlash");
                }
            }
        }
    }
    
    // Method to switch between party members
    public void SetCurrentPartyMember(int index)
    {
        if (slashAnimators != null && index >= 0 && index < slashAnimators.Length)
        {
            currentPartyMemberIndex = index;
        }
    }
    
    // Hide all slash effects at the start
    void HideAllSlashEffects()
    {
        if (slashGameObjects != null)
        {
            foreach (GameObject slashObject in slashGameObjects)
            {
                if (slashObject != null)
                {
                    slashObject.SetActive(false);
                }
            }
        }
    }
    
}
