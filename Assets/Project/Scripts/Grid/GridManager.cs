using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private GameObject tilePrefab;

    public Tile[,] GridArray { get; private set; }

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        GridArray = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject tileObj = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                
                // Initialize Tile data and assign to the grid
                Tile tile = new Tile(new Vector2Int(x, y), TileType.Empty);
                GridArray[x, y] = tile;

                // Initialize TileComponent and attach Tile data to the prefab
                TileComponent tileComponent = tileObj.GetComponent<TileComponent>();
                if (tileComponent != null)
                {
                    tileComponent.Init(tile);
                }
            }
        }
    }
}
