using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Finds paths for agents.
/// </summary>
public class Pathfinding : MonoBehaviour
{
	#region Fields
	private PathRequestManager requestManager;	// the request manager
	private Grid grid;                          // the grid to path find on
    #endregion

    #region Unity Methods
    /// <summary>
    /// Initializes data when the script instance loads & creates the manager & gets the grid.
    /// </summary>
    void Awake()
	{
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<Grid>();
	}
    #endregion

    #region Methods
	/// <summary>
	/// Starts finding a path from the start position to the target.
	/// </summary>
	/// <param name="startPos">The starting position.</param>
	/// <param name="targetPos">The end position.</param>
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
	{
		StartCoroutine(FindPath(startPos, targetPos));
	}


    public Vector3[] CalculatePath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new(grid.MaxSize);
            HashSet<Node> closedSet = new();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                    }
                }
            }
        }

        return RetracePath(startNode, targetNode);
    }

    /// <summary>
    /// Finds a path from the starting position to the target position.
    /// </summary>
    /// <param name="startPos">The starting position.</param>
    /// <param name="targetPos">The end position.</param>
    /// <returns></returns>
    private IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
	{
		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;

		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);


		if (startNode.walkable && targetNode.walkable) {
			Heap<Node> openSet = new(grid.MaxSize);
			HashSet<Node> closedSet = new();
			openSet.Add(startNode);

			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);

				if (currentNode == targetNode) {
					pathSuccess = true;
					break;
				}

				foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
					if (!neighbour.walkable || closedSet.Contains(neighbour)) {
						continue;
					}

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;

						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}
		}

		if (pathSuccess)
		{
			waypoints = RetracePath(startNode,targetNode);
		}

		requestManager.FinishedProcessingPath(waypoints,pathSuccess);

        yield return null;
    }

	public Node FindNearestWalkable(Vector3 targetPos)
	{
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

		int nearestDistance = int.MaxValue;
        Node nearestNode = grid.NodeData[0,0];

        foreach (Node currentNode in grid.NodeData)
		{
			if (currentNode.walkable)
			{
				int tempDist = GetDistance(currentNode, targetNode);

				if (tempDist < nearestDistance)
				{
					nearestDistance = tempDist;
					nearestNode = currentNode;
				}
			}
		}

		return nearestNode;
	}

	/// <summary>
	/// Retrace the path.
	/// </summary>
	/// <param name="startNode">The starting node.</param>
	/// <param name="endNode">The ending node.</param>
	/// <returns>Returns the path vertices.</returns>
	Vector3[] RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new();
		Node currentNode = endNode;

		// Loop through nodes until the path connects
		while (currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}

		// Simplify & return the path
		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);

		return waypoints;

	}

	/// <summary>
	/// Simplifies the path.
	/// </summary>
	/// <param name="path">The path to simplify.</param>
	/// <returns>The simplified path.</returns>
	Vector3[] SimplifyPath(List<Node> path)
	{
		List<Vector3> waypoints = new();
		Vector2 directionOld = Vector2.zero;

		for (int i = 1; i < path.Count; i ++)
		{
			Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);

			if (directionNew != directionOld)
			{
				waypoints.Add(path[i].worldPosition);
			}

			directionOld = directionNew;
		}

		return waypoints.ToArray();
	}

	/// <summary>
	/// Gets the distance between nodes
	/// </summary>
	/// <param name="nodeA"></param>
	/// <param name="nodeB"></param>
	/// <returns></returns>
	private int GetDistance(Node nodeA, Node nodeB)
	{
		// Get x & y distances
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
		{
			return 14 * dstY + 10 * (dstX - dstY);
		}

		return 14 * dstX + 10 * (dstY - dstX);
	}
    #endregion
}