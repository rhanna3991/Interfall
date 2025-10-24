using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MagicMenu : MonoBehaviour
{
    [Header("UI References")]
    public Transform abilitiesContainer;     // Container (MagicAbilities)
    public TMP_Text descriptionText;         // Description box
    public GameObject abilityTextPrefab;     // Prefab for each ability (TMP_Text)
    
    private List<TMP_Text> abilityTexts = new List<TMP_Text>();
    private List<LearnableAbility> abilities = new List<LearnableAbility>();
    private int selectedIndex = 0;

    private float blinkTimer = 0f;
    private bool blinkVisible = true;
    private const float blinkInterval = 0.5f;

    void Update()
    {
        if (abilities.Count == 0) return;

        // Handle input
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = (selectedIndex + 1) % abilities.Count;
            UpdateSelection();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = (selectedIndex - 1 + abilities.Count) % abilities.Count;
            UpdateSelection();
        }

        // Handle blink effect
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkInterval)
        {
            blinkTimer = 0f;
            blinkVisible = !blinkVisible;
            UpdateBlinkIndicator();
        }
    }

    // Populates the magic menu with the given list of abilities
    public void Populate(List<LearnableAbility> unlockedAbilities)
    {
        Clear();

        abilities = unlockedAbilities;

        foreach (var ability in unlockedAbilities)
        {
            GameObject newTextObj = Instantiate(abilityTextPrefab, abilitiesContainer);
            TMP_Text text = newTextObj.GetComponent<TMP_Text>();
            text.text = ability.abilityName;

            // Offset position down for each ability
            RectTransform rt = newTextObj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0, -abilityTexts.Count * 25f);

            abilityTexts.Add(text);
        }

        // Default to first ability
        selectedIndex = 0;
        UpdateSelection();
        
        // Force layout rebuild to ensure proper positioning
        LayoutRebuilder.ForceRebuildLayoutImmediate(abilitiesContainer.GetComponent<RectTransform>());
    }

    private void UpdateSelection()
    {
        if (abilities.Count == 0) return;

        // Update description
        var selectedAbility = abilities[selectedIndex];
        descriptionText.text = $"{selectedAbility.abilityDescription}\n\nBase Power: {selectedAbility.baseDamage}\nMana Cost: {selectedAbility.manaCost}";

        // Update ability text indicators
        for (int i = 0; i < abilityTexts.Count; i++)
        {
            if (i == selectedIndex)
                abilityTexts[i].text = $"▶ {abilities[i].abilityName}";
            else
                abilityTexts[i].text = $"  {abilities[i].abilityName}";
        }

        // Reset blink
        blinkVisible = true;
        blinkTimer = 0f;
    }

    // Causes the blink indicator next to a selected ability to flash on and off
    private void UpdateBlinkIndicator()
    {
        if (abilities.Count == 0) return;

        for (int i = 0; i < abilityTexts.Count; i++)
        {
            if (i == selectedIndex)
            {
                abilityTexts[i].text = blinkVisible
                    ? $"▶ {abilities[i].abilityName}"
                    : $"  {abilities[i].abilityName}";
            }
        }
    }

    // Clears the magic menu
    public void Clear()
    {
        foreach (Transform child in abilitiesContainer)
            Destroy(child.gameObject);
        abilityTexts.Clear();
        abilities.Clear();
    }
}
