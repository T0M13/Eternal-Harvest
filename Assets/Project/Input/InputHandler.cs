using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class InputHandler : MonoBehaviour
{
    [Header("Mouse Settings")]
    [SerializeField] private Vector2 mousePosition;

    [Header("Tile Settings")]
    [SerializeField] private CustomTile currentHoveredTile;
    [SerializeField] private Vector3 currentHoveredTilePosition;
    [SerializeField] private LayerMask tilemapLayerMask;
    [SerializeField] private LayerMask grabbableLayerMask;

    [Header("Gizmos Settings")]
    [SerializeField] private Color rayColor = Color.red;
    [SerializeField] private Color hitColor = Color.green;
    [SerializeField] private float hitSphereRadius = 0.1f;

    [Header("References")]
    [SerializeField] private GameObject tileHighlighter;  // The GameObject that will follow the mouse
    [SerializeField] private SpriteRenderer highlighterRenderer;  // SpriteRenderer of the tile highlighter
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color validColor = Color.green;
    [SerializeField] private Color invalidColor = Color.red;

    [SerializeField] private Camera mainCamera;
    private Vector3 worldPosition;
    private RaycastHit2D hit;

    public Vector2 MousePosition { get => mousePosition; set => mousePosition = value; }
    public CustomTile CurrentHoveredTile { get => currentHoveredTile; set => currentHoveredTile = value; }
    public Vector3 CurrentHoveredTilePosition { get => currentHoveredTilePosition; set => currentHoveredTilePosition = value; }

    [HideInInspector] public UnityEvent<GameObject> OnPickUpObject;
    [HideInInspector] public UnityEvent OnDropObject;

    private bool isDragging = false;  // Flag to indicate if we're dragging

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Main method for handling mouse movement
    public void OnMouseMove(InputAction.CallbackContext value)
    {
        UpdateMousePosition(value);
        DetectTileUnderMouse();
        UpdateTileHighlighter();
    }

    // Update mouse position based on input
    private void UpdateMousePosition(InputAction.CallbackContext value)
    {
        MousePosition = value.ReadValue<Vector2>();
        worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(MousePosition.x, MousePosition.y, 0));
    }

    // Detect if the mouse is over a tile and update the current hovered tile
    private void DetectTileUnderMouse()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPosition, Vector2.zero, tilemapLayerMask);

        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null && hit.collider is TilemapCollider2D)
                {
                    Tilemap tilemap = hit.collider.GetComponent<Tilemap>();

                    if (tilemap != null)
                    {
                        Vector3Int gridPosition = tilemap.WorldToCell(worldPosition);
                        CustomTile tile = tilemap.GetTile<CustomTile>(gridPosition);

                        if (tile != null)
                        {
                            UpdateHoveredTile(tile, tilemap, gridPosition);
                            return; // Exit loop after the first valid tile hit
                        }
                    }
                }
            }
        }

        ResetHoveredTile(); // If no tile is hit or valid, reset
    }


    // Updates the currently hovered tile and the highlighter position
    private void UpdateHoveredTile(CustomTile tile, Tilemap tilemap, Vector3Int gridPosition)
    {
        CurrentHoveredTile = tile;
        currentHoveredTilePosition = tilemap.GetCellCenterWorld(gridPosition);
        tileHighlighter.transform.position = currentHoveredTilePosition;

        //Debug.Log($"Hovering over tile at {gridPosition} (World Position: {currentHoveredTilePosition}): CanDrop = {tile.CanDrop()}");
    }

    // Resets the hovered tile when no valid tile is found
    public void ResetHoveredTile()
    {
        CurrentHoveredTile = null;
        tileHighlighter.SetActive(false);
    }

    // Updates the tile highlighter based on whether the tile can accept a drop (only when dragging)
    private void UpdateTileHighlighter()
    {
        if (tileHighlighter != null)
        {
            if (CurrentHoveredTile != null)
            {
                // Only check CanDrop if we are dragging
                if (isDragging)
                {
                    highlighterRenderer.color = CurrentHoveredTile.CanDrop() ? validColor : invalidColor;
                }
                else
                {
                    highlighterRenderer.color = defaultColor;
                }

                tileHighlighter.SetActive(true);
                return;
            }

            // No hovered tile, reset the color
            if (highlighterRenderer.color != defaultColor)
                highlighterRenderer.color = defaultColor;

            tileHighlighter.SetActive(false);
        }
    }

    // Called when mouse clicks
    public void OnMouseClick(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            TryPickUpObject();
        }
        else if (value.canceled)
        {
            OnDropObject?.Invoke();
            StopDragging();
        }
    }

    // Tries to pick up a draggable object on mouse click
    private void TryPickUpObject()
    {
        hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity, grabbableLayerMask);

        if (hit.collider != null && hit.collider.gameObject.GetComponent<IDraggable>() != null)
        {
            OnPickUpObject?.Invoke(hit.collider.gameObject);
            StartDragging();
        }
    }

    // Starts dragging, enabling drop checks
    private void StartDragging()
    {
        isDragging = true;
    }

    // Stops dragging, disabling drop checks
    private void StopDragging()
    {
        isDragging = false;
    }

    // Check if mouse is in viewport
    public bool IsMouseInViewport()
    {
        Vector3 mouseViewportPos = mainCamera.ScreenToViewportPoint(mousePosition);
        return mouseViewportPos.x >= 0 && mouseViewportPos.x <= 1 && mouseViewportPos.y >= 0 && mouseViewportPos.y <= 1;
    }

    // Draw Gizmos for debugging purposes
    private void OnDrawGizmos()
    {
        if (worldPosition != Vector3.zero)
        {
            Gizmos.color = rayColor;
            Gizmos.DrawSphere(worldPosition, hitSphereRadius);

            if (hit.collider != null)
            {
                Gizmos.color = hitColor;
                Gizmos.DrawSphere(hit.point, hitSphereRadius);
            }
        }
    }
}
