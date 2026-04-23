using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverScreen : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI highScoreText;
    public GameObject gameOverPanel;

    void Start()
    {
        // Hide panel at start
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);

        // Update score displays
        if (ScoreManager.Instance != null)
        {
            finalScoreText.text = "Score: " + ScoreManager.Instance.currentScore;
            highScoreText.text = "Best: " + ScoreManager.Instance.highScore;
        }
    }

    public void OnRetryButton()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}