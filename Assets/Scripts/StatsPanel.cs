using UnityEngine;
using TMPro;

public class StatsPanel : MonoBehaviour
{
    public GameObject statsPanel;
    public TextMeshProUGUI statsText;

    [Header("Player Data")]
    public CharacterStats playerStats;
    public int currentLevel = 1;
    public int currentHP;

    private bool isStatsVisible = false;

    void Start()
    {
        // Hide stats panel at start
        if (statsPanel != null)
        {
            statsPanel.SetActive(false);
        }

        // Initialize current HP if player stats exist
        if (playerStats != null)
        {
            currentHP = playerStats.GetStatAtLevel(StatType.MaxHP, currentLevel);
        }
    }

    void Update()
    {
        // Toggle stats display when Tab is pressed
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleStatsDisplay();
        }
    }

    void ToggleStatsDisplay()
    {
        isStatsVisible = !isStatsVisible;

        if (statsPanel != null)
        {
            statsPanel.SetActive(isStatsVisible);
        }

        // Update the stats text when showing
        if (isStatsVisible)
        {
            UpdateStatsDisplay();
        }
    }

    void UpdateStatsDisplay()
    {
        if (playerStats == null || statsText == null)
        {
            return;
        }

        // Get all stats at current level
        int maxHP = playerStats.GetStatAtLevel(StatType.MaxHP, currentLevel);
        int attack = playerStats.GetStatAtLevel(StatType.Attack, currentLevel);
        int defense = playerStats.GetStatAtLevel(StatType.Defense, currentLevel);
        int speed = playerStats.GetStatAtLevel(StatType.Speed, currentLevel);
        int specialAttack = playerStats.GetStatAtLevel(StatType.SpecialAttack, currentLevel);

        // Format the stats display
        string statsDisplay = $"<b>{playerStats.characterName}</b>\n\n";
        statsDisplay += $"Level: {currentLevel}\n\n";
        statsDisplay += $"HP: {currentHP} / {maxHP}\n";
        statsDisplay += $"Attack: {attack}\n";
        statsDisplay += $"Defense: {defense}\n";
        statsDisplay += $"Speed: {speed}\n";
        statsDisplay += $"Special Attack: {specialAttack}";

        statsText.text = statsDisplay;
    }


    public void SetCurrentHP(int hp)
    {
        if (playerStats != null)
        {
            int maxHP = playerStats.GetStatAtLevel(StatType.MaxHP, currentLevel);
            currentHP = Mathf.Clamp(hp, 0, maxHP);
        }
    }

    public void LevelUp()
    {
        currentLevel++;
        
        // Update display if currently visible
        if (isStatsVisible)
        {
            UpdateStatsDisplay();
        }
    }
}

