using UnityEngine;

public class CollectionZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            // Get the color of the collected block
            SpriteRenderer sr = other.GetComponent<SpriteRenderer>();
            Color collectedColor = sr.color;

            // Tell the combo tracker a block was collected
            ComboTracker.Instance.OnBlockCollected(collectedColor);

            // Destroy the block
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Bomb"))
        {
            // Bomb reached the end - game over!
            GameManager.Instance.EndGame();
            Destroy(other.gameObject);
        }
    }
}