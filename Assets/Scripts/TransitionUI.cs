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
        // Only allow if magic menu is active AND input is enabled in magic menu
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            && magicMenu != null && magicMenu.activeInHierarchy
            && magicMenuUI != null)
        {
            // Only transition if input is enabled (not during attack)
            StartMagicToMainTransition();
        }
    }

    public void StartMainToMagicTransition()
    {
        if (uiAnimator == null) return;
        if (magicMenu != null && magicMenu.activeSelf) return; // already in magic

        uiAnimator.ResetTrigger("MagicToMain");
        uiAnimator.ResetTrigger("DefaultMenu");
        uiAnimator.SetTrigger("MainToMagic");
    }

    public void StartMagicToMainTransition()
    {
        if (uiAnimator == null) return;

        uiAnimator.ResetTrigger("MainToMagic");
        uiAnimator.ResetTrigger("DefaultMenu");
        uiAnimator.SetTrigger("MagicToMain");
    }
    
    public void StartDefaultMenuTransition()
    {
        if (uiAnimator == null) return;

        uiAnimator.ResetTrigger("MainToMagic");
        uiAnimator.ResetTrigger("MagicToMain");
        uiAnimator.SetTrigger("DefaultMenu");

        // Force the magic menu to close if it's still open
        if (magicMenu != null && magicMenu.activeSelf)
            magicMenu.SetActive(false);
    }

    public void ActivateMagicMenu()
    {
        if (magicMenu != null)
        {
            magicMenu.SetActive(true);
            
            // Reset the menu state when activating
            if (magicMenuUI != null)
            {
                magicMenuUI.ResetMenu();
            }
        }
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
