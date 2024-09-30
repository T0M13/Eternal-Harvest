using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputHandler inputHandler;

    [Header("Camera Movement Settings")]
    [SerializeField] private bool useMouseToMove = true;
    [SerializeField] private bool useKeyboardToMove = true;
    [SerializeField] private bool allowPanningWithMiddleMouse = true;
    [SerializeField] private float cameraSpeed = 2f;
    [SerializeField] private float borderThickness = 10f;

    [Header("Limit Settings")]
    [SerializeField] private Vector2 cameraLimitsX = new Vector2(-50f, 50f);
    [SerializeField] private Vector2 cameraLimitsY = new Vector2(-50f, 50f);
    [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0, -10f);

    [Header("Pan Settings")]
    [SerializeField] private float cameraPanSpeed = 5f;
    [SerializeField][ShowOnly] private Vector3 lastMousePosition;
    [SerializeField][ShowOnly] private bool isPanning = false;

    [Header("Camera Scroll Settings")]
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float zoomLerpSpeed = 10f;
    [SerializeField] private float zoomStep = 0.5f;
    [SerializeField][ShowOnly] private float targetZoom;
    private Coroutine zoomCoroutine;

    [Header("Gizmos Settings")]
    [SerializeField] private Color gizmoColor = Color.blue;

    [SerializeField][ShowOnly] private Vector2 moveInputKeyboard;
    [SerializeField][ShowOnly] private Vector2 moveInputMouse;
    [SerializeField][ShowOnly] private Vector3 cameraPosition;
    [SerializeField] private Camera mainCamera;



    private void Awake()
    {
        mainCamera = Camera.main;
        targetZoom = mainCamera.orthographic ? mainCamera.orthographicSize : mainCamera.fieldOfView;
    }

    private void Update()
    {
        if (allowPanningWithMiddleMouse && isPanning)
        {
            HandlePanning();
        }

        MoveCamera(HandleMouseMovement(inputHandler.MousePosition) + moveInputKeyboard);
    }

    private void MoveCamera(Vector3 direction)
    {
        Vector3 movement = new Vector3(direction.x, direction.y, 0);
        cameraPosition = mainCamera.transform.position + movement * cameraSpeed * Time.deltaTime;

        cameraPosition.x = Mathf.Clamp(cameraPosition.x, cameraLimitsX.x, cameraLimitsX.y);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, cameraLimitsY.x, cameraLimitsY.y);
        cameraPosition.z = -10f;

        mainCamera.transform.position = cameraPosition;
    }

    public Vector2 HandleMouseMovement(Vector2 mousePosition)
    {
        if (!useMouseToMove) return Vector2.zero;

        moveInputMouse = Vector3.zero;

        if (mousePosition.x >= Screen.width - borderThickness)
        {
            moveInputMouse.x = 1;
        }
        else if (mousePosition.x <= borderThickness)
        {
            moveInputMouse.x = -1;
        }

        if (mousePosition.y >= Screen.height - borderThickness)
        {
            moveInputMouse.y = 1;
        }
        else if (mousePosition.y <= borderThickness)
        {
            moveInputMouse.y = -1;
        }

        return moveInputMouse;
    }

    public void OnMiddleMousePan(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isPanning = true;
            lastMousePosition = inputHandler.MousePosition; 
        }
        else if (context.canceled)
        {
            isPanning = false;
        }
    }

    private void HandlePanning()
    {
        Vector3 currentMousePosition = inputHandler.MousePosition;
        Vector3 mouseDelta = mainCamera.ScreenToViewportPoint(currentMousePosition - lastMousePosition);

        Vector3 move = new Vector3(-mouseDelta.x * cameraSpeed * cameraPanSpeed, -mouseDelta.y * cameraSpeed * cameraPanSpeed, 0);
        mainCamera.transform.position += move;

        lastMousePosition = currentMousePosition;
    }

    public void OnMoveCamera(InputAction.CallbackContext value)
    {
        if (!useKeyboardToMove)
        {
            moveInputKeyboard = Vector3.zero;
            return;
        }

        moveInputKeyboard = value.ReadValue<Vector2>();
    }

    public void OnMouseScroll(InputAction.CallbackContext value)
    {
        if (inputHandler.IsMouseInViewport())
        {
            float scrollDelta = value.ReadValue<Vector2>().y;

            if (scrollDelta > 0)
            {
                targetZoom -= zoomStep;
            }
            else if (scrollDelta < 0)
            {
                targetZoom += zoomStep;
            }

            if (mainCamera.orthographic)
            {
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            }
            else
            {
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            }

            if (zoomCoroutine != null)
            {
                StopCoroutine(zoomCoroutine);
            }

            zoomCoroutine = StartCoroutine(SmoothZoom());
        }
    }

    private IEnumerator SmoothZoom()
    {
        while (true)
        {
            if (mainCamera.orthographic)
            {
                mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetZoom, Time.deltaTime * zoomLerpSpeed);
                if (Mathf.Abs(mainCamera.orthographicSize - targetZoom) < 0.01f)
                    break;
            }
            else
            {
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetZoom, Time.deltaTime * zoomLerpSpeed);
                if (Mathf.Abs(mainCamera.fieldOfView - targetZoom) < 0.01f)
                    break;
            }

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (mainCamera == null) return;

        Gizmos.color = gizmoColor;
        Vector3 lowerLeft = new Vector3(cameraLimitsX.x, cameraLimitsY.x, 0);
        Vector3 upperRight = new Vector3(cameraLimitsX.y, cameraLimitsY.y, 0);
        Gizmos.DrawLine(lowerLeft, new Vector3(upperRight.x, lowerLeft.y, 0));
        Gizmos.DrawLine(lowerLeft, new Vector3(lowerLeft.x, upperRight.y, 0));
        Gizmos.DrawLine(upperRight, new Vector3(upperRight.x, lowerLeft.y, 0));
        Gizmos.DrawLine(upperRight, new Vector3(lowerLeft.x, upperRight.y, 0));
    }
}
