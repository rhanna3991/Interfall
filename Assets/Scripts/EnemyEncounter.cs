using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyEncounter : MonoBehaviour
{
    public float transitionDuration = 2.0f;
    public Animator battleTransitionAnimator;
    public GameObject transitionContainer;

    private PlayerMovement playerMovement;
    private Animator playerAnimator;
    private Rigidbody2D playerRigidbody;


    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerAnimator = player.GetComponent<Animator>();
            playerRigidbody = player.GetComponent<Rigidbody2D>();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if player collided with enemy
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(PlayBattleTransition());
        }
    }

    private IEnumerator PlayBattleTransition()
    {
        // Completely stop player movement
        if (playerRigidbody != null)
        {
            playerRigidbody.velocity = Vector2.zero;
        }
        
        // Disable player movement
        if (playerMovement != null) {
            playerMovement.enabled = false;
        }

        // Freeze player animation on current frame
        if (playerAnimator != null)
        {
            playerAnimator.enabled = false;
        }

        // Enable the transition container UI
        if (transitionContainer != null)
        {
            transitionContainer.SetActive(true);
        }

        // Trigger the battle entry animation
        if (battleTransitionAnimator != null)
        {
            battleTransitionAnimator.SetTrigger("BattleEntry");
        }

        // Wait for the entry animation to finish
        yield return new WaitForSeconds(transitionDuration);
    
        SceneManager.LoadScene("BattleScene");
    }


}

