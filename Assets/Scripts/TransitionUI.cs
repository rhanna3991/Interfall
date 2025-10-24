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
    
    [Header("Magic Menu")]
    public MagicMenu magicMenuUI;
    public CharacterStats playerStats;

    private UIBattleManager uiBattleManager;

    void Start()
    {
        uiBattleManager = FindObjectOfType<UIBattleManager>();

        if (magicMenu != null)
            magicMenu.SetActive(false);
    }

    void Update()
    {
        // Check for A key or left arrow to go back to main menu
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            && magicMenu != null && magicMenu.activeInHierarchy)
        {
            StartMagicToMainTransition();
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

    public void ActivateMagicMenu()
    {
        if (magicMenu != null)
            magicMenu.SetActive(true);
    }

    public void DeactivateMagicMenu()
    {
        if (magicMenu != null)
            magicMenu.SetActive(false);
    }

    private void SetButtonState(bool interactable)
    {
        if (attackButton) attackButton.interactable = interactable;
        if (magicButton) magicButton.interactable = interactable;
        if (itemsButton) itemsButton.interactable = interactable;
        if (fleeButton) fleeButton.interactable = interactable;
    }
}
