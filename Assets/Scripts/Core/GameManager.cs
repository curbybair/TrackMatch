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

    private BlockSpawner spawner;

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
        spawner = FindObjectOfType<BlockSpawner>();
        StartGame();
    }

    public void StartGame()
    {
        isGameRunning = true;
        ScoreManager.Instance.ResetScore();

        // Set spawner speed
        if (spawner != null)
            spawner.spawnInterval = spawnInterval;

        Debug.Log("Game Started!");
    }

    public void EndGame()
    {
        isGameRunning = false;
        Debug.Log("Game Over! Score: " + ScoreManager.Instance.currentScore);

        // Wait 2 seconds then load game over screen
        Invoke("LoadGameOver", 2f);
    }

    void LoadGameOver()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Gameplay");
    }
}