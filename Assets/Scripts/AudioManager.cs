using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Source")]
    public AudioSource sfxSource;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;
    
    [Header("Button Sounds")]
    public AudioClip buttonClickSound;
    [Range(0f, 1f)]
    public float buttonClickVolume = 1f;
    public AudioClip buttonHoverSound;
    [Range(0f, 1f)]
    public float buttonHoverVolume = 1f;
    
    [Header("Attack Sounds")]
    public AudioClip slashAttackSound;
    [Range(0f, 1f)]
    public float slashAttackVolume = 1f;
    public AudioClip enemyCrySound;
    [Range(0f, 1f)]
    public float enemyCryVolume = 1f;
    public AudioClip enemyDeathSound;
    [Range(0f, 1f)]
    public float enemyDeathVolume = 1f;
    public AudioClip playerHitSound;
    [Range(0f, 1f)]
    public float playerHitVolume = 1f;
    
    [Header("Game Over Sounds")]
    public AudioClip gameOverSound;
    [Range(0f, 1f)]
    public float gameOverVolume = 1f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Set initial volume
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
    
    // Sound effects methods
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void PlaySFX(AudioClip clip, float volume)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }
    
    // Button sound methods
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound, buttonClickVolume);
    }
    
    public void PlayButtonHover()
    {
        PlaySFX(buttonHoverSound, buttonHoverVolume);
    }
    
    // Attack sound methods
    public void PlaySlashAttack()
    {
        PlaySFX(slashAttackSound, slashAttackVolume);
    }
    
    public void PlayEnemyCry()
    {
        PlaySFX(enemyCrySound, enemyCryVolume);
    }
    
    public void PlayEnemyDeath()
    {
        PlaySFX(enemyDeathSound, enemyDeathVolume);
    }
    
    public void PlayPlayerHit()
    {
        PlaySFX(playerHitSound, playerHitVolume);
    }
    
    // Game over sound methods
    public void PlayGameOver()
    {
        PlaySFX(gameOverSound, gameOverVolume);
    }
    
    // Volume control methods
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
    
    // Utility methods
    public bool IsSFXPlaying()
    {
        return sfxSource != null && sfxSource.isPlaying;
    }
}
