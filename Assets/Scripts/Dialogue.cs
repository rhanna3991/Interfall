using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    public float blinkSpeed = 0.5f;
    private int index;
    private Animator dialogueAnimator;
    private bool isTextComplete = false;
    private Coroutine blinkCoroutine;
    
    // Callback for when dialogue completes
    public System.Action OnDialogueComplete;

    void Start()
    {
        // Initialize dialogue system for battlescene
        dialogueAnimator = GetComponent<Animator>();
        
        // Explicitly set text direction to left to right to prevent reversal issues
        if (textComponent != null)
        {
            textComponent.isRightToLeftText = false;
        }
        
        textComponent.text = string.Empty;
        index = 0;
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) && isTextComplete)
        {
            NextLine();
        }
    }

    void NextLine()
    {
        // Stop blinking indicator
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        isTextComplete = false;

        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            // Try to trigger exit animation if animator is available
            if (dialogueAnimator != null && dialogueAnimator.runtimeAnimatorController != null)
            {
                try
                {
                    dialogueAnimator.SetTrigger("Exit");
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("Animator trigger 'Exit' not found: " + e.Message);
                }
            }
            
            // Trigger callback when dialogue completes
            OnDialogueComplete?.Invoke();
        }
    }

    public IEnumerator TypeLine()
    {
        string fullText = lines[index];
        string currentText = string.Empty;

        foreach (char c in fullText.ToCharArray())
        {
            currentText += c;
            textComponent.text = currentText;

            yield return new WaitForSeconds(textSpeed);
        }

        // Text is complete, start blinking indicator
        isTextComplete = true;
        blinkCoroutine = StartCoroutine(BlinkIndicator());
    }

    IEnumerator BlinkIndicator()
    {
        while (true)
        {
            textComponent.text = lines[index] + " ▼";
            yield return new WaitForSeconds(blinkSpeed);
            textComponent.text = lines[index];
            yield return new WaitForSeconds(blinkSpeed);
        }
    }
    

    public void SetDialogueLines(string[] newLines)
    {
        lines = newLines;
        index = 0;
        textComponent.text = string.Empty;
    }
    
    // Method to start dialogue programmatically for battle system
    public void StartBattleDialogue()
    {
        
        dialogueAnimator = GetComponent<Animator>();
        
        // Ensure text direction is set correctly
        if (textComponent != null)
        {
            textComponent.isRightToLeftText = false;
        }
        
        StartCoroutine(BattleDialogueSequence());
    }
    
    // Method for quick damage messages without animations
    public void StartQuickDialogue()
    {
        dialogueAnimator = GetComponent<Animator>();
        
        // Ensure text direction is set correctly
        if (textComponent != null)
        {
            textComponent.isRightToLeftText = false;
        }
        
        StartCoroutine(QuickDialogueSequence());
    }
    
    IEnumerator QuickDialogueSequence()
    {
        // Skip animations and go straight to typing
        yield return StartCoroutine(TypeLine());
    }
    
    IEnumerator BattleDialogueSequence()
    {
        // Trigger entry animation
        if (dialogueAnimator != null)
        {
            dialogueAnimator.SetTrigger("Entry");
            yield return new WaitForSeconds(dialogueAnimator.GetCurrentAnimatorStateInfo(0).length);
        }
        
        // Start typing directly instead of starting another coroutine
        yield return StartCoroutine(TypeLine());
    }
}
