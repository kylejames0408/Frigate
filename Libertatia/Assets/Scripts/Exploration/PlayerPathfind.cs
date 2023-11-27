// Namespaces
using System.Collections;
using System.Collections.Generic;
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

    public GameObject targetPrefab;
    private Queue<Transform> targetQueue;
    private Transform currentTarget;
    #endregion

    #region Unity Methods
    /// <summary>
    /// Request the path when the unit wakes up.
    /// </summary>
    void Start()
    {
        targetQueue = new Queue<Transform>();
    }

    void Update()
    {
        AddTargets();
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

            GameObject tempTarget = Instantiate(targetPrefab, hit.point, Quaternion.identity);
            targetQueue.Enqueue(tempTarget.transform);

            if (currentTarget == null)
            {
                currentTarget = targetQueue.Dequeue();
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

                switch (targetQueue.Count)
                {
                    case 0:
                        currentTarget = null;
                        break;
                    default:
                        currentTarget = targetQueue.Dequeue();
                        PathRequestManager.RequestPath(transform.position, currentTarget.position, OnPathFound);
                        break;
                }

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
    #endregion
}