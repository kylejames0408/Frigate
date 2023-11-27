// Namespaces
using UnityEngine;

/// <summary>
/// Nodes to store data for pathfinding, interfaced for usage in a heap.
/// </summary>
public class Node : IHeapItem<Node>
{
    #region Fields
    // Grid Fields
    public bool walkable;           // whether or not this node is walkable
    public Vector3 worldPosition;   // the position of the node in worldspace
    public int gridX;               // the x position
    public int gridY;               // the y position

    // Pathfinding Fields
    public int gCost;       // the perfect cost
    public int hCost;       // the estimated cost
    public Node parent;     // the parent node
    private int heapIndex;  // the node's index in the heap
    #endregion

    #region Properties
    /// <summary>
    /// The node's index in the heap.
    /// </summary>
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    /// <summary>
    /// The total cost of the node.
    /// </summary>
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Constructs & initializes the node.
    /// </summary>
    /// <param name="_walkable">Whether or not the node is walkable.</param>
    /// <param name="_worldPos">The node's position in the world.</param>
    /// <param name="_gridX">The node's x position in the grid.</param>
    /// <param name="_gridY">The node's y position in the grid.</param>
    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    /// <summary>
    /// Compares the cost of the nodes.
    /// </summary>
    /// <param name="nodeToCompare">The node to compare this node to.</param>
    /// <returns><0 if the node</returns>
	public int CompareTo(Node nodeToCompare)
    {
        // <0 if the fCost is less than the nodes fCost, >0 if the fCost is greater, 0 if equal
        int compare = fCost.CompareTo(nodeToCompare.fCost);

        // If the fCosts are the same, compare the hCosts
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        // Invert the comparison & return it
        return -compare;
    }
    #endregion
}