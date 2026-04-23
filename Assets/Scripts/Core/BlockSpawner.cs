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
        float roll = Random.Range(0f, 1f);

        if (roll < bombChance)
        {
            Instantiate(bombPrefab, new Vector3(spawnX, spawnY, 0), Quaternion.identity);
        }
        else
        {
            GameObject newBlock = Instantiate(blockPrefab,
                new Vector3(spawnX, spawnY, 0), Quaternion.identity);

            // Pick a random color index
            string[] colorIds = { "Red", "Blue", "Green", "Yellow", "Purple", "Orange" };
            int colorIndex = Random.Range(0, colorIds.Length);

            // Assign color visually
            SpriteRenderer sr = newBlock.GetComponent<SpriteRenderer>();
            sr.color = GetColorFromId(colorIds[colorIndex]);

            // Assign colorId string to block
            newBlock.GetComponent<Block>().colorId = colorIds[colorIndex];
        }
    }

    Color GetColorFromId(string colorId)
    {
        switch (colorId)
        {
            case "Red": return new Color(0.93f, 0.23f, 0.23f);
            case "Blue": return new Color(0.23f, 0.53f, 0.93f);
            case "Green": return new Color(0.23f, 0.80f, 0.34f);
            case "Yellow": return new Color(0.98f, 0.85f, 0.17f);
            case "Purple": return new Color(0.63f, 0.23f, 0.93f);
            case "Orange": return new Color(0.95f, 0.55f, 0.10f);
            default: return Color.white;
        }
    }
}