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

        // This is where the held snack is treated as if Donnie got it
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