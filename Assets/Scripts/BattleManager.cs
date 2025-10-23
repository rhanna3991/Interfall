using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BattleManager : MonoBehaviour
{
    [Header("Player Data")]
    public CharacterStats playerStats;
    public int playerLevel = 1;
    
    [Header("Enemy Data")]
    public EnemyStats enemyStats;
    public int enemyCurrentHP;
    public int enemyMaxHP;
    
    [Header("UI References")]
    public Button attackButton;
    public Button magicButton;
    public Button itemsButton;
    public Button fleeButton;
    public GameObject battleOptions;
    public GameObject playerCard;
    public GameObject battleBoxDialogue;
    public Dialogue dialogueScript;
    
    public Animator battleTransitionAnimator;
    public GameObject transitionContainer;
    
    public Animator[] slashAnimators; // Array to hold multiple slash attack animators
    public GameObject[] slashGameObjects; // Array to hold the slash GameObjects (for show/hide)
    public Animator enemyAnimator;
    public Animator attackVisualAnimator;
    public Animator magicVisualAnimator;
    public int currentPartyMemberIndex = 0; // Which party member is currently attacking
    
    void Start()
    {
        // Initialize enemy HP
        if (enemyStats != null)
        {
            enemyMaxHP = enemyStats.baseMaxHP;
            enemyCurrentHP = enemyMaxHP;
        }
        
        // Set up attack button
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(OnAttackButton);
            
            // Add hover events for button animation
            var eventTrigger = attackButton.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = attackButton.gameObject.AddComponent<EventTrigger>();
            }
            
            // Pointer enter event
            var pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => { OnAttackButtonHoverEnter(); });
            eventTrigger.triggers.Add(pointerEnter);
            
            // Pointer exit event
            var pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnAttackButtonHoverExit(); });
            eventTrigger.triggers.Add(pointerExit);
        }
        
        // Set up magic button
        if (magicButton != null)
        {
            magicButton.onClick.AddListener(OnMagicButton);
            
            // Add hover events for button animation
            var eventTrigger = magicButton.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = magicButton.gameObject.AddComponent<EventTrigger>();
            }
            
            // Pointer enter event
            var pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => { OnMagicButtonHoverEnter(); });
            eventTrigger.triggers.Add(pointerEnter);
            
            // Pointer exit event
            var pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnMagicButtonHoverExit(); });
            eventTrigger.triggers.Add(pointerExit);
        }
        
        HideAllSlashEffects();
        
        TransitionSequence();
    }
    
    void TransitionSequence()
    {
        // Enable transition container and play battle entry animation
        if (transitionContainer != null)
        {
            transitionContainer.SetActive(true);
        }
        
        // Start battle dialogue sequence immediately
        StartCoroutine(BattleStartSequence());
    }
    
    public void OnAttackButton()
    {
        StartCoroutine(PlayAttackAnimationThenAttack());
    }
    
    private IEnumerator PlayAttackAnimationThenAttack()
    {
        if (attackVisualAnimator != null)
        {
            attackVisualAnimator.SetTrigger("Pressed");
        }
        
        // Wait for the animation to finish
        yield return new WaitForSeconds(0.35f);
        
        HandlePlayerAttack();
    }
    
    public void OnMagicButton()
    {
        StartCoroutine(PlayMagicAnimationThenMagic());
    }
    
    private IEnumerator PlayMagicAnimationThenMagic()
    {
        if (magicVisualAnimator != null)
        {
            magicVisualAnimator.SetTrigger("Pressed");
        }
        
        // Wait for the animation to finish
        yield return new WaitForSeconds(0.35f);
        
  
    }
    
    public void OnAttackButtonHoverEnter()
    {
        if (attackVisualAnimator != null)
        {
            attackVisualAnimator.SetTrigger("Highlighted");
        }
    }
    
    public void OnAttackButtonHoverExit()
    {
        if (attackVisualAnimator != null)
        {
            attackVisualAnimator.SetTrigger("Normal");
        }
    }
    
    public void OnMagicButtonHoverEnter()
    {
        if (magicVisualAnimator != null)
        {
            magicVisualAnimator.SetTrigger("Highlighted");
        }
    }
    
    public void OnMagicButtonHoverExit()
    {
        if (magicVisualAnimator != null)
        {
            magicVisualAnimator.SetTrigger("Normal");
        }
    }
    
    void HandlePlayerAttack()
    {
        if (playerStats == null || enemyStats == null) return;
        
        // Play slash animation for current party member
        PlaySlashAnimation();
        
        // Calculate and apply damage
        int damage = CalculatePlayerDamage();
        ApplyDamageToEnemy(damage);
        
        // Start delayed damage display
        StartCoroutine(DelayedDamageDisplay(damage));
    }
    
    IEnumerator DelayedDamageDisplay(int damage)
    {
        // Small delay before showing damage dialogue
        yield return new WaitForSeconds(0.05f);
        
        // Update UI and check battle end
        UpdateBattleUI(damage);
        CheckBattleEnd();
    }
    
    int CalculatePlayerDamage()
    {
        int playerAttack = playerStats.GetStatAtLevel(StatType.Attack, playerLevel);
        int enemyDefense = enemyStats.baseDefense;
        return Mathf.Max(1, playerAttack - enemyDefense); // Minimum 1 damage
    }
    
    void ApplyDamageToEnemy(int damage)
    {
        enemyCurrentHP -= damage;
        enemyCurrentHP = Mathf.Max(0, enemyCurrentHP); // Don't go below 0
        
        // Log HP update to console
        Debug.Log($"{enemyStats.enemyName} HP: {enemyCurrentHP}/{enemyMaxHP}");
    }
    
    void UpdateBattleUI(int damage)
    {
        // Disable all button interactions during damage display
        if (attackButton != null)
            attackButton.interactable = false;
        if (magicButton != null)
            magicButton.interactable = false;
        if (itemsButton != null)
            itemsButton.interactable = false;
        if (fleeButton != null)
            fleeButton.interactable = false;
        if (playerCard != null)
            playerCard.SetActive(false);
        
        // Show damage text in battle dialogue box
        if (battleBoxDialogue != null && dialogueScript != null && playerStats != null && enemyStats != null)
        {
            battleBoxDialogue.SetActive(true);
            
            // Create damage message
            string damageMessage = $"{playerStats.characterName} has dealt {damage} damage to {enemyStats.enemyName}!";
            string[] dialogueLines = { damageMessage };
            dialogueScript.SetDialogueLines(dialogueLines);
            
            // Set up callback to hide dialogue and restore UI when complete
            dialogueScript.OnDialogueComplete = () => {
                if (battleBoxDialogue != null)
                    battleBoxDialogue.SetActive(false);
                
                // Restore battle UI elements
                if (attackButton != null)
                    attackButton.interactable = true;
                if (magicButton != null)
                    magicButton.interactable = true;
                if (itemsButton != null)
                    itemsButton.interactable = true;
                if (fleeButton != null)
                    fleeButton.interactable = true;
                if (playerCard != null)
                    playerCard.SetActive(true);
            };
            
            dialogueScript.StartQuickDialogue();
        }
        
        // Also log to console for debugging
        Debug.Log($"{playerStats.characterName} attacks for {damage} damage!");
    }
    
    void CheckBattleEnd()
    {
        if (enemyCurrentHP <= 0)
        {
            Debug.Log(enemyStats.enemyName + " is defeated!");
        }
    }
    
    void PlaySlashAnimation()
    {
        // Check if we have slash animators and current index is valid
        if (slashAnimators != null && slashAnimators.Length > 0 && 
            currentPartyMemberIndex >= 0 && currentPartyMemberIndex < slashAnimators.Length)
        {
            Animator currentAnimator = slashAnimators[currentPartyMemberIndex];
            GameObject currentSlashObject = null;
            
            if (slashGameObjects != null && currentPartyMemberIndex < slashGameObjects.Length)
            {
                currentSlashObject = slashGameObjects[currentPartyMemberIndex];
            }
            
            if (currentAnimator != null && currentSlashObject != null)
            {
                // Show the slash effect and trigger animation
                currentSlashObject.SetActive(true);
                currentAnimator.SetTrigger("FireSlash");
                
                // Trigger enemy hit flash
                if (enemyAnimator != null)
                {
                    enemyAnimator.SetTrigger("HitFlash");
                }
            }
        }
    }
    
    // Method to switch between party members
    public void SetCurrentPartyMember(int index)
    {
        if (slashAnimators != null && index >= 0 && index < slashAnimators.Length)
        {
            currentPartyMemberIndex = index;
        }
    }
    
    // Hide all slash effects at the start
    void HideAllSlashEffects()
    {
        if (slashGameObjects != null)
        {
            foreach (GameObject slashObject in slashGameObjects)
            {
                if (slashObject != null)
                {
                    slashObject.SetActive(false);
                }
            }
        }
    }
    
    IEnumerator BattleStartSequence()
    {
        // Disable battle UI elements initially
        if (battleOptions != null)
            battleOptions.SetActive(false);
        if (playerCard != null)
            playerCard.SetActive(false);
        
        // Show dialogue box and set up dialogue
        if (battleBoxDialogue != null && dialogueScript != null && enemyStats != null)
        {
            battleBoxDialogue.SetActive(true);
            
            // Set up dialogue lines with enemy name and custom message
            string[] dialogueLines = { enemyStats.battleStartMessage };
            dialogueScript.SetDialogueLines(dialogueLines);
            
            dialogueScript.OnDialogueComplete = OnDialogueComplete;
            
            dialogueScript.StartBattleDialogue();
        }
        else
        {
            // Fallback if dialogue system isn't available
            Debug.Log($"{enemyStats.enemyName} dares to challenge you!");
            yield return new WaitForSeconds(2f);
            OnDialogueComplete();
        }
    }
    
    void OnDialogueComplete()
    {
        // Hide dialogue box
        if (battleBoxDialogue != null)
            battleBoxDialogue.SetActive(false);
        
        // Show battle UI elements
        if (battleOptions != null)
            battleOptions.SetActive(true);
        if (playerCard != null)
            playerCard.SetActive(true);
        
        Debug.Log($"{enemyStats.enemyName} HP: {enemyCurrentHP}/{enemyMaxHP}");
    }
    
}
