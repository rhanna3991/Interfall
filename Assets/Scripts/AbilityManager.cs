using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    [Header("Fireball System")]
    public GameObject fireballPrefab; // The fireball prefab to instantiate
    public Transform fireballSpawnPoint; // The FireballSpawn GameObject transform
    public Animator fireballAnimator;
    
    [Header("Battle References")]
    public BattleManager battleManager;
    public UIBattleManager uiBattleManager;
    
    void Start()
    {
        // Get references if not assigned
        if (battleManager == null)
            battleManager = FindObjectOfType<BattleManager>();
        if (uiBattleManager == null)
            uiBattleManager = FindObjectOfType<UIBattleManager>();
    }
    
    // Called when an ability is selected in the MagicMenu
    public void CastAbility(LearnableAbility ability)
    {
        if (ability == null)
        {
            Debug.LogError("Ability is null!");
            return;
        }
        
        Debug.Log($"Casting ability: {ability.abilityName}");
        
        // Handle different ability types
        switch (ability.abilityName)
        {
            case "Fireball":
                CastFireball();
                break;
            default:
                Debug.LogWarning($"Unknown ability: {ability.abilityName}");
                break;
        }
    }
    
    private void CastFireball()
    {
        if (fireballPrefab == null)
        {
            Debug.LogError("Fireball prefab not assigned!");
            return;
        }
        
        if (fireballSpawnPoint == null)
        {
            Debug.LogError("Fireball spawn point not assigned!");
            return;
        }
        
        // Instantiate the fireball at the spawn point with its prefab rotation
        GameObject fireball = Instantiate(fireballPrefab, fireballSpawnPoint.position, fireballPrefab.transform.rotation);
        Debug.Log("Fireball instantiated at spawn point with prefab rotation!");
        
        // Trigger the fireball casting animation
        if (fireballAnimator != null)
        {
            fireballAnimator.SetTrigger("FireballCast");
            Debug.Log("Fireball casting animation triggered!");
        }
    }
}
