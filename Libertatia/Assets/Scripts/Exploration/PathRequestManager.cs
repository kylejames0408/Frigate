// Namespaces
using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Manages path requests for pathfinding.
/// </summary>
public class PathRequestManager : MonoBehaviour
{
    /// <summary>
    /// Path Request structure to keep track of paths.
    /// </summary>
    private struct PathRequest
    {
        #region Fields
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;
        #endregion

        #region Methods
        /// <summary>
        /// Constructs & initializes the PathRequst struct.
        /// </summary>
        /// <param name="_start">The starting point of the path.</param>
        /// <param name="_end">The end point of the path.</param>
        /// <param name="_callback">The action to call for the path.</param>
        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
        #endregion
    }

    #region Fields
    private readonly Queue<PathRequest> pathRequestQueue = new();   // the queue of requested paths
    private PathRequest currentPathRequest;                         // the current request

    private static PathRequestManager instance;                     // the instanced path request manager
	private Pathfinding pathfinding;                                // the pathfinding algorithms

    private bool isProcessingPath;                                  // whether or not the manager is already processing a path
    #endregion

    #region Unity Methods
    /// <summary>
    /// Initializes data when the script instance loads & creates the manager.
    /// </summary>
    void Awake()
	{
		instance = this;
		pathfinding = GetComponent<Pathfinding>();
	}
    #endregion

    #region Methods
    /// <summary>
    /// Requests the path.
    /// </summary>
    /// <param name="pathStart">The starting point.</param>
    /// <param name="pathEnd">The end point.</param>
    /// <param name="callback">The action.</param>
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
		PathRequest newRequest = new(pathStart, pathEnd, callback);
		instance.pathRequestQueue.Enqueue(newRequest);
		instance.TryProcessNext();
	}

    /// <summary>
    /// If the manager isn't processing a path, find the next path.
    /// </summary>
	void TryProcessNext()
    {
		if (!isProcessingPath && pathRequestQueue.Count > 0) {
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
		}
	}

    /// <summary>
    /// Call when the path is finished processing.
    /// </summary>
    /// <param name="path">The completed path.</param>
    /// <param name="success">Whether or not the process was successful.</param>
	public void FinishedProcessingPath(Vector3[] path, bool success) {
		currentPathRequest.callback(path, success);
		isProcessingPath = false;
		TryProcessNext();
	}
    #endregion
}