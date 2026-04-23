using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("Block Settings")]
    public float speed = 3f;
    public bool isMoving = true;

    [Header("Color")]
    public string colorId;

    void Update()
    {
        if (isMoving)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}