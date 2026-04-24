using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sound Effects")]
    public AudioClip collectSound;
    public AudioClip comboSound;
    public AudioClip bombSound;
    public AudioClip gameOverSound;

    private AudioSource audioSource;

    [Header("Music")]
    public AudioClip gameMusic;
    public float musicVolume = 0.5f;

    private AudioSource musicSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        audioSource = GetComponent<AudioSource>();

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.clip = gameMusic;
    }

    public void PlayCollect()
    {
        audioSource.PlayOneShot(collectSound);
    }

    public void PlayCombo()
    {
        audioSource.PlayOneShot(comboSound);
    }

    public void PlayBomb()
    {
        audioSource.PlayOneShot(bombSound);
    }

    public void PlayGameOver()
    {
        audioSource.PlayOneShot(gameOverSound);
    }

    public void PlayMusic()
    {
        if (gameMusic != null)
            musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}