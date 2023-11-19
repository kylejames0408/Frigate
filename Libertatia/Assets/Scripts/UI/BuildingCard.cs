using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Outline outline;
    public UnityEvent onHover;
    public UnityEvent onHoverExit;
    public BuildingResources resourceCost;
    public BuildingResources resourceProduction;

    public void Init(BuildingResources buildingCost, BuildingResources buildingProduction)
    {
        resourceCost = buildingCost;
        resourceProduction = buildingProduction;
        outline = GetComponent<Outline>();
        Deselect();
    }
    public void Select()
    {
        outline.enabled = true;
    }
    public void Deselect()
    {
        outline.enabled = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        onHover.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onHoverExit.Invoke();
    }
}