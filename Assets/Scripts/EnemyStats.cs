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
    public string battleStartMessage = "dares to challenge you!";
    public string description; // Optional flavor text
    
    [Header("Stage Tier")]
    [Range(1, 10)] public int stageTier = 1; // How late in the run it appears
    
    [Header("Visual Settings")]
    public Vector3 scale = Vector3.one; // Enemy size multiplier
    
    [Header("Collider Settings")]
    public float colliderSizeMultiplier = 1.0f; // Collider size multiplier
    public Vector2 colliderOffset = Vector2.zero; // Collider position offset

    [Header("Base Stats")]
    public int baseMaxHP = 100;
    public int baseAttack = 10;
    public int baseDefense = 5;
    public int baseSpeed = 10;
    public int baseSpecialAttack = 10;

    [Header("Rewards")]
    public int expGiven = 25;
    public int goldGiven = 10;
    
    [Header("Animation Triggers")]
    public string attackTrigger = "EnemyAttack";
    public string hitFlashTrigger = "HitFlash";
    public string deathTrigger = "EnemyDeath";
    public float attackAnimationDuration = 1.0f;
    
    [Header("Animator Controller")]
    public RuntimeAnimatorController animatorController;

}


