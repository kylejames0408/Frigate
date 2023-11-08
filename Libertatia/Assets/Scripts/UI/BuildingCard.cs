using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BuildingCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent onHover;
    public UnityEvent onHoverExit;
    public BuildingResources resourceCost;
    public BuildingResources resourceProduction;

    public void Init(BuildingResources buildingCost, BuildingResources buildingProduction)
    {
        resourceCost = buildingCost;
        resourceProduction = buildingProduction;
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