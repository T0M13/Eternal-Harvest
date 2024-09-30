using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class WanderState : AIState
{
    public WanderStateConfig Config;

    private Coroutine wanderCoroutine;
    private Vector3 previousDirection = Vector3.zero;
    private Queue<Vector3Int> currentPath = new Queue<Vector3Int>();

    public override void EnterState(AIAgent agent)
    {
        wanderCoroutine = agent.StartCoroutine(Wander(agent));
    }

    public override void UpdateState(AIAgent agent) { }

    public override void ExitState(AIAgent agent)
    {
        if (wanderCoroutine != null)
        {
            agent.StopCoroutine(wanderCoroutine);
        }
    }

    public override AIStateType GetStateType()
    {
        return AIStateType.Wander;
    }

    public override void DrawGizmos(AIAgent agent)
    {
        Gizmos.color = Config.gizmoColor;
        Gizmos.DrawWireSphere(agent.transform.position, Config.wanderRadius);
    }

    private IEnumerator Wander(AIAgent agent)
    {
        while (true)
        {
            // Get the Tilemap beneath the minion
            Tilemap currentTilemap = GetTilemapBeneathMinion(agent);

            if (currentTilemap != null)
            {
                // Get a random tile within that tilemap
                Vector3Int nextTile = GetRandomTileWithinTilemap(agent, currentTilemap);

                if (nextTile != Vector3Int.zero)
                {
                    Vector3 worldPosition = currentTilemap.CellToWorld(nextTile);

                    // Move the minion to the random tile smoothly, tile by tile
                    currentPath = CalculatePath(agent, nextTile);

                    while (currentPath.Count > 0)
                    {
                        Vector3Int nextPathTile = currentPath.Dequeue();
                        Vector3 targetPosition = currentTilemap.CellToWorld(nextPathTile);
                        Vector3 direction = (targetPosition - agent.transform.position).normalized;

                        if (direction != previousDirection)
                        {
                            string walkAnimation = GetAnimationStateFromDirection(direction);
                            agent.AiAnimator.Play(walkAnimation);
                        }

                        previousDirection = direction;

                        // Move the agent toward the next tile
                        agent.transform.position = Vector3.Lerp(agent.transform.position, targetPosition, Time.deltaTime * agent.MoveSpeed);
                        yield return null;  // Wait for the next frame
                    }
                }
            }

            yield return new WaitForSeconds(Config.walkDuration);
        }
    }

    // Get the Tilemap beneath the minion's current position using OverlapPoint
    private Tilemap GetTilemapBeneathMinion(AIAgent agent)
    {
        Collider2D hitCollider = Physics2D.OverlapPoint(agent.transform.position, Config.walkableLayerMask);
        if (hitCollider != null)
        {
            return hitCollider.GetComponent<Tilemap>();
        }
        return null;
    }

    // Get a random valid tile from the Tilemap beneath the minion
    private Vector3Int GetRandomTileWithinTilemap(AIAgent agent, Tilemap tilemap)
    {
        Vector3Int currentGridPos = tilemap.WorldToCell(agent.transform.position);
        List<Vector3Int> walkableTiles = new List<Vector3Int>();

        foreach (Vector3Int direction in Config.directions)
        {
            Vector3Int neighborPos = currentGridPos + direction;

            if (IsTileWalkable(tilemap, neighborPos))
            {
                walkableTiles.Add(neighborPos);
            }
        }

        if (walkableTiles.Count > 0)
        {
            // Pick a random valid tile from nearby tiles
            int randomIndex = Random.Range(0, walkableTiles.Count);
            return walkableTiles[randomIndex];
        }

        return Vector3Int.zero;
    }

    // Check if a tile is walkable based on the tilemap
    private bool IsTileWalkable(Tilemap tilemap, Vector3Int tilePosition)
    {
        TileBase tile = tilemap.GetTile(tilePosition);
        return tile != null;  // Basic check if a tile exists in the tilemap
    }

    // Pathfinding logic to calculate the path to the target tile (tile by tile)
    // A* Pathfinding method to calculate the path to the target tile
    private Queue<Vector3Int> CalculatePath(AIAgent agent, Vector3Int targetTile)
    {
        Tilemap tilemap = GetTilemapBeneathMinion(agent);
        Vector3Int startTile = tilemap.WorldToCell(agent.transform.position);

        // Priority queue or list for open set of tiles to evaluate
        List<Vector3Int> openSet = new List<Vector3Int> { startTile };

        // Keep track of the best path to each tile
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        // gScore is the cost from start to the current tile
        Dictionary<Vector3Int, float> gScore = new Dictionary<Vector3Int, float>();
        gScore[startTile] = 0;

        // fScore is the estimated total cost from start to the goal (gScore + heuristic)
        Dictionary<Vector3Int, float> fScore = new Dictionary<Vector3Int, float>();
        fScore[startTile] = Vector3Int.Distance(startTile, targetTile);

        // Closed set of evaluated tiles
        HashSet<Vector3Int> closedSet = new HashSet<Vector3Int>();

        while (openSet.Count > 0)
        {
            // Get the tile with the lowest fScore
            Vector3Int currentTile = GetLowestFScoreTile(openSet, fScore);

            // If we've reached the target tile, reconstruct the path
            if (currentTile == targetTile)
            {
                return ReconstructPath(cameFrom, currentTile);
            }

            // Move current tile from openSet to closedSet
            openSet.Remove(currentTile);
            closedSet.Add(currentTile);

            // Check neighboring tiles (using Config.directions)
            foreach (Vector3Int direction in Config.directions)
            {
                Vector3Int neighborTile = currentTile + direction;

                // Skip neighbors that are not walkable or already evaluated
                if (!IsTileWalkable(tilemap, neighborTile) || closedSet.Contains(neighborTile))
                    continue;

                // Tentative gScore for neighbor
                float tentativeGScore = gScore[currentTile] + Vector3Int.Distance(currentTile, neighborTile);

                if (!openSet.Contains(neighborTile))
                {
                    openSet.Add(neighborTile);  // Discover new tile
                }
                else if (tentativeGScore >= gScore[neighborTile])
                {
                    continue;  // Not a better path
                }

                // Record the best path to this tile
                cameFrom[neighborTile] = currentTile;
                gScore[neighborTile] = tentativeGScore;
                fScore[neighborTile] = gScore[neighborTile] + Vector3Int.Distance(neighborTile, targetTile);  // Heuristic
            }
        }

        return new Queue<Vector3Int>();  // No valid path found
    }

    // Get the tile with the lowest fScore
    private Vector3Int GetLowestFScoreTile(List<Vector3Int> openSet, Dictionary<Vector3Int, float> fScore)
    {
        Vector3Int bestTile = openSet[0];
        float lowestFScore = fScore[bestTile];

        foreach (var tile in openSet)
        {
            if (fScore.ContainsKey(tile) && fScore[tile] < lowestFScore)
            {
                bestTile = tile;
                lowestFScore = fScore[tile];
            }
        }

        return bestTile;
    }

    // Reconstruct the path from the pathfinding algorithm
    private Queue<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int currentTile)
    {
        Stack<Vector3Int> path = new Stack<Vector3Int>();

        while (cameFrom.ContainsKey(currentTile))
        {
            path.Push(currentTile);
            currentTile = cameFrom[currentTile];
        }

        return new Queue<Vector3Int>(path);
    }


    // Get the correct animation state based on the minion's movement direction
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
