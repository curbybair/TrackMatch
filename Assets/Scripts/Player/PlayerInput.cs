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
                    if (HoldSlotManager.Instance != null && HoldSlotManager.Instance.IsHeldBlock(hit.gameObject))
                    {
                        return;
                    }

                    draggedBlock = hit.gameObject;
                    draggedBlock.GetComponent<Block>().isMoving = false;
                    offset = draggedBlock.transform.position - mousePos;
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = GetMouseWorldPosition();
            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            Debug.Log("Right clicked: " + (hit != null ? hit.gameObject.name + " tag:" + hit.tag : "nothing"));

            if (hit != null && hit.CompareTag("Block"))
            {
                Block block = hit.GetComponent<Block>();

                if (block == null)
                    return;

                bool stored = HoldSlotManager.Instance.TryStoreBlock(block);

                if (stored)
                {
                    Debug.Log("Block stored in hold slot!");
                }
                else
                {
                    Debug.Log("Hold slot already full!");
                }
            }
        }

        if (Input.GetMouseButton(0) && draggedBlock != null)
        {
            draggedBlock.transform.position = GetMouseWorldPosition() + offset;
        }

        if (Input.GetMouseButtonUp(0) && draggedBlock != null)
        {
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