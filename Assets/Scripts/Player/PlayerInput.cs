using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Settings")]
    public float doubleClickTime = 0.3f;

    private GameObject draggedBlock;
    private Vector3 offset;
    private float lastClickTime = 0f;
    private GameObject lastClickedBlock;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isGameRunning)
            return;

        HandleDrag();
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = GetMouseWorldPosition();
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            if (hit != null && (hit.CompareTag("Block") || hit.CompareTag("Bomb")))
            {
                // Check for double click
                if (hit.gameObject == lastClickedBlock &&
                    Time.time - lastClickTime < doubleClickTime)
                {
                    // Double clicked - destroy block or bomb
                    DestroyBlock(hit.gameObject);
                    return;
                }

                // Single click - start dragging (only blocks, not bombs)
                lastClickTime = Time.time;
                lastClickedBlock = hit.gameObject;

                if (hit.CompareTag("Block"))
                {
                    draggedBlock = hit.gameObject;
                    draggedBlock.GetComponent<Block>().isMoving = false;
                    offset = draggedBlock.transform.position - mousePos;
                }
            }
        }
        // Right click to hold a block
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = GetMouseWorldPosition();
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            // Debug what we're hitting
            Debug.Log("Right clicked: " + (hit != null ? hit.gameObject.name + " tag:" + hit.tag : "nothing"));

            if (hit != null && hit.CompareTag("Block"))
            {
                Block block = hit.GetComponent<Block>();
                SpriteRenderer sr = hit.GetComponent<SpriteRenderer>();

                // Debug the block data
                Debug.Log("ColorId: " + block.colorId + " Sprite: " + sr.sprite);

                bool stored = HoldSlotManager.Instance.TryStoreSnack(
                    block.colorId,
                    100,
                    sr.sprite,
                    sr.color
                );

                if (stored)
                {
                    Destroy(hit.gameObject);
                    Debug.Log("Block stored in hold slot!");
                }
                else
                {
                    Debug.Log("Hold slot already full!");
                }
            }
        }

        // Mouse held - move block with cursor
        if (Input.GetMouseButton(0) && draggedBlock != null)
        {
            draggedBlock.transform.position = GetMouseWorldPosition() + offset;
        }

        // Mouse released - drop block
        if (Input.GetMouseButtonUp(0) && draggedBlock != null)
        {
            // Resume block movement
            draggedBlock.GetComponent<Block>().isMoving = true;
            draggedBlock = null;
        }
    }

    void DestroyBlock(GameObject block)
    {
        draggedBlock = null;
        lastClickedBlock = null;
        Destroy(block);
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        return mainCamera.ScreenToWorldPoint(mousePos);
    }
}