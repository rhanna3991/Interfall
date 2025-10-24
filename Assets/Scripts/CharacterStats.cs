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
    SpecialAttack
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

    [Header("How much each stat increases per level")]
    public float hpGrowth = 5f;
    public float attackGrowth = 2f;
    public float defenseGrowth = 1f;
    public float speedGrowth = 1f;
    public float specialAttackGrowth = 2f;
    
    [Header("Abilities")]
    public List<LearnableAbility> characterAbilities;

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
            default:
                return 0;
        }
    }
    
    // Get an ability that is unlocked at a given level
    public List<LearnableAbility> GetUnlockedAbilities(int playerLevel)
    {
        List<LearnableAbility> unlocked = new List<LearnableAbility>();
        
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
}


