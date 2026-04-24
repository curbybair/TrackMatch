using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Settings")]
    public float doubleClickTime = 0.3f;

    [Header("Belt Boundaries")]
    public float beltLeft = -0.7f;
    public float beltRight = 0.7f;
    public float beltTop = 4.5f;
    public float beltBottom = -3.5f;

    [Header("Detection")]
    public float clickRadius = 0.5f;

    private GameObject draggedBlock;
    private Vector3 offset;
    private float lastClickTime = 0f;
    private GameObject lastClickedBlock;
    private Camera mainCamera;
    private Vector3 originalPosition;

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
        // Left click - drag blocks
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = GetMouseWorldPosition();
            int blockLayer = LayerMask.GetMask("Blocks");
            Collider2D hit = Physics2D.OverlapCircle(mousePos, clickRadius, blockLayer);

            if (hit != null && (hit.CompareTag("Block") || hit.CompareTag("Bomb")))
            {
                if (HoldSlotManager.Instance.IsHeldBlock(hit.gameObject))
                    return;

                if (hit.gameObject == lastClickedBlock &&
                    Time.time - lastClickTime < doubleClickTime)
                {
                    DestroyBlock(hit.gameObject);
                    return;
                }

                lastClickTime = Time.time;
                lastClickedBlock = hit.gameObject;

                if (hit.CompareTag("Block"))
                {
                    draggedBlock = hit.gameObject;
                    originalPosition = draggedBlock.transform.position;
                    draggedBlock.GetComponent<Block>().isMoving = false;
                    offset = draggedBlock.transform.position - mousePos;
                }
            }
        }

        // Mouse held - move block within belt boundaries
        if (Input.GetMouseButton(0) && draggedBlock != null)
        {
            Vector3 newPos = GetMouseWorldPosition() + offset;
            newPos.x = Mathf.Clamp(newPos.x, beltLeft, beltRight);
            newPos.y = Mathf.Clamp(newPos.y, beltBottom, beltTop);
            draggedBlock.transform.position = newPos;
        }

        // Mouse released - check for overlaps
        if (Input.GetMouseButtonUp(0) && draggedBlock != null)
        {
            if (IsOverlapping())
            {
                draggedBlock.transform.position = originalPosition;
                Debug.Log("Can't place there - overlapping!");
            }

            draggedBlock.GetComponent<Block>().isMoving = true;
            draggedBlock = null;
            lastClickedBlock = null;
        }

        // Right click - store block in hold slot
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = GetMouseWorldPosition();
            int blockLayer = LayerMask.GetMask("Blocks");
            Collider2D hit = Physics2D.OverlapCircle(mousePos, clickRadius, blockLayer);

            Debug.Log("Right clicked: " + (hit != null ? hit.gameObject.name + " tag:" + hit.tag : "nothing"));

            if (hit != null && hit.CompareTag("Block"))
            {
                if (HoldSlotManager.Instance.IsHeldBlock(hit.gameObject))
                    return;

                Block block = hit.GetComponent<Block>();
                bool stored = HoldSlotManager.Instance.TryStoreBlock(block);
                Debug.Log("Stored result: " + stored);
            }
        }
    }

    bool IsOverlapping()
    {
        Collider2D col = draggedBlock.GetComponent<Collider2D>();
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            draggedBlock.transform.position,
            col.bounds.size * 0.9f,
            0f
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != draggedBlock &&
                !HoldSlotManager.Instance.IsHeldBlock(hit.gameObject) &&
                (hit.CompareTag("Block") || hit.CompareTag("Bomb")))
            {
                return true;
            }
        }
        return false;
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