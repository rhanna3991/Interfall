using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("Player Data")]
    public CharacterStats playerStats;
    public int playerLevel = 1;
    public int playerCurrentMana;
    public int playerCurrentHP;
    public int playerMaxHP;
    public int playerMaxMP;
    
    [Header("Enemy Data")]
    public EnemyStats enemyStats;
    public int enemyCurrentHP;
    public int enemyMaxHP;
    
    // Runtime enemy instance for current battle
    private RuntimeEnemy currentRuntimeEnemy;
    
    [Header("Projectile Spawns")]
    public Transform playerProjectileSpawn;
    public Transform enemyProjectileSpawn;
    
    [Header("UI References")]
    public MagicMenu magicMenuUI;
    
    public int currentPartyMemberIndex = 0; // Which party member is currently attacking
    
    // Reference to UIBattleManager for communication
    private UIBattleManager uiBattleManager;
    
    // Reference to PlayerStatsUI for communication
    private PlayerUI playerUI;
    
    // Unlocked abilities system
    public List<Ability> unlockedAbilities = new List<Ability>();
    
    void Start()
    {
        // Get reference to UIBattleManager
        uiBattleManager = FindObjectOfType<UIBattleManager>();
        
        // Get reference to PlayerUI
        playerUI = FindObjectOfType<PlayerUI>();
        
        // Initialize unlocked abilities FIRST
        InitializeUnlockedAbilities();
        
        // Initialize player mana and HP
        if (playerStats != null)
        {
            playerCurrentMana = playerStats.GetStatAtLevel(StatType.MaxMana, playerLevel);
            playerMaxMP = playerStats.GetStatAtLevel(StatType.MaxMana, playerLevel);
            playerMaxHP = playerStats.GetStatAtLevel(StatType.MaxHP, playerLevel);
            playerCurrentHP = playerMaxHP;
            
            // Initialize UI bars
            if (uiBattleManager != null)
            {
                uiBattleManager.UpdateManaBar(playerCurrentMana, playerMaxMP);
                uiBattleManager.UpdateHealthBar(playerCurrentHP, playerMaxHP);
            }
            
            // Notify PlayerUI of initial values
            NotifyPlayerStatsChanged();
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
    }
    
    public void HandleEnemyAttack()
    {
        if (playerStats == null || enemyStats == null) return;
        
        // Calculate and apply damage
        int damage = CalculateEnemyDamage();
        ApplyDamageToPlayer(damage);
        
        // Show enemy damage dialogue
        if (uiBattleManager != null)
        {
            uiBattleManager.ShowEnemyDamageDialogue(damage, playerStats, enemyStats);
        }
    }
    
    int CalculatePlayerDamage()
    {
        int playerAttack = playerStats.GetStatAtLevel(StatType.Attack, playerLevel);
        int enemyDefense = currentRuntimeEnemy != null ? currentRuntimeEnemy.GetDefense() : enemyStats.baseDefense;
        return Mathf.Max(1, playerAttack - enemyDefense); // Minimum 1 damage
    }
    
    int CalculateEnemyDamage()
    {
        int enemyAttack = currentRuntimeEnemy != null ? currentRuntimeEnemy.GetAttack() : enemyStats.baseAttack;
        int playerDefense = playerStats.GetStatAtLevel(StatType.Defense, playerLevel);
        return Mathf.Max(1, enemyAttack - playerDefense); // Minimum 1 damage
    }
    
    public void ApplyDamageToEnemy(int damage)
    {
        enemyCurrentHP -= damage;
        enemyCurrentHP = Mathf.Max(0, enemyCurrentHP); // Don't go below 0
        
        // Update RuntimeEnemy if available
        if (currentRuntimeEnemy != null)
        {
            currentRuntimeEnemy.TakeDamage(damage);
        }
        
        uiBattleManager?.UpdateBattleUI(damage, playerStats, enemyStats);
        CheckBattleEnd();
    }
    
    public void ApplyDamageToPlayer(int damage)
    {
        playerCurrentHP -= damage;
        playerCurrentHP = Mathf.Max(0, playerCurrentHP); // Don't go below 0
        
        // Update health bar UI
        if (uiBattleManager != null)
        {
            uiBattleManager.UpdateHealthBar(playerCurrentHP, playerMaxHP);
        }
        
        // Notify PlayerUI of HP change
        NotifyPlayerStatsChanged();
        
        CheckBattleEnd();
    }
    
    // Trigger hitflash on enemy
    public void TriggerEnemyHitflash()
    {
        if (uiBattleManager != null && enemyStats != null)
        {
            uiBattleManager.PlayEnemyHitflash(enemyStats);
        }
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
                TransitionUI transitionUI = FindObjectOfType<TransitionUI>();
                if (transitionUI != null && transitionUI.magicMenu != null)
                    transitionUI.magicMenu.SetActive(false);
            }
            uiBattleManager.ShowMagicDamageDialogue(damageMessage);
        }
    }
    
    
    void CheckBattleEnd()
    {
        if (enemyCurrentHP <= 0)
        {
            Debug.Log(enemyStats.enemyName + " is defeated!");
            
            // Show victory dialogue with EXP message - battle will end when dialogue completes
            if (uiBattleManager != null && enemyStats != null)
            {
                int expGained = enemyStats.expGiven;
                
                // Give EXP to player
                if (playerUI != null)
                {
                    playerUI.GainExp(expGained);
                }
                
                // Show victory dialogue with EXP message
                uiBattleManager.ShowVictoryDialogue(enemyStats.enemyName, expGained);
            }
        }
        
        if (playerCurrentHP <= 0)
        {
            Debug.Log("Player is defeated!");
            // Notify UIBattleManager that player is defeated
            if (uiBattleManager != null)
            {
                uiBattleManager.OnPlayerDeath();
            }
        }
    }
    
    // Method to set the current RuntimeEnemy
    public void SetRuntimeEnemy(RuntimeEnemy runtimeEnemy)
    {
        currentRuntimeEnemy = runtimeEnemy;
        if (runtimeEnemy != null)
        {
            enemyStats = runtimeEnemy.data;
            enemyCurrentHP = runtimeEnemy.currentHP;
            enemyMaxHP = runtimeEnemy.GetMaxHP();
        }
    }
    
    // Method to get the current enemy's defense (for ability calculations)
    public int GetEnemyDefense()
    {
        return currentRuntimeEnemy != null ? currentRuntimeEnemy.GetDefense() : enemyStats.baseDefense;
    }
    
    
    // Method to switch between party members
    public void SetCurrentPartyMember(int index)
    {
        currentPartyMemberIndex = index;
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
        
        // Notify PlayerUI of MP change
        NotifyPlayerStatsChanged();
    }
    
    // Method to notify PlayerUI when stats change
    private void NotifyPlayerStatsChanged()
    {
        if (playerUI != null)
        {
            playerUI.OnPlayerStatsChanged();
        }
    }
}
