using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI highScoreText;

    void Start()
    {
        // Load and display high score
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        if (highScoreText != null)
            highScoreText.text = "Best: " + highScore;
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene("Gameplay");
    }

    public void OnQuitButton()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}