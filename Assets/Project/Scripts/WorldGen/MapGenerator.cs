using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System;
using Unity.Collections.LowLevel.Unsafe;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapWidth = 100;
    public int mapHeight = 100;
    public float noiseScale = 50f;

    [Header("Noise Settings")]
    public int seed = 42;
    public int octaves = 4;
    [Range(0, 1)] public float persistence = 0.5f;
    public float lacunarity = 2f;
    public Vector2 offset;

    [Header("Tilemap and Tiles")]
    public Tilemap placeholderTilemap;
    public Tilemap displayTilemap;

    public TileBase grassPlaceholderTile;
    public TileBase waterPlaceholderTile;

    public TileBase[] tiles; // 16 tiles array representing all configurations   
    public int numThresholds = 2;

    [Header("Noise Preview")]
    public bool showNoise = false;
    [SerializeField, HideInInspector] private Texture2D noiseTexture;

    private float[,] noiseMap;
    protected static Vector3Int[] NEIGHBOURS = new Vector3Int[] {
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 1, 0)
    };

    // Dictionary for neighbor tuple-to-tile mapping
    protected static Dictionary<Tuple<TileType, TileType, TileType, TileType>, TileBase> neighbourTupleToTile;

    public Dictionary<Vector3Int, TileProperty> tilePropertiesDictionary;

    public Texture2D NoiseTexture { get => noiseTexture; set => noiseTexture = value; }

    private void Awake()
    {
        InitializeNeighbourTileDictionary();
        GenerateEverything();
    }

    // Function to generate everything
    public void GenerateEverything()
    {
        GenerateMap();       // Generates both placeholder and refreshes display
    }

    // Function to generate only the placeholder tiles
    public void GeneratePlaceholdersOnly()
    {
        placeholderTilemap.ClearAllTiles();
        noiseMap = NoiseGenerator.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistence, lacunarity, offset, seed, numThresholds);
        GenerateTilesFromNoise(noiseMap);
    }

    // Function to refresh the display based on placeholders
    public void RefreshDisplayOnly()
    {
        RefreshDisplayTilemap(); // Only refreshes display tiles
    }

    public void ClearPlaceHolderMap()
    {
        placeholderTilemap.ClearAllTiles();
    }

    public void GenerateMap()
    {
        placeholderTilemap.ClearAllTiles();
        noiseMap = NoiseGenerator.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistence, lacunarity, offset, seed, numThresholds);

        if (showNoise)
            GenerateNoiseTexture(noiseMap);

        GenerateTilesFromNoise(noiseMap);
        RefreshDisplayTilemap();
        InitializeTileProperties();
    }

    private void GenerateNoiseTexture(float[,] noiseMap)
    {
        NoiseTexture = new Texture2D(mapWidth, mapHeight);

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float value = noiseMap[x, y];
                Color color = Color.Lerp(Color.black, Color.white, value);
                NoiseTexture.SetPixel(x, y, color);
            }
        }

        NoiseTexture.Apply();
    }

    // Generate placeholder tiles based on noise values
    private void GenerateTilesFromNoise(float[,] noiseMap)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                float noiseValue = noiseMap[x, y];

                TileBase tile = GetTileFromNoiseValue(noiseValue);
                placeholderTilemap.SetTile(tilePosition, tile);
            }
        }
    }

    // Get a tile based on the noise value
    private TileBase GetTileFromNoiseValue(float noiseValue)
    {
        int tileIndex = Mathf.FloorToInt(noiseValue * numThresholds);
        tileIndex = Mathf.Clamp(tileIndex, 0, tiles.Length - 1);

        return tileIndex == 0 ? grassPlaceholderTile : waterPlaceholderTile;
    }

    // Initialize neighbor-tile configuration dictionary
    private void InitializeNeighbourTileDictionary()
    {
        neighbourTupleToTile = new Dictionary<Tuple<TileType, TileType, TileType, TileType>, TileBase>
        {
            {new (TileType.Grass, TileType.Grass, TileType.Grass, TileType.Grass), tiles[6]},
            {new (TileType.Water, TileType.Water, TileType.Water, TileType.Grass), tiles[13]},
            {new (TileType.Water, TileType.Water, TileType.Grass, TileType.Water), tiles[0]},
            {new (TileType.Water, TileType.Grass, TileType.Water, TileType.Water), tiles[8]},
            {new (TileType.Grass, TileType.Water, TileType.Water, TileType.Water), tiles[15]},
            {new (TileType.Water, TileType.Grass, TileType.Water, TileType.Grass), tiles[1]},
            {new (TileType.Grass, TileType.Water, TileType.Grass, TileType.Water), tiles[11]},
            {new (TileType.Water, TileType.Water, TileType.Grass, TileType.Grass), tiles[3]},
            {new (TileType.Grass, TileType.Grass, TileType.Water, TileType.Water), tiles[9]},
            {new (TileType.Water, TileType.Grass, TileType.Grass, TileType.Grass), tiles[5]},
            {new (TileType.Grass, TileType.Water, TileType.Grass, TileType.Grass), tiles[2]},
            {new (TileType.Grass, TileType.Grass, TileType.Water, TileType.Grass), tiles[10]},
            {new (TileType.Grass, TileType.Grass, TileType.Grass, TileType.Water), tiles[7]},
            {new (TileType.Water, TileType.Grass, TileType.Grass, TileType.Water), tiles[14]},
            {new (TileType.Grass, TileType.Water, TileType.Water, TileType.Grass), tiles[4]},
            {new (TileType.Water, TileType.Water, TileType.Water, TileType.Water), tiles[12]}
        };
    }

    // Update the display tilemap by checking neighbors and setting the correct tile
    private void RefreshDisplayTilemap()
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                // Get the neighboring tile types (top-left, top-right, bottom-left, bottom-right)
                TileType topRight = GetPlaceholderTileTypeAt(tilePosition - NEIGHBOURS[0]);
                TileType topLeft = GetPlaceholderTileTypeAt(tilePosition - NEIGHBOURS[1]);
                TileType bottomRight = GetPlaceholderTileTypeAt(tilePosition - NEIGHBOURS[2]);
                TileType bottomLeft = GetPlaceholderTileTypeAt(tilePosition - NEIGHBOURS[3]);


                // Get the corresponding display tile based on neighbors
                TileBase displayTile = CalculateDisplayTile(topLeft, topRight, bottomLeft, bottomRight);
                displayTilemap.SetTile(tilePosition, displayTile);
            }
        }
    }

    private TileBase CalculateDisplayTile(TileType topLeft, TileType topRight, TileType bottomLeft, TileType bottomRight)
    {
        Tuple<TileType, TileType, TileType, TileType> neighbourTuple = new(topLeft, topRight, bottomLeft, bottomRight);
        return neighbourTupleToTile.ContainsKey(neighbourTuple) ? neighbourTupleToTile[neighbourTuple] : tiles[12]; // Default tile
    }

    private TileType GetPlaceholderTileTypeAt(Vector3Int coords)
    {
        if (placeholderTilemap.GetTile(coords) == grassPlaceholderTile)
            return TileType.Grass;
        else
            return TileType.Water;
    
   }


    private void InitializeTileProperties()
    {
        tilePropertiesDictionary = new Dictionary<Vector3Int, TileProperty>();

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                TileBase tile = placeholderTilemap.GetTile(tilePosition);

                TileProperty tileProperty = new TileProperty();

                if (tile == grassPlaceholderTile)
                {

                    AddTileProperty(tilePosition, TilePropertyType.Walkable, true);
                    AddTileProperty(tilePosition, TilePropertyType.Blocked, false);
                }
                else if (tile == waterPlaceholderTile)
                {
                    AddTileProperty(tilePosition, TilePropertyType.Walkable, false);
                    AddTileProperty(tilePosition, TilePropertyType.Blocked, true);
                }

            }
        }
    }

    public void UpdateTileProperty(Vector3Int tilePosition, TilePropertyType propertyType, bool value)
    {
        if (tilePropertiesDictionary.ContainsKey(tilePosition))
        {
            tilePropertiesDictionary[tilePosition].SetProperty(propertyType, value);
        }
    }

    public void AddTileProperty(Vector3Int tilePosition, TilePropertyType propertyType, bool value)
    {
        if (tilePropertiesDictionary.ContainsKey(tilePosition))
        {
            if (!tilePropertiesDictionary[tilePosition].HasProperty(propertyType))
            {
                tilePropertiesDictionary[tilePosition].AddProperty(propertyType, value);
            }
        }
        else
        {
            TileProperty newTileProperty = new TileProperty();
            newTileProperty.AddProperty(propertyType, value);
            tilePropertiesDictionary[tilePosition] = newTileProperty;
        }
    }

    public void RemoveTileProperty(Vector3Int tilePosition, TilePropertyType propertyType)
    {
        if (tilePropertiesDictionary.ContainsKey(tilePosition))
        {
            tilePropertiesDictionary[tilePosition].RemoveProperty(propertyType);
        }
    }

    public void ClearTileProperties(Vector3Int tilePosition)
    {
        if (tilePropertiesDictionary.ContainsKey(tilePosition))
        {
            tilePropertiesDictionary[tilePosition].ClearAllProperties();
        }
    }

    public TileProperty GetTileProperty(Vector3Int tilePosition)
    {
        if (tilePropertiesDictionary.ContainsKey(tilePosition))
        {
            return tilePropertiesDictionary[tilePosition];
        }

        return null;
    }

}
