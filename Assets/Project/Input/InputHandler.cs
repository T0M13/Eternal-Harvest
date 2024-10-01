using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class InputHandler : MonoBehaviour
{
    [Header("Mouse Settings")]
    private Vector2 mousePosition; // Made public to fix missing accessors

    [Header("Tile Settings")]
    [SerializeField] private Vector3 currentHoveredTilePosition;
    [SerializeField] private Vector3Int currentHoveredTileGridPosition;

    [Header("References")]
    [SerializeField] private GameObject tileHighlighter;
    [SerializeField] private SpriteRenderer highlighterRenderer;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color validColor = Color.green;
    [SerializeField] private Color invalidColor = Color.red;

    private Camera mainCamera;
    private MapGenerator mapGenerator; // Reference to MapGenerator
    private bool isDragging = false;

    [HideInInspector] public UnityEvent<GameObject> OnPickUpObject = new UnityEvent<GameObject>(); // Added initialization
    [HideInInspector] public UnityEvent OnDropObject = new UnityEvent(); // Added initialization
    public Vector2 MousePosition { get => mousePosition; set => mousePosition = value; }

    private void Awake()
    {
        mainCamera = Camera.main;
        mapGenerator = FindObjectOfType<MapGenerator>(); // Fetch the MapGenerator
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
    }

    // Detect if the mouse is over a tile and update the current hovered tile
    private void DetectTileUnderMouse()
    {
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(MousePosition.x, MousePosition.y, 0));
        Vector3Int gridPosition = mapGenerator.placeholderTilemap.WorldToCell(worldPosition);

        // Check if the tile has properties, and if so, update the highlighter position
        TileProperty tileProperty = mapGenerator.GetTileProperty(gridPosition);

        if (tileProperty != null)
        {
            currentHoveredTilePosition = mapGenerator.placeholderTilemap.GetCellCenterWorld(gridPosition);
            currentHoveredTileGridPosition = gridPosition; // Store the grid position
            tileHighlighter.transform.position = currentHoveredTilePosition;
        }
        else
        {
            ResetHoveredTile();
        }
    }

    // Resets the hovered tile when no valid tile is found
    public void ResetHoveredTile()
    {
        tileHighlighter.SetActive(false);
    }

    // Updates the tile highlighter based on whether the tile can accept a drop (only when dragging)
    private void UpdateTileHighlighter()
    {
        if (tileHighlighter != null)
        {
            if (mapGenerator.GetTileProperty(currentHoveredTileGridPosition) != null)
            {
                // Only check CanDrop if we are dragging
                if (isDragging)
                {
                    Vector3Int gridPosition = currentHoveredTileGridPosition;
                    TileProperty tileProperty = mapGenerator.GetTileProperty(gridPosition);

                    if (tileProperty != null)
                    {
                        // Set the highlighter color based on whether the tile is walkable
                        if (tileProperty.GetProperty(TilePropertyType.Walkable))
                        {
                            highlighterRenderer.color = validColor;
                        }
                        else if (tileProperty.GetProperty(TilePropertyType.Blocked))
                        {
                            highlighterRenderer.color = invalidColor;
                        }
                    }
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
            {
                highlighterRenderer.color = defaultColor;
            }

            tileHighlighter.SetActive(false);
        }
    }

    // Returns the current hovered tile's grid position
    public Vector3Int GetCurrentHoveredTilePosition()
    {
        return currentHoveredTileGridPosition;
    }

    // Returns the current hovered tile's world position
    public Vector3 GetCurrentHoveredTileWorldPosition()
    {
        return currentHoveredTilePosition;
    }

    // Called when mouse clicks
    public void OnMouseClick(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            TryPickUpObject(); // Call this to try picking up an object on click
        }
        else if (value.canceled)
        {
            StopDragging(); // Call StopDragging when the mouse button is released
        }
    }

    // This method tries to pick up the first draggable object the mouse ray hits
    private void TryPickUpObject()
    {
        // Convert mouse screen position to world position
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(MousePosition.x, MousePosition.y, 0));

        // Raycast to detect objects in the scene with a collider
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Vector2.zero, Mathf.Infinity);

        if (hit.collider != null)
        {
            GameObject clickedObject = hit.collider.gameObject;

            // Check if the object has the IDraggable interface
            IDraggable draggable = clickedObject.GetComponent<IDraggable>();
            if (draggable != null)
            {
                // Trigger event for starting the dragging action and pass the clicked object
                OnPickUpObject.Invoke(clickedObject);
                StartDragging();
            }
        }
    }

    private void StartDragging()
    {
        isDragging = true;
        // Optionally: Update highlighter to reflect dragging if necessary
    }



    private void StopDragging()
    {
        isDragging = false;

        // Trigger event for stopping the dragging action (drop)
        OnDropObject.Invoke();
    }

    public bool CanDropOnTile(Vector3Int tilePosition)
    {
        TileProperty tileProperty = mapGenerator.GetTileProperty(tilePosition);
        if (tileProperty != null && tileProperty.GetProperty(TilePropertyType.Walkable))
        {
            return true; // Tile allows dropping
        }
        return false;  // Tile does not allow dropping
    }

    public bool IsMouseInViewport()
    {
        Vector3 mouseViewportPos = mainCamera.ScreenToViewportPoint(mousePosition);
        return mouseViewportPos.x >= 0 && mouseViewportPos.x <= 1 && mouseViewportPos.y >= 0 && mouseViewportPos.y <= 1;
    }

}
