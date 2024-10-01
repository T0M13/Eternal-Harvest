using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarPathfinding
{
    private Tilemap tilemap;
    private MapGenerator mapGenerator;

    public AStarPathfinding(Tilemap tilemap, MapGenerator mapGenerator)
    {
        this.tilemap = tilemap;
        this.mapGenerator = mapGenerator;
    }

    public List<Vector3Int> FindPath(Vector3Int startTile, Vector3Int targetTile)
    {
        List<Vector3Int> openList = new List<Vector3Int> { startTile };
        HashSet<Vector3Int> closedList = new HashSet<Vector3Int>();

        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float>
        {
            [startTile] = 0
        };
        Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float>
        {
            [startTile] = GetHeuristic(startTile, targetTile)
        };

        while (openList.Count > 0)
        {
            Vector3Int currentTile = GetTileWithLowestFScore(openList, fScore);

            if (currentTile == targetTile)
            {
                return ReconstructPath(cameFrom, currentTile);
            }

            openList.Remove(currentTile);
            closedList.Add(currentTile);

            foreach (Vector3Int neighbor in GetNeighbors(currentTile))
            {
                if (closedList.Contains(neighbor) || !IsTileWalkable(neighbor))
                {
                    continue;
                }

                float tentativeGScore = gScore[currentTile] + GetDistance(currentTile, neighbor);

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                cameFrom[neighbor] = currentTile;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + GetHeuristic(neighbor, targetTile);
            }
        }

        return null; // No path found
    }

    private Vector3Int GetTileWithLowestFScore(List<Vector3Int> openList, Dictionary<Vector3Int, float> fScore)
    {
        float lowestScore = float.MaxValue;
        Vector3Int bestTile = openList[0];

        foreach (var tile in openList)
        {
            if (fScore.ContainsKey(tile) && fScore[tile] < lowestScore)
            {
                lowestScore = fScore[tile];
                bestTile = tile;
            }
        }

        return bestTile;
    }

    private List<Vector3Int> GetNeighbors(Vector3Int tilePosition)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>
        {
            tilePosition + Vector3Int.up,
            tilePosition + Vector3Int.down,
            tilePosition + Vector3Int.left,
            tilePosition + Vector3Int.right
        };

        return neighbors;
    }

    private float GetHeuristic(Vector3Int tile, Vector3Int targetTile)
    {
        return Vector3Int.Distance(tile, targetTile);
    }

    private float GetDistance(Vector3Int a, Vector3Int b)
    {
        return Vector3Int.Distance(a, b);
    }

    private bool IsTileWalkable(Vector3Int tilePosition)
    {
        TileProperty tileProperty = mapGenerator.GetTileProperty(tilePosition);
        return tileProperty != null && tileProperty.GetProperty(TilePropertyType.Walkable);
    }

    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int currentTile)
    {
        List<Vector3Int> path = new List<Vector3Int> { currentTile };

        while (cameFrom.ContainsKey(currentTile))
        {
            currentTile = cameFrom[currentTile];
            path.Insert(0, currentTile);
        }

        return path;
    }
}
