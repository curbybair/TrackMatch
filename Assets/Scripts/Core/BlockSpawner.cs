using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject blockPrefab;
    public GameObject bombPrefab;

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;
    public float spawnX = 0f;
    public float spawnY = 5f;

    [Header("Difficulty")]
    public float bombChance = 0.2f; 

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnBlock();
            timer = 0f;
        }
    }

    void SpawnBlock()
    {
        // Decide whether to spawn a bomb or block
        float roll = Random.Range(0f, 1f);

        if (roll < bombChance)
        {
            Instantiate(bombPrefab, new Vector3(spawnX, spawnY, 0), Quaternion.identity);
        }
        else
        {
            GameObject newBlock = Instantiate(blockPrefab,
                new Vector3(spawnX, spawnY, 0), Quaternion.identity);

            // Assign random color
            SpriteRenderer sr = newBlock.GetComponent<SpriteRenderer>();
            sr.color = GetRandomColor();
        }
    }

    Color GetRandomColor()
    {
        Color[] colors = new Color[]
        {
            new Color(0.93f, 0.23f, 0.23f), // Red
            new Color(0.23f, 0.53f, 0.93f), // Blue
            new Color(0.23f, 0.80f, 0.34f), // Green
            new Color(0.98f, 0.85f, 0.17f), // Yellow
            new Color(0.63f, 0.23f, 0.93f), // Purple
            new Color(0.95f, 0.55f, 0.10f)  // Orange
        };

        return colors[Random.Range(0, colors.Length)];
    }
}