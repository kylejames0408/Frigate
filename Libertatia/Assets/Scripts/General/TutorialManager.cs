using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public List<DialogueTrigger> outpostDialogues;
    public List<DialogueTrigger> combatDialogues;
    public GameObject btnAttack;
    private bool secondVisit = false;

    int buildingsPlaced = 0;
    int crewmatesAssigned = 0;

    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetSceneByName("Outpost").isLoaded || SceneManager.GetSceneByName("Outpost-Testing").isLoaded) // SceneManager.GetActiveScene().name == "PlayerData"
        {
            //check to see if this is the first time, or if its when they're coming back from combat
            if (GameManager.outpostVisitNumber == 0)
            {
                outpostDialogues[0].TriggerDialogue();
                GameManager.outpostVisitNumber++;
                btnAttack.SetActive(false);
            }
            else if(GameManager.outpostVisitNumber == 1)
            {
                GameManager.data.isTutorial = false;
                outpostDialogues[5].TriggerDialogue();
                secondVisit = true;
                GameManager.outpostVisitNumber++;
                btnAttack.SetActive(true);
            }
            else
            {
                btnAttack.SetActive(true);
            }
        }
        else if (SceneManager.GetSceneByName("Combat").isLoaded || SceneManager.GetSceneByName("Combat-Testing").isLoaded)
        {
            if(GameManager.combatVisitNumber == 0)
            {
                combatDialogues[0].TriggerDialogue();
                GameManager.combatVisitNumber++;
            }    
        }
        else
        {
            Debug.Log("Something went wrong");
        }
    }

    public void BuildingPlacedEvent(Component sender, object data)
    {
        if(sender is BuildingManager && data is Building && !secondVisit)
        {
            Building recievedBuilding = (Building)data;
            buildingsPlaced++;

            switch (buildingsPlaced)
            {
                case 1:
                    if (recievedBuilding.Name == "Farm")
                    {
                        outpostDialogues[1].dialogue.sentences[0] = "Yarr! Now place the house";
                        outpostDialogues[1].tasks.tasks[0] = "- Build the house";
                    } 
                    if(recievedBuilding.Name == "House")
                    {
                        outpostDialogues[1].dialogue.sentences[0] = "Yarr! Now place the farm";
                        outpostDialogues[1].tasks.tasks[0] = "- Build the farm";
                    }
                    outpostDialogues[1].TriggerDialogue();
                    break;
                case 2:
                    outpostDialogues[2].TriggerDialogue();
                    break;
                default:
                    return;
            }
        }
            


        
    }

    public void CrewmateAssignedEvent(Component sender, object data)
    {
        crewmatesAssigned++;
        if(crewmatesAssigned == 1 && !secondVisit)
        {
            //Hide outpost drag card text
            OutpostManagementUI oMUI = GameObject.Find("INT-Outpost").GetComponentInChildren<OutpostManagementUI>();
            oMUI.HideCrewmateCardArrow();
        }
        if(crewmatesAssigned == 2 && !secondVisit)
        {
            outpostDialogues[3].TriggerDialogue();
            btnAttack.SetActive(true);
        }
    }

    public void AllEnemiesDeadEvent(Component sender, object data)
    {
        if (GameManager.combatVisitNumber == 1)
            combatDialogues[1].TriggerDialogue();
        GameObject.Find("Ship").GetComponent<Ship>().detectionRange = 30;
    }
}
