using UnityEngine;
using UnityEngine.UI;
using EasyDragAndDrop.Core;
using UnityEngine.EventSystems;

public class AssignSlot : DropSlot
{
    public PirateInfo pirateInfo;   //The info itself the slot is recieving
    private Button originalPirate;
    public bool isOverrideOnDrop = true;
    private bool isAssigned = false;
    [SerializeField]
    private GameObject unnasignButton;

    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        if (!isOverrideOnDrop) return;
        var model = (PirateInfo)dropObj2D.m_ObjInfoComponent;
        pirateInfo.Initialize(model);
        isAssigned = true;
        unnasignButton.SetActive(true);

        originalPirate = dropObj2D.gameObject.GetComponent<Button>();
        originalPirate.interactable = false;
    }

    public void ClearAssignment()
    {
        pirateInfo.Clear();
        isAssigned = false;
        unnasignButton.SetActive(false);

        originalPirate.interactable = true;
        originalPirate = null;
    }
}


