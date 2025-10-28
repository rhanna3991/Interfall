using UnityEngine;
using System.Collections;

public class StageController : MonoBehaviour
{
    [Header("References")]
    public UIBattleManager battleManager;
    public EnemyDatabase enemyDatabase;
    public PlayerUI playerUI;

    [Header("Settings")]
    public int startingStage = 1;
    public float stageDelay = 1.5f;

    private int currentStage;
    private bool isGameOver = false;

    void Start()
    {
        currentStage = startingStage;
        
        // Initialize stage UI
        if (playerUI != null)
        {
            playerUI.SetStage(currentStage);
        }
        
        StartCoroutine(StartStageRoutine());
    }

    IEnumerator StartStageRoutine()
    {
        while (!isGameOver)
        {
            EnemyStats enemyData = enemyDatabase.GetRandomEnemy(currentStage);
            Debug.Log($"--- Starting Stage {currentStage} against {enemyData.enemyName} ---");

            yield return StartCoroutine(battleManager.StartBattle(enemyData, currentStage));

            // Clear enemy sprite after battle with delay to allow animations to finish
            StartCoroutine(ClearAfterDelay());

            yield return new WaitForSeconds(stageDelay);

            if (battleManager.PlayerDefeated)
            {
                Debug.Log("Game Over — player defeated!");
                isGameOver = true;
            }
            else
            {
                currentStage++;
                Debug.Log($"Player won! Moving to stage {currentStage}...");
                
                // Update stage UI
                if (playerUI != null)
                {
                    playerUI.SetStage(currentStage);
                }
            }
        }

        Debug.Log("Game loop ended.");
    }
    
    IEnumerator ClearAfterDelay(float delay = 0.75f)
    {
        yield return new WaitForSeconds(delay);
        battleManager.ClearEnemySprite();
    }
    
    public int GetCurrentStage()
    {
        return currentStage;
    }
}
