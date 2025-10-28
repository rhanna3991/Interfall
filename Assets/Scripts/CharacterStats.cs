using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LearnableAbility
{
    public string abilityName;
    public string abilityDescription;
    public int manaCost;
    public int baseDamage;
    public int unlockLevel;
}

public enum StatType
{
    MaxHP,
    Attack,
    Defense,
    Speed,
    SpecialAttack,
    MaxMana
}

[CreateAssetMenu(fileName = "New Character Data", menuName = "Interfall/Character")]
public class CharacterStats : ScriptableObject
{
    [Header("Character Info")]
    public string characterName;
    public Sprite characterSprite;

    [Header("Base Stats (Level 1)")]
    public int baseMaxHP = 100;
    public int baseAttack = 10;
    public int baseDefense = 5;
    public int baseSpeed = 10;
    public int baseSpecialAttack = 10;
    public int baseMaxMana = 100;

    [Header("How much each stat increases per level")]
    public float hpGrowth = 5f;
    public float attackGrowth = 2f;
    public float defenseGrowth = 1f;
    public float speedGrowth = 1f;
    public float specialAttackGrowth = 2f;
    public float manaGrowth = 10f;
    
    [Header("EXP System")]
    public int baseExpRequired = 100;
    public float expGrowthMultiplier = 1.2f; // Each level requires 20% more EXP
    
    [Header("Abilities")]
    public List<Ability> characterAbilities;

    public int GetStatAtLevel(StatType statType, int level)
    {
        level = Mathf.Max(1, level); // Ensure level is at least 1

        switch (statType)
        {
            case StatType.MaxHP:
                return Mathf.RoundToInt(baseMaxHP + (hpGrowth * (level - 1)));
            case StatType.Attack:
                return Mathf.RoundToInt(baseAttack + (attackGrowth * (level - 1)));
            case StatType.Defense:
                return Mathf.RoundToInt(baseDefense + (defenseGrowth * (level - 1)));
            case StatType.Speed:
                return Mathf.RoundToInt(baseSpeed + (speedGrowth * (level - 1)));
            case StatType.SpecialAttack:
                return Mathf.RoundToInt(baseSpecialAttack + (specialAttackGrowth * (level - 1)));
            case StatType.MaxMana:
                return Mathf.RoundToInt(baseMaxMana + (manaGrowth * (level - 1)));
            default:
                return 0;
        }
    }
    
    // Get an ability that is unlocked at a given level
    public List<Ability> GetUnlockedAbilities(int playerLevel)
    {
        List<Ability> unlocked = new List<Ability>();
        
        if (characterAbilities != null)
        {
            foreach (var ability in characterAbilities)
            {
                if (playerLevel >= ability.unlockLevel)
                {
                    unlocked.Add(ability);
                }
            }
        }
        
        return unlocked;
    }
    
    // Check if a specific ability is unlocked at a given level
    public bool IsAbilityUnlocked(string abilityName, int playerLevel)
    {
        if (characterAbilities != null)
        {
            foreach (var ability in characterAbilities)
            {
                if (ability.abilityName == abilityName && playerLevel >= ability.unlockLevel)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    // Calculate EXP required for a specific level
    public int GetExpRequiredForLevel(int level)
    {
        level = Mathf.Max(1, level); // Ensure level is at least 1
        return Mathf.RoundToInt(baseExpRequired * Mathf.Pow(expGrowthMultiplier, level - 1));
    }
    
    // Calculate total EXP needed to reach a specific level from level 1
    public int GetTotalExpForLevel(int level)
    {
        level = Mathf.Max(1, level); // Ensure level is at least 1
        int totalExp = 0;
        
        for (int i = 1; i < level; i++)
        {
            totalExp += GetExpRequiredForLevel(i);
        }
        
        return totalExp;
    }
}


