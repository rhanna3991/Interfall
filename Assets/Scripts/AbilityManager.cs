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
        
        // Execute logic
        ability.Activate(battleManager);
    }
}
