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
    private PlayerMovement playerMovement;
    private bool isTextComplete = false;
    private Coroutine blinkCoroutine;
    
    // Callback for when dialogue completes
    public System.Action OnDialogueComplete;


    void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SampleScene")
        {
            // Get player movement component
            playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();

            dialogueAnimator = GetComponent<Animator>();
            
            // Explicitly set text direction to left to right to prevent reversal issues
            if (textComponent != null)
            {
                textComponent.isRightToLeftText = false;
            }
            
            textComponent.text = string.Empty;
            index = 0;
            StartCoroutine(StartDialogueSequence());
        }
    }

    IEnumerator StartDialogueSequence()
    {
        // Disable player movement when dialogue starts
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Trigger entry animation
        if (dialogueAnimator != null)
        {
            dialogueAnimator.SetTrigger("Entry");
            yield return new WaitForSeconds(dialogueAnimator.GetCurrentAnimatorStateInfo(0).length);
        }
        
        StartCoroutine(TypeLine());
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
            
            // Re-enable player movement when dialogue ends
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
            
            // Trigger callback when dialogue completes
            OnDialogueComplete?.Invoke();
        }
    }

    IEnumerator TypeLine()
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
        
        StartCoroutine(StartDialogueSequence());
    }
    
    // Method for quick battle messages without animations (like damage messages)
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
}
