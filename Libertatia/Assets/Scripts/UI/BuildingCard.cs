using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private int index = -1;
    private Outline outline;
    public UnityEvent<int> onHover;
    public UnityEvent onHoverExit;
    public BuildingResources resourceCost;
    public BuildingResources resourceProduction;

    public int Index
    {
        get { return index; }
    }

    private void Awake()
    {
        outline = GetComponent<Outline>();
        Deselect();
    }

    public void Set(int index, BuildingResources buildingCost, BuildingResources buildingProduction)
    {
        this.index = index;
        resourceCost = buildingCost;
        resourceProduction = buildingProduction;
    }
    public void Select()
    {
        outline.effectColor = new Color(255, 187, 00);
        outline.enabled = true;
    }
    public void Deselect()
    {
        outline.enabled = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        onHover.Invoke(index);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        onHoverExit.Invoke();
    }

    public void Highlight()
    {
        outline.effectColor = new Color(0, 217, 255);
        outline.enabled = true;
    }
}