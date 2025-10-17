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

        dialogueAnimator.SetTrigger("Entry");
        yield return new WaitForSeconds(dialogueAnimator.GetCurrentAnimatorStateInfo(0).length);
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
            dialogueAnimator.SetTrigger("Exit");
            // Re-enable player movement when dialogue ends
            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
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
}
