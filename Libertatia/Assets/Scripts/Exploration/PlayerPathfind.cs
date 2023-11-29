// Namespaces
using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Player that pathfinds to a target.
/// </summary>
public class PlayerPathfind : MonoBehaviour
{
    #region Fields
    public bool invert = true;

    private readonly float speed = 20;  // the speed to move at
    private Vector3[] path;             // the path to follow
    private int targetIndex;            // the target in the path to move toward

    public GameObject pathfindManager;  // the pathfinding manager
    private Grid grid;                  // the grid
    private Pathfinding pathfinding;    // the pathfinding algorithms

    private bool movingToIsland;        // whether the player is moving to an island
    private bool movingToOutpost;       // whether the player is moving to the outpost

    public GameObject targetPrefab;
    private Transform currentTarget;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        grid = pathfindManager.GetComponent<Grid>();
        pathfinding = pathfindManager.GetComponent<Pathfinding>();
    }

    void Update()
    {
        //AddTargets();
    }

    /// <summary>
    /// Draws unit path Gizmos UI.
    /// </summary>
    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
    #endregion

    #region Methods
    private void AddTargets()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            hit.point = new Vector3(hit.point.x, 0, hit.point.z);

            // Only add a target if the ship is still
            if (currentTarget == null)
            {
                // If the placement is invalid
                if (!grid.NodeFromWorldPoint(hit.point).walkable)
                {
                    // If it's an island, find the nearest walkable point and change the hit point to that position
                    if (Physics.CheckSphere(hit.point, grid.nodeRadius, LayerMask.GetMask("Terrain")))
                    {
                        // Find nearest walkable point & set island travel bool
                        hit.point = pathfinding.FindNearestWalkable(hit.point).worldPosition;
                        movingToIsland = true;
                    }
                    else if (Physics.CheckSphere(hit.point, grid.nodeRadius, LayerMask.GetMask("Building"))) // If it's the outpost...
                    {
                        // Find nearest walkable point & set island travel bool
                        hit.point = pathfinding.FindNearestWalkable(hit.point).worldPosition;
                        movingToOutpost = true;
                    }
                    else // Don't place the target
                    {
                        return;
                    }
                }

                currentTarget = Instantiate(targetPrefab, hit.point, Quaternion.identity).transform;
                PathRequestManager.RequestPath(transform.position, currentTarget.position, OnPathFound);
            }
        }
    }

    /// <summary>
    /// Start following the path when a path is established.
    /// </summary>
    /// <param name="newPath">The path to follow.</param>
    /// <param name="pathSuccessful">If the path is successful.</param>
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine(nameof(FollowPath));
            StartCoroutine(nameof(FollowPath));
        }
    }

    /// <summary>
    /// Moves the unit along the path.
    /// </summary>
    /// <returns>The routine to follow.</returns>
	private IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);

            if (transform.position == path[path.Length - 1])
            {
                Destroy(currentTarget.gameObject);
                yield break;
            }

            FaceTarget();

            yield return null;
        }
    }

    /// <summary>
    /// Turns the player to face the direction they're moving.
    /// </summary>
    private void FaceTarget()
    {
        Vector3 dir = path[targetIndex] - transform.position;

        if (invert)
        {
            transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Inverse(Quaternion.LookRotation(Vector3.back));
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }

    }

    internal int GetDistance(Vector3 shipPos)
    {
        if(currentTarget)
        {
            Destroy(currentTarget.gameObject);
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit);
        hit.point = new Vector3(hit.point.x, 0, hit.point.z);
        hit.point = pathfinding.FindNearestWalkable(hit.point).worldPosition;
        movingToIsland = true;
        currentTarget = Instantiate(targetPrefab, hit.point, Quaternion.identity).transform;
        return pathfinding.CalculatePath(shipPos, currentTarget.position).Length;
    }
    internal void Depart(Vector3 shipPos)
    {
        PathRequestManager.RequestPath(shipPos, currentTarget.position, OnPathFound);
    }
    #endregion
}