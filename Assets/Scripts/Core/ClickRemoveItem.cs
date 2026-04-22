using UnityEngine;

public class ClickRemoveItem : MonoBehaviour
{
    [Header("Removal")]
    public bool canBeRemoved = true;

    private bool removed = false;

    private void OnMouseDown()
    {
        if (removed || !canBeRemoved)
            return;

        if (GameManager.Instance != null && !GameManager.Instance.isGameRunning)
            return;

        removed = true;
        Destroy(gameObject);
    }
}