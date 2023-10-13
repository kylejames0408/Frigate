using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Crewmate : MonoBehaviour
{
    [SerializeField] private string crewmateName;
    [SerializeField] private Sprite icon;
    public bool isHovered = false;
    private Building currentBuilding;
    [SerializeField] private bool isBuilding = false;
    //private NavMeshAgent agent;

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
        //agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        //UnitSelections.Instance.unitList.Add(gameObject);
    }

    public void GiveJob(Building job)
    {
        currentBuilding = job;
        isBuilding = true;
        Vector3 jobPosition = job.transform.position;
        Vector2 randomPosition = Random.insideUnitCircle.normalized * job.Radius;
        jobPosition.x += randomPosition.x;
        jobPosition.z += randomPosition.y;
        //agent.destination = jobPosition;
        //UnitSelections.Instance.RemoveSelection(gameObject);
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
