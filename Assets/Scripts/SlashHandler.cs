using UnityEngine;

public class SlashEffectHandler : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }


    public void OnSlashFinished()
    {
        animator.Rebind(); // Reset the animation state
        animator.Update(0); // Apply immediately
        gameObject.SetActive(false); // Hide the slash effect
    }
}
