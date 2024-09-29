using UnityEngine;
using UnityEngine.InputSystem;

public class DragObject : MonoBehaviour
{
    [Header("Drag Sort Settings")]
    [SerializeField][ShowOnly] private int originalSortingOrder;
    [SerializeField] private int dragSortingOrder = 100;

    [Header("Drag Opacity Settings")]
    [SerializeField][ShowOnly] private Color originalColor;
    [SerializeField] private float dragOpacity = 0.5f;

    [Header("References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField][ShowOnly] private SpriteRenderer objectRenderer;
    [SerializeField][ShowOnly] private IDraggable draggedObject;
    [SerializeField][ShowOnly] private Collider2D objectCollider;  // The object's collider

    [Header("Settings")]
    [SerializeField][ShowOnly] private bool isDragging = false;

    private Vector3 originalPosition;  // Store original position to teleport back if needed
    private InputHandler inputHandler;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        inputHandler = FindObjectOfType<InputHandler>();  // Get reference to the InputHandler in the scene
    }

    // Called when the dragging starts
    public void StartDragging(GameObject obj)
    {
        draggedObject = obj.GetComponent<IDraggable>();
        objectRenderer = obj.GetComponent<SpriteRenderer>();
        objectCollider = obj.GetComponent<Collider2D>();  // Get the object's collider

        if (draggedObject != null && objectRenderer != null && objectCollider != null)
        {
            CacheOriginalProperties();
            SetDraggingProperties();

            // Disable the collider while dragging
            objectCollider.enabled = false;

            isDragging = true;
            draggedObject.OnPickUp();

            // Store the original position
            originalPosition = (draggedObject as MonoBehaviour).transform.position;
        }
    }

    // Called when the dragging stops
    public void StopDragging()
    {
        if (draggedObject != null && objectRenderer != null && objectCollider != null)
        {
            // Re-enable the collider when the dragging ends
            objectCollider.enabled = true;

            // Check if we are hovering over a valid tile that allows dropping
            if (inputHandler.CurrentHoveredTile != null && inputHandler.CurrentHoveredTile.CanDrop())
            {
                // Drop the object at the hovered tile's position
                Vector3 dropPosition = inputHandler.CurrentHoveredTilePosition;
                (draggedObject as MonoBehaviour).transform.position = new Vector3(dropPosition.x, dropPosition.y, originalPosition.z);
                inputHandler.ResetHoveredTile();
                //Debug.Log("Dropped object at valid tile position.");
            }
            else
            {
                // Teleport the object back to its original position if drop is invalid
                (draggedObject as MonoBehaviour).transform.position = originalPosition;
                //Debug.Log("Invalid drop. Returning object to original position.");
            }

            // Restore original properties
            RestoreOriginalProperties();
            draggedObject.OnDrop();
        }

        ResetDraggingState();
    }

    private void Update()
    {
        if (isDragging && draggedObject != null)
        {
            UpdateDraggedObjectPosition();  // Update the position while dragging
        }
    }

    // Cache the original properties (sorting order and color)
    private void CacheOriginalProperties()
    {
        originalSortingOrder = objectRenderer.sortingOrder;
        originalColor = objectRenderer.color;
    }

    // Set properties for dragging (e.g., opacity and sorting order)
    private void SetDraggingProperties()
    {
        draggedObject.UpdateSortingLayer(dragSortingOrder);  // Set sorting order for dragging
        SetObjectOpacity(dragOpacity);  // Set lower opacity while dragging
    }

    // Restore the object's original properties after dragging
    private void RestoreOriginalProperties()
    {
        draggedObject.UpdateSortingLayer(originalSortingOrder);  // Restore original sorting order
        SetObjectOpacity(originalColor.a);  // Restore original opacity
    }

    // Set object opacity by changing the alpha value
    private void SetObjectOpacity(float alphaValue)
    {
        Color newColor = objectRenderer.color;
        newColor.a = alphaValue;  // Adjust the alpha value (transparency)
        objectRenderer.color = newColor;
    }

    // Update the object's position to follow the mouse while dragging
    private void UpdateDraggedObjectPosition()
    {
        Vector3 mousePos = MouseToWorldPosition();
        (draggedObject as MonoBehaviour).transform.position = new Vector3(mousePos.x, mousePos.y, (draggedObject as MonoBehaviour).transform.position.z);
    }

    // Convert mouse screen position to world position
    private Vector3 MouseToWorldPosition()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        return mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
    }

    // Reset the dragging state after dragging stops
    private void ResetDraggingState()
    {
        isDragging = false;
        draggedObject = null;
        objectRenderer = null;
        objectCollider = null;
    }
}
