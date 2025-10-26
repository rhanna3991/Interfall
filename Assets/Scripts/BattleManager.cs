using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Player Data")]
    public CharacterStats playerStats;
    public int playerLevel = 1;
    public int playerCurrentMana;
    
    [Header("Enemy Data")]
    public EnemyStats enemyStats;
    public int enemyCurrentHP;
    public int enemyMaxHP;
    
    [Header("Projectile Spawns")]
    public Transform playerProjectileSpawn;
    public Transform enemyProjectileSpawn;
    
    [Header("UI References")]
    public MagicMenu magicMenuUI;
    
    public int currentPartyMemberIndex = 0; // Which party member is currently attacking
    
    // Reference to UIBattleManager for communication
    private UIBattleManager uiBattleManager;
    
    // Unlocked abilities system
    public List<Ability> unlockedAbilities = new List<Ability>();
    
    void Start()
    {
        // Get reference to UIBattleManager
        uiBattleManager = FindObjectOfType<UIBattleManager>();
        
        // Initialize unlocked abilities FIRST
        InitializeUnlockedAbilities();
        
        // Initialize player mana
        if (playerStats != null)
        {
            playerCurrentMana = playerStats.GetStatAtLevel(StatType.MaxMana, playerLevel);
            
            // Initialize mana bar UI
            if (uiBattleManager != null)
            {
                uiBattleManager.UpdateManaBar(playerCurrentMana, playerStats.GetStatAtLevel(StatType.MaxMana, playerLevel));
            }
        }
        
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
        
        // Populate magic menu with unlocked abilities right away
        if (magicMenuUI != null)
        {
            magicMenuUI.Populate(unlockedAbilities);
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
    
    public void ApplyDamageToEnemy(int damage)
    {
        enemyCurrentHP -= damage;
        enemyCurrentHP = Mathf.Max(0, enemyCurrentHP); // Don't go below 0
        uiBattleManager?.UpdateBattleUI(damage, playerStats, enemyStats);
        CheckBattleEnd();
    }
    
    // Method to show custom damage dialogue (for projectile abilities)
    public void ShowCustomDamageDialogue(int damage, string abilityName)
    {
        if (playerStats == null || enemyStats == null) return;
        
        string damageMessage = $"{playerStats.characterName} has dealt {damage} damage to {enemyStats.enemyName} with {abilityName}!";
        
        if (uiBattleManager != null)
        {
            // Hide magic menu before showing dialogue
            if (magicMenuUI != null)
            {
                // Get reference to TransitionUI to manage magic menu visibility
                TransitionUI transitionUI = FindObjectOfType<TransitionUI>();
                if (transitionUI != null && transitionUI.magicMenu != null)
                {
                    transitionUI.magicMenu.SetActive(false);
                }
            }
            
            uiBattleManager.ShowDamageDialogue(damageMessage);
        }
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
    
    // Initializes the list of unlocked abilities for the player
    void InitializeUnlockedAbilities()
    {
        unlockedAbilities.Clear();

        if (playerStats != null && playerStats.characterAbilities != null)
        {
            foreach (var ability in playerStats.characterAbilities)
            {
                if (playerLevel >= ability.unlockLevel)
                    unlockedAbilities.Add(ability);
            }
        }

        Debug.Log($"Unlocked {unlockedAbilities.Count} abilities.");
    }
    
    // Check if player has enough mana to cast an ability
    public bool CanCastAbility(Ability ability)
    {
        if (ability == null) return false;
        return playerCurrentMana >= ability.manaCost;
    }
    
    // Deduct mana cost from player's current mana
    public void DeductManaCost(Ability ability)
    {
        if (ability == null) return;
        
        playerCurrentMana -= ability.manaCost;
        playerCurrentMana = Mathf.Max(0, playerCurrentMana); // Don't go below 0
        
        // Update UI
        if (uiBattleManager != null)
        {
            uiBattleManager.UpdateManaBar(playerCurrentMana, playerStats.GetStatAtLevel(StatType.MaxMana, playerLevel));
        }
    }
}
