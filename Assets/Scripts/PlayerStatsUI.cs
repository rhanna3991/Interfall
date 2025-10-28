using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("Stage UI")]
    [SerializeField] private TextMeshProUGUI stageNumberText;
    [SerializeField] private string stagePrefix = "Stage: ";
    
    [Header("Player Stats UI")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI mpText;
    [SerializeField] private string hpFormat = "HP: {0}/{1}";
    [SerializeField] private string mpFormat = "MP: {0}/{1}";
    
    private BattleManager battleManager;
    private StageController stageController;
    private CharacterStats playerStats;
    private int currentStage = 1;
    
    void Start()
    {
        // Get references
        battleManager = FindObjectOfType<BattleManager>();
        stageController = FindObjectOfType<StageController>();
        
        if (battleManager != null)
        {
            playerStats = battleManager.playerStats;
            
            // Initial update of player stats
            UpdatePlayerStats();
        }
        
        // Initialize stage display
        UpdateStageDisplay();
    }
    
    void Update()
    {
        // Update player stats every frame
        UpdatePlayerStats();
    }
    
    // Stage UI Methods
    public void UpdateStageDisplay()
    {
        if (stageNumberText != null)
        {
            stageNumberText.text = stagePrefix + currentStage.ToString();
        }
    }
    
    public void SetStage(int stageNumber)
    {
        currentStage = stageNumber;
        UpdateStageDisplay();
    }
    
    public void IncrementStage()
    {
        currentStage++;
        UpdateStageDisplay();
    }
    
    public int GetCurrentStage()
    {
        return currentStage;
    }
    
    // Player Stats UI Methods
    public void UpdatePlayerStats()
    {
        if (battleManager == null || playerStats == null) return;
        
        // Update HP text
        if (hpText != null)
        {
            int currentHP = battleManager.playerCurrentHP;
            int maxHP = battleManager.playerMaxHP;
            hpText.text = string.Format(hpFormat, currentHP, maxHP);
        }
        
        // Update MP text
        if (mpText != null)
        {
            int currentMP = battleManager.playerCurrentMana;
            int maxMP = playerStats.GetStatAtLevel(StatType.MaxMana, battleManager.playerLevel);
            mpText.text = string.Format(mpFormat, currentMP, maxMP);
        }
    }
    
    // Method to be called from BattleManager when stats change
    public void OnPlayerStatsChanged()
    {
        UpdatePlayerStats();
    }
}
