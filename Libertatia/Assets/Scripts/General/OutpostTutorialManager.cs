using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OutpostTutorialManager : MonoBehaviour
{
    public bool tutorial = true;
    public List<DialogueTrigger> outpostDialogues;
    private bool secondVisit = false;
    public GameObject departButton;
    public GameObject buildingUnassignButton;
    public GameObject buildingDemolishButton;
    public List<GameObject> shipUnassignButtons;
    public OutpostManagementUI oMUI;
    public ShipUI shipUI;
    [SerializeField] private Dictionary<int, CrewmateCard> crewmateCards;
    [SerializeField] private List<BuildingCard> buildingCards;

    [SerializeField] private BuildingManager bm;
    [SerializeField] private CrewmateManager cm;

    int buildingsPlaced = 0;
    int crewmatesAssignedToBuilding = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(tutorial)
        {
            if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
            if (bm == null) { bm = FindObjectOfType<BuildingManager>(); }
            if (oMUI == null) { oMUI = GameObject.Find("INT-Outpost").GetComponentInChildren<OutpostManagementUI>(); }
            if (shipUI == null) { shipUI = FindObjectOfType<ShipUI>(); }

            //check to see if this is the first time, or if its when they're coming back from combat
            if (GameManager.outpostVisitNumber == 0)
            {
                outpostDialogues[0].TriggerDialogue();
                GameManager.outpostVisitNumber++;

                departButton.SetActive(false);
                buildingUnassignButton.SetActive(false);
                buildingDemolishButton.SetActive(false);
                foreach (GameObject unassignButton in shipUnassignButtons)
                    unassignButton.SetActive(false);
            }
            else if (GameManager.outpostVisitNumber == 1)
            {
                GameManager.data.isTutorial = false;
                outpostDialogues[4].TriggerDialogue();
                secondVisit = true;
                GameManager.outpostVisitNumber++;
                departButton.SetActive(true);
            }
            else
            {
                departButton.SetActive(true);
            }
        }
    }

    public void HighlightBuildings()
    {
        buildingCards = oMUI.buildingCards;
        // Open the building tab
        oMUI.SelectTabCallback(0);
        // Highlight the buildings
        buildingCards[0].Highlight();
        buildingCards[1].Highlight();
    }

    public void HighlightCrewmates()
    {
        crewmateCards = oMUI.crewmateCards;
        // Open the crewmate tab
        oMUI.SelectTabCallback(1);
        // Highlight the crewmates
        foreach (KeyValuePair<int, CrewmateCard> kvp in crewmateCards)
        {
            kvp.Value.Highlight();
        }
    }

    public void OpenShipUI()
    {
        shipUI.OpenMenu();
    }

    public void PanCameraToBoat()
    {
        
        // Lerp camera transform to the boat x = -22, z = -160
        Vector3 shipCamPos = new Vector3(-22, Camera.main.transform.position.y, -160);
        CameraManager.Instance.PanTo(shipCamPos);
    }

    public void BuildingPlacedEvent(Component sender, object data)
    {
        if (sender is BuildingManager && data is Building && !secondVisit)
        {
            Building recievedBuilding = (Building)data;
            buildingsPlaced++;

            if(buildingsPlaced == 2)
                outpostDialogues[1].TriggerDialogue();
        }
    }

    public void CrewmateAssignedBuildingEvent(Component sender, object data)
    {
        Debug.Log("Shit yourself from the start");
        // Make sure both buildings have been placed
        if (bm.Buildings.Length < 2)
            return;

        Debug.Log("Building Length is fine");
        // Check if all buildings have an assigned crewmate. If not, end the method early
        for (int i = 0; i < bm.Buildings.Length; i++)
        {
            if (bm.Buildings[0].CanAssign())
                return;
        }

        Debug.Log("You can't assign to a single building");
        // Check if this is the first visit. If so, trigger the next dialogue
        if (!secondVisit)
            outpostDialogues[2].TriggerDialogue();
    }

    public void CrewmateAssignedBoatEvent(Component sender, object data)
    {
        // Make sure both buildings have been placed, otherwise return
        if (bm.Buildings.Length < 2)
            return;

        // Check if all buildings have an assigned crewmate. If not, end the method early
        for (int i = 0; i < bm.Buildings.Length; i++)
        {
            if (bm.Buildings[i].CanAssign())
                return;
        }

        // Check if all remaining crewmates have been assigned (the only thing they can be assigned to is the boat), if not return
        for (int i = 0; i < cm.Crewmates.Count; i++)
        {
            Debug.Log(cm.Crewmates[i].FullName + " can be assigned? " + cm.Crewmates[i].IsAssigned);
            if (!cm.Crewmates[i].IsAssigned)
                return;
        }

        if (!secondVisit)
        {
            outpostDialogues[3].TriggerDialogue();
            departButton.SetActive(true);
        }
    }
}
