using UnityEngine;

public abstract class Ability : ScriptableObject
{
    [Header("Ability Info")]
    public string abilityName;
    [TextArea] public string abilityDescription;
    public int manaCost = 5;
    public int baseDamage = 10;
    public int unlockLevel = 1;
    public float animationDuration = 1.5f;

    [Header("Visuals & Effects")]
    public GameObject effectPrefab;

    // Called when the player activates this ability in battle.
    public abstract void Activate(BattleManager battleManager);
}
