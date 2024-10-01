using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class TileInfoDisplay : MonoBehaviour
{
    [Header("References")]
    public Tilemap placeholderTilemap; // Reference to your tilemap
    public TextMeshProUGUI tileInfoText; // Reference to the UI Text component for displaying tile info

    private MapGenerator mapGenerator;

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
    }

    private void Update()
    {
        ShowTileInfoUnderMouse();

        // Check for left mouse button click
        //if (Input.GetMouseButtonDown(0)) // Left mouse button is pressed
        //{
        //    UpdateTileToBlockedOnly();
        //}
    }

    // Detects the tile under the mouse and shows its info
    private void ShowTileInfoUnderMouse()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = placeholderTilemap.WorldToCell(mouseWorldPosition);
        TileBase tile = placeholderTilemap.GetTile(gridPosition);

        if (tile != null)
        {
            TileProperty tileProperty = mapGenerator.GetTileProperty(gridPosition);
            if (tileProperty != null)
            {
                // Display all tile properties dynamically
                StringBuilder tileInfo = new StringBuilder();
                tileInfo.AppendLine($"Tile: {tile.name}");

                Dictionary<TilePropertyType, bool> properties = tileProperty.GetAllProperties();
                foreach (var property in properties)
                {
                    tileInfo.AppendLine($"{property.Key}: {property.Value}");
                }

                tileInfoText.text = tileInfo.ToString();
            }
        }
        else
        {
            tileInfoText.text = "No tile under the mouse";
        }
    }

    // Updates the clicked tile to only have the 'Blocked' property
    private void UpdateTileToBlockedOnly()
    {
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int gridPosition = placeholderTilemap.WorldToCell(mouseWorldPosition);
        TileBase tile = placeholderTilemap.GetTile(gridPosition);

        if (tile != null)
        {
            // Clear all properties and add the Blocked property only
            mapGenerator.ClearTileProperties(gridPosition);
            mapGenerator.AddTileProperty(gridPosition, TilePropertyType.Blocked, true);

            // Update the tile info display to reflect the changes
            ShowTileInfoUnderMouse();
        }
    }
}
