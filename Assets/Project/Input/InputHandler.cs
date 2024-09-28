using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    [Header("Mouse Settings")]
    [SerializeField] private Vector2 mousePosition;

    [Header("Gizmos Settings")]
    [SerializeField] private Color rayColor = Color.red;
    [SerializeField] private Color hitColor = Color.green;
    [SerializeField] private float hitSphereRadius = 0.1f;

    private Camera mainCamera;
    private Vector3 worldPosition;
    private RaycastHit2D hit;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void OnMouseMove(InputAction.CallbackContext value)
    {
        mousePosition = value.ReadValue<Vector2>();
        worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));

        hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            TileComponent tileComponent = hit.collider.GetComponent<TileComponent>();
            if (tileComponent != null)
                OnTileHover(tileComponent);
        }
    }

    public void OnMouseClick(InputAction.CallbackContext value)
    {
        hit = Physics2D.Raycast(worldPosition, Vector2.zero);

        if (hit.collider != null)
        {
            TileComponent tileComponent = hit.collider.GetComponent<TileComponent>();
            if (tileComponent != null)
                OnTileClick(tileComponent);
        }
    }

    private void OnTileHover(TileComponent tileComponent)
    {
        // Perform hover action (logic handled here)
    }

    private void OnTileClick(TileComponent tileComponent)
    {
        // Perform click action (logic handled here)
    }

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
