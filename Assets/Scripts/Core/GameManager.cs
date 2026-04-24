using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public bool isGameRunning = false;

    [Header("Difficulty")]
    public float beltSpeed = 3f;
    public float spawnInterval = 2f;

    [Header("Difficulty Scaling")]
    public float speedIncreaseRate = 0.1f;
    public float maxBlockSpeed = 5f;
    public float difficultyInterval = 15f;

    private BlockSpawner spawner;
    private float difficultyTimer = 0f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;
        spawner = FindObjectOfType<BlockSpawner>();
        Invoke("StartGame", 1f);
    }

    void Update()
    {
        if (!isGameRunning) return;

        difficultyTimer += Time.deltaTime;

        if (difficultyTimer >= difficultyInterval)
        {
            IncreaseDifficulty();
            difficultyTimer = 0f;
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        isGameRunning = true;

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();
        else
            Debug.LogWarning("ScoreManager not found!");

        if (spawner != null)
            spawner.spawnInterval = spawnInterval;

        AudioManager.Instance.PlayMusic();

        Debug.Log("Game Started!");
    }

    void IncreaseDifficulty()
    {
        Block[] activeBlocks = FindObjectsByType<Block>(FindObjectsSortMode.None);
        foreach (Block block in activeBlocks)
        {
            block.speed = Mathf.Min(block.speed + speedIncreaseRate, maxBlockSpeed);
        }

        if (spawner != null)
            spawner.spawnInterval = Mathf.Max(spawner.spawnInterval - 0.1f, 0.5f);

        Debug.Log("Difficulty increased!");
    }

    public void EndGame()
    {
        isGameRunning = false;
        Time.timeScale = 0f;

        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayGameOver();

        Debug.Log("Game Over! Score: " + ScoreManager.Instance.currentScore);
        FindObjectOfType<GameOverScreen>().ShowGameOver();
    }

    void LoadGameOver()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameplay");
    }
}