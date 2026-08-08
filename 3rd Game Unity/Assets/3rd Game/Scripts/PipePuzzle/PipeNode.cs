using UnityEngine;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class PipeNode : MonoBehaviour
{
    [Header("Pipe Settings")]
    public bool isStart = false;
    public bool isEnd = false;

    [Header("Pipe Connections")]
    public bool connectionUp = false;
    public bool connectionDown = false;
    public bool connectionLeft = false;
    public bool connectionRight = false;

    [HideInInspector]
    public Vector2Int gridPosition;
    [HideInInspector]
    public PipeNode parent;

    public void SetGridPosition(Vector2Int pos)
    {
        gridPosition = pos;
    }

    public bool HasConnection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up: return connectionUp;
            case Direction.Down: return connectionDown;
            case Direction.Left: return connectionLeft;
            case Direction.Right: return connectionRight;
            default: return false;
        }
    }

    // Call this when rotating the pipe
    public void RotatePipe(bool clockwise = true)
    {
        bool tempUp = connectionUp;
        bool tempRight = connectionRight;
        bool tempDown = connectionDown;
        bool tempLeft = connectionLeft;

        if (clockwise)
        {
            connectionUp = tempLeft;
            connectionRight = tempUp;
            connectionDown = tempRight;
            connectionLeft = tempDown;
        }
        else
        {
            connectionUp = tempRight;
            connectionRight = tempDown;
            connectionDown = tempLeft;
            connectionLeft = tempUp;
        }

        // Update visual rotation
        transform.Rotate(0, 0, clockwise ? -90f : 90f);

        // Notify the grid manager that pipes have changed
        // Using FindObjectsByType for the grid manager
        PipeGridManager gridManager = FindFirstObjectByType<PipeGridManager>();
        PipeMover mover = FindFirstObjectByType<PipeMover>();

        if (gridManager != null && mover != null)
        {
            gridManager.CheckAndMoveObject(mover.gameObject, gridPosition);
        }
    }
}