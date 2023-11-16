using System;
using UnityEngine;
using UnityEngine.UI;

public class AssigneeCard : MonoBehaviour
{
    [SerializeField] private int crewmateID = -1;
    [SerializeField] private Image icon;
    public Button btnUnassign;

    public ObjectData CrewmateData
    {
        get
        {
            return new ObjectData(crewmateID, icon.sprite);
        }
    }

    private void Start()
    {
        btnUnassign.interactable = false;
    }
    internal void Set(ObjectData crewmate)
    {
        crewmateID = crewmate.id;
        icon.sprite = crewmate.icon;
        btnUnassign.onClick.RemoveAllListeners();
        btnUnassign.interactable = true;
    }
    internal bool IsEmpty()
    {
        return crewmateID == -1;
    }
    internal void ResetCard(Sprite icon)
    {
        crewmateID = -1;
        this.icon.sprite = icon;
        btnUnassign.interactable = false;
    }
    internal void Enable()
    {
        gameObject.SetActive(true);
    }
    internal void Disable()
    {
        gameObject.SetActive(false);
    }
}
