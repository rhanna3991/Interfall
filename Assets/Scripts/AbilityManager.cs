using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [Header("References")]
    public BattleManager battleManager;
    public UIBattleManager uiBattleManager;

    private void Awake()
    {
        if (battleManager == null) battleManager = FindObjectOfType<BattleManager>();
        if (uiBattleManager == null) uiBattleManager = FindObjectOfType<UIBattleManager>();
    }

    public void CastAbility(Ability ability)
    {
        if (ability == null)
        {
            Debug.LogError("Tried to cast a null ability!");
            return;
        }

        if (battleManager == null)
        {
            Debug.LogError("BattleManager reference missing!");
            return;
        }
        
        // Check if player has enough mana
        if (!battleManager.CanCastAbility(ability))
        {
            Debug.Log($"Not enough mana to cast {ability.abilityName}! Need {ability.manaCost} mana.");
            return;
        }
        
        // Deduct mana cost
        battleManager.DeductManaCost(ability);
        
        // Execute ability logic
        ability.Activate(battleManager);
    }
}
