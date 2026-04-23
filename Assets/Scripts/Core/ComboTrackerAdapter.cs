using UnityEngine;

public class ComboTrackerAdapter : MonoBehaviour
{
    public static ComboTrackerAdapter Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterHeldColor(string colorId)
    {
        // Convert colorId string to Color and pass to ComboTracker
        Color color = ColorIdToColor(colorId);
        ComboTracker.Instance.OnBlockCollected(color);
    }

    Color ColorIdToColor(string colorId)
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