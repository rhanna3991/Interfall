using UnityEngine;

public enum EnemyStatType
{
    MaxHP,
    Attack,
    Defense,
    Speed,
    SpecialAttack
}

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Interfall/Enemy")]
public class EnemyStats : ScriptableObject
{
    [Header("Enemy Info")]
    public string enemyName;
    public Sprite enemySprite;

    [Header("Base Stats")]
    public int baseMaxHP = 100;
    public int baseAttack = 10;
    public int baseDefense = 5;
    public int baseSpeed = 10;
    public int baseSpecialAttack = 10;

    [Header("Rewards")]
    public int expGiven = 25;
    public int goldGiven = 10;
    

}


