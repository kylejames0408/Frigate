using System;
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
    public int buildingID = -1;
    public int cardIndex = -1;
    [SerializeField] private bool isBuilding = false;
    private NavMeshAgent agent;
    public UnityEvent onAssign;
    public UnityEvent onSelect;

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
        HandleSelection();
    }

    public void GiveJob(Building job)
    {
        buildingID = job.id;
        isBuilding = true;
        Vector3 jobPosition = job.transform.position;
        Vector2 randomPosition = UnityEngine.Random.insideUnitCircle.normalized * job.Radius;
        randomPosition.y = -(Mathf.Abs(randomPosition.y)); // keeps builders in-front of the building
        jobPosition.x += randomPosition.x;
        jobPosition.z += randomPosition.y;
        agent.destination = jobPosition;
        CrewmateManager.Instance.RemoveSelection(gameObject);
        onAssign.Invoke();
    }

    public void Free()
    {
        buildingID = -1;
        isBuilding = false;
    }

    private void HandleSelection()
    {
        if (isHovered && Input.GetMouseButtonDown(0))
        {
            onSelect.Invoke();
        }
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
