using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class WanderState : AIState
{
    public WanderStateConfig Config;

    private Vector3 previousDirection = Vector3.zero;
    private Queue<Vector3Int> currentPath = new Queue<Vector3Int>();
    private Vector3 targetPosition;
    private Tilemap currentTilemap;
    private MapGenerator mapGenerator;

    public override void EnterState(AIAgent agent)
    {
        mapGenerator = agent.MapGenerator;  // Fetch MapGenerator from AIAgent
        currentTilemap = mapGenerator.placeholderTilemap;
        GetNewTargetPosition(agent);  // Get a new random target position
    }

    public override void UpdateState(AIAgent agent)
    {
        if (currentPath.Count > 0)
        {
            Vector3Int nextPathTile = currentPath.Peek();
            Vector3 targetTilePosition = currentTilemap.CellToWorld(nextPathTile) + new Vector3(currentTilemap.cellSize.x / 2, currentTilemap.cellSize.y / 2, 0);  // Move to the tile center
            Vector3 direction = (targetTilePosition - agent.transform.position).normalized;

            if (direction != previousDirection)
            {
                string walkAnimation = GetAnimationStateFromDirection(direction);
                agent.AiAnimator.Play(walkAnimation);
                previousDirection = direction;
            }

            // Move the minion towards the next tile in the path
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, targetTilePosition, Time.deltaTime * Config.wanderSpeed);

            // If the agent reaches the current tile, dequeue and move to the next tile
            if (Vector3.Distance(agent.transform.position, targetTilePosition) <= 0.1f)
            {
                currentPath.Dequeue();

                // If path is completed, get a new target position
                if (currentPath.Count == 0)
                {
                    GetNewTargetPosition(agent);
                }
            }
        }
        else
        {
            // If there's no path, always use A* to find a path to the new target
            GetNewTargetPosition(agent);
        }
    }

    public override void ExitState(AIAgent agent) { }

    public override AIStateType GetStateType()
    {
        return AIStateType.Wander;
    }

    public override void DrawGizmos(AIAgent agent)
    {
        Gizmos.color = Config.gizmoColor;
        Gizmos.DrawWireSphere(agent.transform.position, Config.maxTileDistance);
        Gizmos.DrawWireSphere(agent.transform.position, Config.minTileDistance);

        if (targetPosition != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPosition, 0.3f);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(agent.transform.position, targetPosition);
    }

    private void GetNewTargetPosition(AIAgent agent)
    {
        if (currentTilemap != null)
        {
            // Find a random tile within the given distance constraints (minTileDistance, maxTileDistance)
            Vector3Int nextTile = GetRandomTileWithinTilemap(agent, currentTilemap);

            if (nextTile != Vector3Int.zero)
            {
                targetPosition = currentTilemap.CellToWorld(nextTile);
                // Always use A* to calculate the path
                currentPath = CalculateAStarPath(agent, nextTile);
            }
        }
    }

    private Vector3Int GetRandomTileWithinTilemap(AIAgent agent, Tilemap tilemap)
    {
        Vector3Int currentGridPos = tilemap.WorldToCell(agent.transform.position);
        List<Vector3Int> walkableTiles = new List<Vector3Int>();

        for (int i = 0; i < Config.maxWanderAttempts; i++)
        {
            // Randomly choose a point within a circular radius using the distanceBias and tile distances
            Vector2 randomOffset = Random.insideUnitCircle.normalized * Mathf.Lerp(Config.minTileDistance, Config.maxTileDistance, Mathf.Pow(Random.value, Config.distanceBias));

            Vector3Int randomGridPos = currentGridPos + new Vector3Int(Mathf.RoundToInt(randomOffset.x), Mathf.RoundToInt(randomOffset.y), 0);

            if (IsTileWalkable(randomGridPos))
            {
                walkableTiles.Add(randomGridPos);
            }
        }

        if (walkableTiles.Count > 0)
        {
            return walkableTiles[Random.Range(0, walkableTiles.Count)];
        }

        return currentGridPos;  // Default to the current tile if no walkable tiles are found
    }

    // Checks if a tile is walkable based on the TileProperty from the MapGenerator
    private bool IsTileWalkable(Vector3Int tilePosition)
    {
        TileProperty tileProperty = mapGenerator.GetTileProperty(tilePosition);
        return tileProperty != null && tileProperty.GetProperty(TilePropertyType.Walkable);
    }

    // Calculate A* pathfinding from the agent's current position to the target tile
    private Queue<Vector3Int> CalculateAStarPath(AIAgent agent, Vector3Int targetTile)
    {
        Tilemap tilemap = mapGenerator.placeholderTilemap;
        Vector3Int startTile = tilemap.WorldToCell(agent.transform.position);

        AStarPathfinding aStar = new AStarPathfinding(tilemap, mapGenerator);
        List<Vector3Int> path = aStar.FindPath(startTile, targetTile);

        if (path == null || path.Count == 0)
        {
            path = new List<Vector3Int> { startTile };  // If no path is found, stay at the current position
        }

        return new Queue<Vector3Int>(path);  // Return the calculated path
    }

    private string GetAnimationStateFromDirection(Vector3 direction)
    {
        if (direction.x > 0 && Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            return Config.walkRight;
        }
        else if (direction.x < 0 && Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            return Config.walkLeft;
        }
        else if (direction.y > 0 && Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
        {
            return Config.walkUp;
        }
        else
        {
            return Config.walkDown;
        }
    }
}
