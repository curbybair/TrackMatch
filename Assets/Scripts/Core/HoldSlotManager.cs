using UnityEngine;
using TMPro;

public class HoldSlotManager : MonoBehaviour
{
    public static HoldSlotManager Instance { get; private set; }

    [System.Serializable]
    public class HeldSnackData
    {
        public string colorId;
        public int scoreValue;
        public Sprite sprite;
        public Color color;
    }

    [Header("Held Snack UI")]
    public SpriteRenderer heldPreviewRenderer;
    public TextMeshProUGUI heldText;

    [Header("Use Key")]
    public KeyCode useHeldKey = KeyCode.H;

    private HeldSnackData heldSnack;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateUI();
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isGameRunning)
            return;

        if (Input.GetKeyDown(useHeldKey))
        {
            UseHeldSnack();
        }
    }

    public bool HasHeldSnack()
    {
        return heldSnack != null;
    }

    public bool TryStoreSnack(string colorId, int scoreValue, Sprite sprite, Color color)
    {
        if (heldSnack != null)
            return false;

        heldSnack = new HeldSnackData
        {
            colorId = colorId,
            scoreValue = scoreValue,
            sprite = sprite,
            color = color
        };

        UpdateUI();
        return true;
    }

    public void UseHeldSnack()
    {
        if (heldSnack == null)
            return;

        // Spawn the held block back onto the track at the top
        BlockSpawner spawner = FindObjectOfType<BlockSpawner>();
        if (spawner != null)
        {
            // Get the block prefab and spawn it
            GameObject newBlock = Instantiate(
                spawner.blockPrefab,
                new Vector3(spawner.spawnX, spawner.spawnY, 0),
                Quaternion.identity
            );

            // Apply the stored color and colorId
            SpriteRenderer sr = newBlock.GetComponent<SpriteRenderer>();
            sr.color = heldSnack.color;
            newBlock.GetComponent<Block>().colorId = heldSnack.colorId;
        }

        // Add score and register combo
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(heldSnack.scoreValue);

        if (ComboTrackerAdapter.Instance != null)
            ComboTrackerAdapter.Instance.RegisterHeldColor(heldSnack.colorId);

        heldSnack = null;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (heldPreviewRenderer != null)
        {
            if (heldSnack != null)
            {
                heldPreviewRenderer.enabled = true;
                heldPreviewRenderer.sprite = heldSnack.sprite;
                heldPreviewRenderer.color = heldSnack.color;
            }
            else
            {
                heldPreviewRenderer.enabled = false;
            }
        }

        if (heldText != null)
        {
            heldText.text = heldSnack == null ? "Held: Empty" : "Held: Ready";
        }
    }
}