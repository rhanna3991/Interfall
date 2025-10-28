using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    
    [Header("EXP and Level UI")]
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private string levelFormat = "Level: {0}";
    
    private BattleManager battleManager;
    private StageController stageController;
    private CharacterStats playerStats;
    private int currentStage = 1;
    
    // EXP and Level tracking
    private int currentExp = 0;
    private int currentLevel = 1;
    
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
        
        // Initialize EXP UI
        UpdateExpUI();
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
            int maxMP = battleManager.playerMaxMP;
            mpText.text = string.Format(mpFormat, currentMP, maxMP);
        }
    }
    
    // Method to be called from BattleManager when stats change
    public void OnPlayerStatsChanged()
    {
        UpdatePlayerStats();
    }
    
    // EXP and Level Methods
    public void GainExp(int expAmount)
    {
        currentExp += expAmount;
        
        // Check for level ups
        while (currentExp >= GetExpRequiredForCurrentLevel())
        {
            LevelUp();
        }
        
        UpdateExpUI();
    }
    
    private void LevelUp()
    {
        currentExp -= GetExpRequiredForCurrentLevel();
        currentLevel++;
        
        // Update battle manager's player level
        if (battleManager != null)
        {
            battleManager.playerLevel = currentLevel;
            
            // Update max HP and MP when leveling up
            if (playerStats != null)
            {
                int newMaxHP = playerStats.GetStatAtLevel(StatType.MaxHP, currentLevel);
                int newMaxMP = playerStats.GetStatAtLevel(StatType.MaxMana, currentLevel);
                
                // Increase current HP and MP proportionally
                float hpRatio = (float)battleManager.playerCurrentHP / battleManager.playerMaxHP;
                float mpRatio = (float)battleManager.playerCurrentMana / battleManager.playerMaxMP;
                
                battleManager.playerMaxHP = newMaxHP;
                battleManager.playerMaxMP = newMaxMP;
                battleManager.playerCurrentHP = Mathf.RoundToInt(newMaxHP * hpRatio);
                battleManager.playerCurrentMana = Mathf.RoundToInt(newMaxMP * mpRatio);
                
                // Ensure we don't go below 1 HP/MP
                battleManager.playerCurrentHP = Mathf.Max(1, battleManager.playerCurrentHP);
                battleManager.playerCurrentMana = Mathf.Max(0, battleManager.playerCurrentMana);
                
                Debug.Log($"Level up! Now level {currentLevel}. New stats - HP: {battleManager.playerCurrentHP}/{battleManager.playerMaxHP}, MP: {battleManager.playerCurrentMana}/{battleManager.playerMaxMP}");
            }
        }
        
        UpdateExpUI();
        UpdatePlayerStats(); // Refresh HP/MP display
    }
    
    private int GetExpRequiredForCurrentLevel()
    {
        if (playerStats != null)
        {
            return playerStats.GetExpRequiredForLevel(currentLevel);
        }
        return 100; // Default fallback
    }
    
    private void UpdateExpUI()
    {
        // Update EXP slider
        if (expSlider != null)
        {
            int expRequired = GetExpRequiredForCurrentLevel();
            expSlider.value = expRequired > 0 ? (float)currentExp / expRequired : 1f;
        }
        
        // Update level text
        if (levelText != null)
        {
            levelText.text = string.Format(levelFormat, currentLevel);
        }
    }
    
    // Public getters for external access
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    
    public int GetCurrentExp()
    {
        return currentExp;
    }
    
    // Method to sync with BattleManager's level (useful for initialization)
    public void SyncWithBattleManager()
    {
        if (battleManager != null)
        {
            currentLevel = battleManager.playerLevel;
            UpdateExpUI();
        }
    }
}
