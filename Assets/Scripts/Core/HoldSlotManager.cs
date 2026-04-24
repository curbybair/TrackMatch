using UnityEngine;
using TMPro;

public class HoldSlotManager : MonoBehaviour
{
    public static HoldSlotManager Instance { get; private set; }

    [Header("Hold Slot")]
    public Transform holdPoint;
    public TextMeshProUGUI heldText;

    private Block heldBlock;
    private bool isDraggingHeldBlock = false;
    private Vector3 dragOffset;

    private Camera mainCam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mainCam = Camera.main;
        UpdateUI();
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isGameRunning)
            return;

        HandleHeldBlockDrag();
    }

    public bool HasHeldBlock()
    {
        return heldBlock != null;
    }

    public bool TryStoreBlock(Block block)
    {
        if (block == null || heldBlock != null)
            return false;

        heldBlock = block;

        heldBlock.transform.SetParent(holdPoint);
        heldBlock.transform.position = holdPoint.position;

        heldBlock.isMoving = false;

        Collider2D col = heldBlock.GetComponent<Collider2D>();
        if (col != null)
            col.enabled = true;

        Rigidbody2D rb = heldBlock.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        UpdateUI();
        return true;
    }

    public bool IsHeldBlock(GameObject obj)
    {
        return heldBlock != null && heldBlock.gameObject == obj;
    }

    private void HandleHeldBlockDrag()
    {
        if (heldBlock == null || mainCam == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorld = GetMouseWorldPosition();
            Collider2D hit = Physics2D.OverlapPoint(mouseWorld);

            if (hit != null && hit.gameObject == heldBlock.gameObject)
            {
                isDraggingHeldBlock = true;
                dragOffset = heldBlock.transform.position - mouseWorld;
            }
        }

        if (isDraggingHeldBlock && Input.GetMouseButton(0))
        {
            Vector3 mouseWorld = GetMouseWorldPosition();
            heldBlock.transform.position = mouseWorld + dragOffset;
        }

        if (isDraggingHeldBlock && Input.GetMouseButtonUp(0))
        {
            isDraggingHeldBlock = false;
            ReleaseHeldBlockToConveyor();
        }
    }

    private void ReleaseHeldBlockToConveyor()
    {
        if (heldBlock == null)
            return;

        heldBlock.transform.SetParent(null);
        heldBlock.isMoving = true;

        Rigidbody2D rb = heldBlock.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        heldBlock = null;
        UpdateUI();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = 0f;
        Vector3 world = mainCam.ScreenToWorldPoint(mouse);
        world.z = 0f;
        return world;
    }

    private void UpdateUI()
    {
        if (heldText != null)
            heldText.text = heldBlock == null ? "Held: Empty" : "Held: Ready";
    }
}