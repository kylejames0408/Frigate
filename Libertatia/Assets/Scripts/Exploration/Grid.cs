// Namespaces
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Organizes node data for pathfinding.
/// </summary>
public class Grid : MonoBehaviour
{
    #region Fields
    public bool displayGridGizmos;      // whether or not to display the gizmos
    public LayerMask unwalkableMask;    // what mask to use as "unwalkable"
    public Vector2 gridWorldSize;       // what the world size is
    public float nodeRadius;            // the radius for the nodes
    private Node[,] grid;               // the grid data

    private float nodeDiameter;         // the diameter of the nodes
    private int gridSizeX, gridSizeY;   // the grid size
    #endregion

    #region Properties
    /// <summary>
    /// Returns the grid surface area.
    /// </summary>
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    public Node[,] NodeData
    {
        get
        {
            return grid;
        }
    }
    #endregion

    #region Unity Methods
    /// <summary>
    /// Initializes data when the script instance loads & creates the grid.
    /// </summary>
    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    /// <summary>
    /// Draws grid Gizmos UI if enabled.
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Creates the grid w/ initialized data.
    /// </summary>
    private void CreateGrid()
    {
        // Create a new grid of nodes
        grid = new Node[gridSizeX, gridSizeY];

        // Get the bottom left position of the world
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        // Generate each node in the grid
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    /// <summary>
    /// Get a node's neighbors.
    /// </summary>
    /// <param name="node">The node to find neighbors for.</param>
    /// <returns>A list of the node's neighbors.</returns>
    public List<Node> GetNeighbours(Node node)
    {
        // Set up an empty list of neighbors
        List<Node> neighbours = new();

        // Loop through possible neighbor positions
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // If it's the node you're checking
                if (x == 0 && y == 0)
                {
                    continue;
                }

                // Get x & y relative coordinates to check
                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // If it's a neighbor in the grid, add it to the list
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        // Return the neighbors
        return neighbours;
    }

    /// <summary>
    /// Gets the node from the world position.
    /// </summary>
    /// <param name="worldPosition">The position to get the node from.</param>
    /// <returns>The node in the world position.</returns>
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }
    #endregion
}