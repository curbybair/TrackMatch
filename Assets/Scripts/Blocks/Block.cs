using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("Block Settings")]
    public float speed = 3f;
    public bool isMoving = true;

    void Update()
    {
        if (isMoving)
        {
            // Move block downward
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }

        // Destroy block if it falls too far off screen
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}