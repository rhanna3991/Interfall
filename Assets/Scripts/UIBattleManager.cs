using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIBattleManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button attackButton;
    [SerializeField] private Button magicButton;
    [SerializeField] private Button itemsButton;
    [SerializeField] private Button fleeButton;
    [SerializeField] private GameObject battleOptions;
    [SerializeField] private GameObject playerCard;
    [SerializeField] private GameObject battleBoxDialogue;
    [SerializeField] private Dialogue dialogueScript;
    [SerializeField] private Slider rightSideBar; // Mana bar slider
    
    [Header("Animation References")]
    [SerializeField] private Animator battleTransitionAnimator;
    [SerializeField] private GameObject transitionContainer;
    [SerializeField] private Animator[] slashAnimators; // Array to hold multiple slash attack animators
    [SerializeField] private GameObject[] slashGameObjects; // Array to hold the slash GameObjects (for show/hide)
    [SerializeField] private Animator enemyAnimator;
    [SerializeField] private Animator attackVisualAnimator;
    [SerializeField] private Animator magicVisualAnimator;
    [SerializeField] private Animator itemsVisualAnimator;
    [SerializeField] private Animator fleeVisualAnimator;
    
    [Header("Animation Timings")]
    [SerializeField] private float defaultButtonAnimationDuration = 0.35f;
    [SerializeField] private float magicButtonAnimationDuration = 0.15f;
    [SerializeField] private float fallbackBattleStartDelay = 2f;
    [SerializeField] private float attackCooldownDuration = 1.0f; // Cooldown between attacks
    
    // Button spam prevention
    private bool isAttackInProgress = false;
    private bool isAnyActionInProgress = false;
    
    // Animation trigger/state constants
    private const string ANIM_PRESSED = "Pressed";
    private const string ANIM_HIGHLIGHTED = "Highlighted";
    private const string ANIM_NORMAL = "Normal";
    private const string ANIM_FIRE_SLASH = "FireSlash";
    private const string ANIM_HIT_FLASH = "HitFlash";
    
    // Reference to BattleManager for communication
    [SerializeField] private BattleManager battleManager;
    
    // Reference to TransitionUI for animation events
    [SerializeField] private TransitionUI transitionUI;
    
    // Reference to MagicMenu for controlling it
    [SerializeField] private MagicMenu magicMenu;
    
    private Button[] allBattleButtons;
    
    void Awake()
    {
        // Cache all buttons for easier management
        allBattleButtons = new Button[] { attackButton, magicButton, itemsButton, fleeButton };
    }
    
    void Start()
    {
        // Validate BattleManager reference
        if (battleManager == null)
        {
            Debug.LogError("BattleManager reference not assigned in Inspector!");
        }
        
        SetupButtons();
        HideAllSlashEffects();
    }
    
    private void SetupButtons()
    {
        // Set up buttons using helper method
        SetupButton(attackButton, OnAttackButton, OnAttackButtonHoverEnter, OnAttackButtonHoverExit);
        SetupButton(magicButton, OnMagicButton, OnMagicButtonHoverEnter, OnMagicButtonHoverExit);
        SetupButton(itemsButton, OnItemsButton, OnItemsButtonHoverEnter, OnItemsButtonHoverExit);
        SetupButton(fleeButton, OnFleeButton, OnFleeButtonHoverEnter, OnFleeButtonHoverExit);
    }
    
    // Set up buttons for the battle UI
    private void SetupButton(Button button, UnityEngine.Events.UnityAction onClick, 
        Action onHoverEnter, Action onHoverExit)
    {
        if (button == null)
        {
            Debug.LogWarning($"Button reference is null in {nameof(UIBattleManager)}");
            return;
        }

        button.onClick.AddListener(onClick);

        // Add event triggers to the buttons
        var trigger = button.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();
        AddEventTrigger(trigger, EventTriggerType.PointerEnter, onHoverEnter);
        AddEventTrigger(trigger, EventTriggerType.PointerExit, onHoverExit);
    }

    // Add event triggers to the buttons
    private void AddEventTrigger(EventTrigger trigger, EventTriggerType type, Action callback)
    {
        if (callback == null) return;
        
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(_ => callback());
        trigger.triggers.Add(entry);
    }
    
    public void OnAttackButton()
    {
        // Prevent spam clicking during attack or any other action
        if (isAttackInProgress || isAnyActionInProgress)
        {
            Debug.Log("Attack button clicked but action already in progress - ignoring spam click");
            return;
        }
        
        // Mark attack as in progress
        isAttackInProgress = true;
        isAnyActionInProgress = true;
        
        // Disable all buttons during attack
        SetButtonsInteractable(false);
        
        StartCoroutine(PlayButtonAnimationThenExecute(
            attackVisualAnimator, 
            defaultButtonAnimationDuration,
            () => {
                battleManager?.HandlePlayerAttack();
                // Start cooldown after attack
                StartCoroutine(AttackCooldown());
            }
        ));
    }
    
    public void OnMagicButton()
    {
        StartCoroutine(PlayButtonAnimationThenExecute(
            magicVisualAnimator, 
            magicButtonAnimationDuration,
            () => { 
                if (transitionUI != null)
                {
                    transitionUI.StartMainToMagicTransition();
                }
                else
                {
                    Debug.LogWarning("TransitionUI reference not set in UIBattleManager!");
                }
            }
        ));
    }
    
    public void OnItemsButton()
    {
        StartCoroutine(PlayButtonAnimationThenExecute(
            itemsVisualAnimator, 
            defaultButtonAnimationDuration,
            () => { 
                // TODO: Implement items system
                Debug.Log("Items button clicked - not implemented yet");
            }
        ));
    }
    
    public void OnFleeButton()
    {
        StartCoroutine(PlayButtonAnimationThenExecute(
            fleeVisualAnimator, 
            defaultButtonAnimationDuration,
            () => { 
                // TODO: Implement flee system
                Debug.Log("Flee button clicked - not implemented yet");
            }
        ));
    }
    
    // Play button animation then execute callback
    private IEnumerator PlayButtonAnimationThenExecute(Animator animator, float duration, Action onComplete)
    {
        TriggerAnimation(animator, ANIM_PRESSED);
        
        // Wait for the animation to finish
        yield return new WaitForSeconds(duration);
        
        onComplete?.Invoke();
    }
    
    // Attack cooldown to prevent rapid successive attacks
    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldownDuration);
        isAttackInProgress = false;
        isAnyActionInProgress = false;
        
        // Re-enable buttons after cooldown
        SetButtonsInteractable(true);
    }
    
    // Button hover events
    public void OnAttackButtonHoverEnter() => SetButtonState(attackVisualAnimator, ANIM_HIGHLIGHTED);
    public void OnAttackButtonHoverExit() => SetButtonState(attackVisualAnimator, ANIM_NORMAL);
    public void OnMagicButtonHoverEnter() => SetButtonState(magicVisualAnimator, ANIM_HIGHLIGHTED);
    public void OnMagicButtonHoverExit() => SetButtonState(magicVisualAnimator, ANIM_NORMAL);
    
    public void OnItemsButtonHoverEnter() => SetButtonState(itemsVisualAnimator, ANIM_HIGHLIGHTED);
    public void OnItemsButtonHoverExit() => SetButtonState(itemsVisualAnimator, ANIM_NORMAL);
    public void OnFleeButtonHoverEnter() => SetButtonState(fleeVisualAnimator, ANIM_HIGHLIGHTED);
    public void OnFleeButtonHoverExit() => SetButtonState(fleeVisualAnimator, ANIM_NORMAL);
    
    public void UpdateBattleUI(int damage, CharacterStats playerStats, EnemyStats enemyStats)
    {
        if (!ValidateReferences(playerStats, enemyStats)) return;
        
        // Disable all button interactions during damage display
        SetButtonsInteractable(false);
        SetActiveSafe(playerCard, false);
        
        // Show damage text in battle dialogue box
        ShowDamageDialogue(damage, playerStats, enemyStats);
    }
    
    private void ShowDamageDialogue(int damage, CharacterStats playerStats, EnemyStats enemyStats)
    {
        if (!ValidateDialogueSystem()) return;
        
        SetActiveSafe(battleBoxDialogue, true);
        
        // Create damage message
        string damageMessage = $"{playerStats.characterName} has dealt {damage} damage to {enemyStats.enemyName}!";
        dialogueScript.SetDialogueLines(new[] { damageMessage });
        
        // Set up callback to hide dialogue and restore UI when complete
        dialogueScript.OnDialogueComplete = () => {
            SetActiveSafe(battleBoxDialogue, false);
            
            // Restore battle UI elements
            SetButtonsInteractable(true);
            SetActiveSafe(playerCard, true);
        };
        
        dialogueScript.StartQuickDialogue();
    }
    
    // Public method to show custom damage dialogue message
    public void ShowDamageDialogue(string customMessage)
    {
        if (!ValidateDialogueSystem()) return;
        
        // Hide magic menu if it's active
        if (transitionUI != null && transitionUI.magicMenu != null && transitionUI.magicMenu.activeSelf)
        {
            transitionUI.DeactivateMagicMenu();
        }
        
        // Disable buttons and hide player card during dialogue
        SetButtonsInteractable(false);
        SetActiveSafe(playerCard, false);
        SetActiveSafe(battleBoxDialogue, true);
        
        dialogueScript.SetDialogueLines(new[] { customMessage });
        
        // Set up callback to hide dialogue and restore UI when complete
        dialogueScript.OnDialogueComplete = () => {
            SetActiveSafe(battleBoxDialogue, false);
            
            // Always return to the default main menu
            if (transitionUI != null)
            {
                transitionUI.StartDefaultMenuTransition();
            }

            // Re-enable buttons safely
            SetButtonsInteractable(true);
            SetActiveSafe(playerCard, true);
        };
        
        dialogueScript.StartQuickDialogue();
    }
    
    // Returns to main battle options after an action completes
    private void ReturnToMainBattleOptions()
    {
        transitionUI?.StartDefaultMenuTransition();
    }
    
    // Set the interactability of all battle buttons
    public void SetButtonsInteractable(bool interactable)
    {
        foreach (var button in allBattleButtons)
        {
            SetButtonInteractable(button, interactable);
        }
    }
    
    // Play the slash animation for the current party member
    public void PlaySlashAnimation(int currentPartyMemberIndex)
    {
        // Check if we have slash animators and current index is valid
        if (!IsValidSlashIndex(currentPartyMemberIndex)) return;
        
        var slashObject = slashGameObjects[currentPartyMemberIndex];
        var slashAnimator = slashAnimators[currentPartyMemberIndex];
        
        if (slashObject != null && slashAnimator != null)
        {
            // Show the slash effect and trigger animation
            slashObject.SetActive(true);
            TriggerAnimation(slashAnimator, ANIM_FIRE_SLASH);
            
            // Trigger enemy hit flash
            TriggerAnimation(enemyAnimator, ANIM_HIT_FLASH);
        }
    }
    
    // Hide all slash effects at the start
    public void HideAllSlashEffects()
    {
        if (slashGameObjects == null) return;
        
        foreach (var slashObject in slashGameObjects)
        {
            SetActiveSafe(slashObject, false);
        }
    }
    
    private void TriggerAnimation(Animator animator, string triggerName)
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }
    
    private void SetButtonState(Animator animator, string state)
    {
        TriggerAnimation(animator, state);
    }
    
    public void StartBattleSequence(EnemyStats enemyStats)
    {
        if (enemyStats == null)
        {
            Debug.LogError("Cannot start battle sequence: EnemyStats is null");
            return;
        }
        
        // Disable battle UI elements initially
        SetActiveSafe(battleOptions, false);
        SetActiveSafe(playerCard, false);
        
        // Show dialogue box and set up dialogue
        if (ValidateDialogueSystem())
        {
            ShowBattleStartDialogue(enemyStats);
        }
        else
        {
            // Fallback if dialogue system isn't available
            Debug.Log($"{enemyStats.enemyName} dares to challenge you!");
            StartCoroutine(FallbackBattleStart());
        }
    }
    
    private void ShowBattleStartDialogue(EnemyStats enemyStats)
    {
        SetActiveSafe(battleBoxDialogue, true);
        
        // Set up dialogue lines with enemy name and custom message
        dialogueScript.SetDialogueLines(new[] { enemyStats.battleStartMessage });
        dialogueScript.OnDialogueComplete = OnBattleDialogueComplete;
        dialogueScript.StartBattleDialogue();
    }
    
    // Fallback for when the dialogue system is not available
    private IEnumerator FallbackBattleStart()
    {
        yield return new WaitForSeconds(fallbackBattleStartDelay);
        OnBattleDialogueComplete();
    }
    
    private void OnBattleDialogueComplete()
    {
        // Hide dialogue box
        SetActiveSafe(battleBoxDialogue, false);
        
        // Show battle UI elements
        SetActiveSafe(battleOptions, true);
        SetActiveSafe(playerCard, true);
    }
    
    public void TransitionSequence()
    {
        // Enable transition container and play battle entry animation
        SetActiveSafe(transitionContainer, true);
    }
    
    private bool ValidateReferences(CharacterStats playerStats, EnemyStats enemyStats)
    {
        if (playerStats == null || enemyStats == null)
        {
            Debug.LogError("PlayerStats or EnemyStats is null in UpdateBattleUI");
            return false;
        }
        return true;
    }
    
    private bool ValidateDialogueSystem()
    {
        return battleBoxDialogue != null && dialogueScript != null;
    }
    
    private bool IsValidSlashIndex(int index)
    {
        return slashAnimators != null && 
               slashGameObjects != null && 
               index >= 0 && 
               index < slashAnimators.Length && 
               index < slashGameObjects.Length;
    }
    
    // Set the active state of a game object safely
    private void SetActiveSafe(GameObject obj, bool active)
    {
        if (obj != null && obj.activeSelf != active)
        {
            obj.SetActive(active);
        }
    }
    
    private void SetButtonInteractable(Button button, bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
        }
    }
    
    // Update the mana bar slider
    public void UpdateManaBar(int currentMana, int maxMana)
    {
        if (rightSideBar != null && maxMana > 0)
        {
            rightSideBar.value = (float)currentMana / maxMana;
        }
    }
}