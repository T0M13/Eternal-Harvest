using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private DragObject dragObject;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }

    private void Start()
    {
        inputHandler.OnPickUpObject.AddListener(dragObject.StartDragging);
        inputHandler.OnDropObject.AddListener(dragObject.StopDragging);
    }
}
