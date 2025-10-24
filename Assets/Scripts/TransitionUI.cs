using UnityEngine;
using UnityEngine.UI;

public class TransitionUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject playerCard;
    public GameObject battleOptions;
    public GameObject magicMenu;

    [Header("Button References")]
    public Button attackButton;
    public Button magicButton;
    public Button itemsButton;
    public Button fleeButton;

    [Header("Animator Reference")]
    public Animator uiAnimator;

    private UIBattleManager uiBattleManager;

    void Start()
    {
        uiBattleManager = FindObjectOfType<UIBattleManager>();
        
        // Hide magic menu initially
        if (magicMenu != null)
        {
            magicMenu.SetActive(false);
        }
    }
    
    void Update()
    {
        // Check for A key or left arrow to go back to main menu
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Only trigger if we're currently in magic menu
            if (magicMenu != null && magicMenu.activeInHierarchy)
            {
                StartMagicToMainTransition();
            }
        }
    }

    public void StartMainToMagicTransition()
    {
        if (uiAnimator != null)
        {
            uiAnimator.SetTrigger("MainToMagic");
            Debug.Log("Triggered MainToMagic animation");
        }
        else
        {
            Debug.LogError("No Animator assigned to TransitionUI!");
        }
    }
    
    public void StartMagicToMainTransition()
    {
        if (uiAnimator != null)
        {
            uiAnimator.SetTrigger("MagicToMain");
            Debug.Log("Triggered MagicToMain animation");
        }
        else
        {
            Debug.LogError("No Animator assigned to TransitionUI!");
        }
    }

    public void LockInputs()
    {
        SetButtonState(false);
        Debug.Log("Inputs locked");
    }

    public void UnlockMagicInputs()
    {
        SetButtonState(true);
        Debug.Log("Magic menu inputs unlocked");
    }
    

    public void ActivateMagicMenu()
    {
        magicMenu.SetActive(true);
    }
    
    public void UnlockMainInputs()
    {
        SetButtonState(true);
        Debug.Log("Main menu inputs unlocked");
    }

    private void SetButtonState(bool interactable)
    {
        attackButton.interactable = interactable;
        magicButton.interactable = interactable;
        itemsButton.interactable = interactable;
        fleeButton.interactable = interactable;
    }
}
