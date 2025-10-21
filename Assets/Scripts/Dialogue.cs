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

        // Check if animator is available and has the Entry trigger
        if (dialogueAnimator != null && dialogueAnimator.runtimeAnimatorController != null)
        {
            try
            {
                dialogueAnimator.SetTrigger("Entry");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Animator trigger 'Entry' not found or animator not properly configured: " + e.Message);
            }
            
            // Wait for animation to complete (outside try-catch)
            if (dialogueAnimator != null && dialogueAnimator.runtimeAnimatorController != null)
            {
                yield return new WaitForSeconds(dialogueAnimator.GetCurrentAnimatorStateInfo(0).length);
            }
        }
        else
        {
            Debug.LogWarning("Dialogue animator not properly configured. Continuing without animation.");
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
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
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
        StartCoroutine(StartDialogueSequence());
    }
}
