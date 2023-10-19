using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[Serializable]
public class Crewmate : MonoBehaviour
{
    public int id;
    public string crewmateName;
    public Sprite icon;
    public bool isHovered = false;
    private Building currentBuilding;
    public int buildingID = -1;
    [SerializeField] private bool isBuilding = false;
    private NavMeshAgent agent;
    public UnityEvent onAssign;

    public string Name
    {
        get { return crewmateName; }
    }
    public Sprite Icon
    {
        get { return icon; }
    }
    public bool IsBuilding
    {
        get
        {
            return isBuilding;
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        id = gameObject.GetInstanceID();
        buildingID = -1;
    }
    private void Update()
    {
        if(isHovered && Input.GetMouseButtonDown(0))
        {
            CrewmateManager.Instance.ClickSelect(gameObject);
        }
    }

    public void GiveJob(Building job)
    {
        buildingID = job.id;
        currentBuilding = job;
        isBuilding = true;
        Vector3 jobPosition = job.transform.position;
        Vector2 randomPosition = UnityEngine.Random.insideUnitCircle.normalized * job.Radius;
        jobPosition.x += randomPosition.x;
        jobPosition.z += randomPosition.y;
        agent.destination = jobPosition;
        CrewmateManager.Instance.RemoveSelection(gameObject);
        onAssign.Invoke();
    }

    public void Free()
    {
        currentBuilding = null;
        isBuilding = false;
    }

    private void OnMouseEnter()
    {
        isHovered = true;
    }
    private void OnMouseExit()
    {
        isHovered = false;
    }
    private void OnDestroy()
    {
        UnitSelections selection = UnitSelections.Instance;
        if (selection != null)
        {
            selection.enemies.Remove(gameObject);
        }
    }
}
