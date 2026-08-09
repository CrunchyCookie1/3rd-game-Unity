using System.Collections.Generic;
using UnityEngine;

public class PipeGridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public Vector2Int gridSize = new Vector2Int(5, 5);
    public float cellSize = 1f;
    public Vector3 gridOrigin = Vector3.zero;

    [Header("Pipe References")]
    public GameObject startPipe;
    public GameObject endPipe;

    private Dictionary<Vector2Int, PipeNode> pipeNodes = new Dictionary<Vector2Int, PipeNode>();
    private Queue<PipeNode> pathfindingQueue = new Queue<PipeNode>();
    private List<PipeNode> visitedNodes = new List<PipeNode>();

    public Vector2Int StartPosition { get; private set; }
    public Vector2Int EndPosition { get; private set; }

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        // Register all pipes in the grid using FindObjectsByType
        PipeNode[] allPipes = FindObjectsByType<PipeNode>(FindObjectsSortMode.None);
        foreach (PipeNode pipe in allPipes)
        {
            Vector2Int gridPos = WorldToGrid(pipe.transform.position);
            pipeNodes[gridPos] = pipe;
            pipe.SetGridPosition(gridPos);

            if (pipe.isStart) StartPosition = gridPos;
            if (pipe.isEnd) EndPosition = gridPos;
        }
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition.x - gridOrigin.x) / cellSize);
        int y = Mathf.RoundToInt((worldPosition.z - gridOrigin.z) / cellSize);
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(
            gridOrigin.x + gridPosition.x * cellSize,
            0,
            gridOrigin.z + gridPosition.y * cellSize
        );
    }

    public bool FindPathToEnd(Vector2Int startPos, out List<Vector2Int> path)
    {
        path = new List<Vector2Int>();

        if (!pipeNodes.ContainsKey(startPos) || !pipeNodes.ContainsKey(EndPosition))
            return false;

        pathfindingQueue.Clear();
        visitedNodes.Clear();

        PipeNode startNode = pipeNodes[startPos];
        startNode.parent = null;
        pathfindingQueue.Enqueue(startNode);
        visitedNodes.Add(startNode);

        while (pathfindingQueue.Count > 0)
        {
            PipeNode currentNode = pathfindingQueue.Dequeue();

            // Check if we reached the end
            if (currentNode.gridPosition == EndPosition)
            {
                // Reconstruct path
                PipeNode node = currentNode;
                while (node != null)
                {
                    path.Add(node.gridPosition);
                    node = node.parent;
                }
                path.Reverse();
                return true;
            }

            // Get all valid neighbors based on pipe connections
            List<PipeNode> neighbors = GetConnectedNeighbors(currentNode);

            foreach (PipeNode neighbor in neighbors)
            {
                if (!visitedNodes.Contains(neighbor))
                {
                    neighbor.parent = currentNode;
                    pathfindingQueue.Enqueue(neighbor);
                    visitedNodes.Add(neighbor);
                }
            }
        }

        return false;
    }

    List<PipeNode> GetConnectedNeighbors(PipeNode node)
    {
        List<PipeNode> neighbors = new List<PipeNode>();
        Vector2Int currentPos = node.gridPosition;

        // Check each direction based on pipe connections
        if (node.HasConnection(Direction.Up))
        {
            Vector2Int upPos = currentPos + Vector2Int.up;
            if (pipeNodes.ContainsKey(upPos) && pipeNodes[upPos].HasConnection(Direction.Down))
                neighbors.Add(pipeNodes[upPos]);
        }

        if (node.HasConnection(Direction.Down))
        {
            Vector2Int downPos = currentPos + Vector2Int.down;
            if (pipeNodes.ContainsKey(downPos) && pipeNodes[downPos].HasConnection(Direction.Up))
                neighbors.Add(pipeNodes[downPos]);
        }

        if (node.HasConnection(Direction.Left))
        {
            Vector2Int leftPos = currentPos + Vector2Int.left;
            if (pipeNodes.ContainsKey(leftPos) && pipeNodes[leftPos].HasConnection(Direction.Right))
                neighbors.Add(pipeNodes[leftPos]);
        }

        if (node.HasConnection(Direction.Right))
        {
            Vector2Int rightPos = currentPos + Vector2Int.right;
            if (pipeNodes.ContainsKey(rightPos) && pipeNodes[rightPos].HasConnection(Direction.Left))
                neighbors.Add(pipeNodes[rightPos]);
        }

        return neighbors;
    }

    public void CheckAndMoveObject(GameObject movingObject, Vector2Int currentPos)
    {
        List<Vector2Int> path;
        if (FindPathToEnd(currentPos, out path))
        {
            // Path found - move the object along the path
            PipeMover mover = movingObject.GetComponent<PipeMover>();
            if (mover != null)
            {
                mover.StartMoving(path, this);
            }
        }
        else
        {
            // No path to end - trigger the event
            OnPathNotFound(movingObject);
        }
    }

    void OnPathNotFound(GameObject movingObject)
    {
        // Trigger your custom event
        PipeMover mover = movingObject.GetComponent<PipeMover>();
        if (mover != null)
        {
            mover.OnPathNotFound();
        }

        // You can also broadcast the event globally
        Debug.Log("Path not found! Object stopped at: " + movingObject.transform.position);
        // Add your custom event logic here
    }
}