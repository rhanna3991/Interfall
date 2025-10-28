using UnityEngine;

[System.Serializable]
public class RuntimeEnemy
{
    [Header("Enemy Data")]
    public EnemyStats data;
    public int currentHP;
    public int level;

    public RuntimeEnemy(EnemyStats source, int stageLevel)
    {
        data = source;
        level = stageLevel;
        currentHP = GetMaxHP();
    }

    public int GetMaxHP()
    {
        return data.baseMaxHP + Mathf.FloorToInt(level * 8f);
    }

    public int GetAttack()
    {
        return data.baseAttack + Mathf.FloorToInt(level * 1.5f);
    }

    public int GetDefense()
    {
        return data.baseDefense + Mathf.FloorToInt(level * 1.2f);
    }

    public int GetSpeed()
    {
        return data.baseSpeed + Mathf.FloorToInt(level * 0.5f);
    }

    public int GetSpecialAttack()
    {
        return data.baseSpecialAttack + Mathf.FloorToInt(level * 1.5f);
    }

    public int GetExpReward()
    {
        return data.expGiven + Mathf.FloorToInt(level * 2f);
    }

    public int GetGoldReward()
    {
        return data.goldGiven + Mathf.FloorToInt(level * 1f);
    }

    public bool TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);
        return currentHP <= 0;
    }

    public void Heal(int healAmount)
    {
        currentHP += healAmount;
        currentHP = Mathf.Min(currentHP, GetMaxHP());
    }

    public bool IsDefeated()
    {
        return currentHP <= 0;
    }

    public float GetHPPercentage()
    {
        return (float)currentHP / GetMaxHP();
    }

    public void FullHeal()
    {
        currentHP = GetMaxHP();
    }
}
