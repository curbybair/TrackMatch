using UnityEngine;
using UnityEngine.SceneManagement;

public class ComboTracker : MonoBehaviour
{
    public static ComboTracker Instance { get; private set; }

    [Header("Combo Settings")]
    public float comboWindowSeconds = 3f;
    public int currentStreak = 0;
    public int pointsPerMatch = 100;

    private Color lastColor = Color.white;
    private float timer = 0f;
    private bool hasCollectedFirst = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Update()
    {
        // Count down combo timer
        if (hasCollectedFirst && timer > 0f)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                // Combo expired
                currentStreak = 0;
                Debug.Log("Combo broken! Timer ran out.");
            }
        }
    }

    public void OnBlockCollected(Color collectedColor)
    {
        if (!hasCollectedFirst)
        {
            // First block ever collected
            hasCollectedFirst = true;
            currentStreak = 1;
            lastColor = collectedColor;
            timer = comboWindowSeconds;
            Debug.Log("First block collected!");
            return;
        }

        if (ColorsMatch(collectedColor, lastColor))
        {
            // Matching color - extend combo!
            currentStreak++;
            timer = comboWindowSeconds;
            int points = currentStreak * pointsPerMatch;
            ScoreManager.Instance.AddScore(points);
            Debug.Log("Combo! Streak: " + currentStreak + " Points: " + points);
        }
        else
        {
            // Different color - reset streak
            currentStreak = 1;
            timer = comboWindowSeconds;
            ScoreManager.Instance.AddScore(pointsPerMatch);
            Debug.Log("No match. Streak reset.");
        }

        lastColor = collectedColor;
    }

    bool ColorsMatch(Color a, Color b)
    {
        // Compare colors with small tolerance for floating point
        float tolerance = 0.1f;
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}