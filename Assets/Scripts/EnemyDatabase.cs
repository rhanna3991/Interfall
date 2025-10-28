using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "EnemyDatabase", menuName = "Interfall/Enemy Database")]
public class EnemyDatabase : ScriptableObject
{
    [Header("Enemy Collection")]
    public List<EnemyStats> allEnemies = new List<EnemyStats>();

    private static EnemyDatabase _instance;
    public static EnemyDatabase Instance
    {
        get
        {
            if (_instance == null)
                _instance = Resources.Load<EnemyDatabase>("EnemyDatabase");
            return _instance;
        }
    }

    public EnemyStats GetRandomEnemy(int stage)
    {
        // Filter enemies that can appear at this stage or earlier
        var possibleEnemies = allEnemies.Where(e => e.stageTier <= stage).ToList();
        
        // If no enemies match the stage, fall back to all enemies
        if (possibleEnemies.Count == 0)
        {
            possibleEnemies = allEnemies;
            Debug.LogWarning($"No enemies found for stage {stage}, using all enemies");
        }
        
        // Select random enemy from filtered list
        EnemyStats chosenEnemy = possibleEnemies[Random.Range(0, possibleEnemies.Count)];
        Debug.Log($"Selected enemy: {chosenEnemy.enemyName} (Tier {chosenEnemy.stageTier}) for stage {stage}");
        
        return chosenEnemy;
    }

    public List<EnemyStats> GetEnemiesForTier(int stageTier)
    {
        return allEnemies.Where(e => e.stageTier == stageTier).ToList();
    }

    public EnemyStats GetEnemyByName(string enemyName)
    {
        return allEnemies.FirstOrDefault(e => e.enemyName == enemyName);
    }

    [ContextMenu("Validate Database")]
    public void ValidateDatabase()
    {
        Debug.Log($"Enemy Database Validation:");
        Debug.Log($"Total enemies: {allEnemies.Count}");
        
        for (int tier = 1; tier <= 10; tier++)
        {
            var tierEnemies = GetEnemiesForTier(tier);
            Debug.Log($"Tier {tier}: {tierEnemies.Count} enemies");
            
            if (tierEnemies.Count == 0)
            {
                Debug.LogWarning($"No enemies found for tier {tier}!");
            }
        }
    }
}
