using UnityEngine;
using TMPro;

public class HoldSlotManager : MonoBehaviour
{
    public static HoldSlotManager Instance { get; private set; }

    [Header("Hold Slot")]
    public Transform holdPoint;
    public TextMeshProUGUI heldText;

    [Header("Belt Boundaries")]
    public float beltLeft = -0.7f;
    public float beltRight = 0.7f;
    public float beltTop = 4.5f;
    public float beltBottom = -3.5f;

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

        Vector3 mouseWorld = GetMouseWorldPosition();
        int blockLayer = LayerMask.GetMask("Blocks");

        // MOUSE DOWN
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D hit = Physics2D.OverlapPoint(mouseWorld, blockLayer);

            //licking anything else = ignore held block system
            if (hit != null && hit.gameObject != heldBlock.gameObject)
            {
                isDraggingHeldBlock = false;
                return;
            }

            //  Only start dragging held block
            if (hit != null && hit.gameObject == heldBlock.gameObject)
            {
                isDraggingHeldBlock = true;
                dragOffset = heldBlock.transform.position - mouseWorld;
            }
        }

        //  DRAGGING (with clamp)
        if (isDraggingHeldBlock && Input.GetMouseButton(0))
        {
            Vector3 newPos = mouseWorld + dragOffset;

            newPos.x = Mathf.Clamp(newPos.x, beltLeft, beltRight);
            newPos.y = Mathf.Clamp(newPos.y, beltBottom, beltTop);

            heldBlock.transform.position = newPos;
        }

        // RELEASE
        if (isDraggingHeldBlock && Input.GetMouseButtonUp(0))
        {
            isDraggingHeldBlock = false;

            if (IsOverlapping())
            {
                // Invalid placement → snap back to hold slot
                heldBlock.transform.position = holdPoint.position;
                heldBlock.transform.SetParent(holdPoint);

                Debug.Log("Invalid placement - snapped back to hold slot");
            }
            else
            {
                // Valid placement → release to conveyor
                ReleaseHeldBlockToConveyor();
            }
        }
    }

    private bool IsOverlapping()
    {
        Collider2D col = heldBlock.GetComponent<Collider2D>();

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            heldBlock.transform.position,
            col.bounds.size * 0.5f,
            0f
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.gameObject != heldBlock.gameObject &&
                (hit.CompareTag("Block") || hit.CompareTag("Bomb")))
            {
                return true;
            }
        }

        return false;
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