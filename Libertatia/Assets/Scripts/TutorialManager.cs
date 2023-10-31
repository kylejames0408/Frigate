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
        if(SceneManager.GetSceneByName("Outpost").isLoaded) // SceneManager.GetActiveScene().name == "PlayerData"
        {
            //check to see if this is the first time, or if its when they're coming back from combat
            if (!GameManager.outpostVisited)
            {
                outpostDialogues[0].TriggerDialogue();
                GameManager.outpostVisited = true;
                btnAttack.SetActive(false);
            }
            else
            {
                GameManager.data.isTutorial = false;
                outpostDialogues[5].TriggerDialogue();
                secondVisit = true;
                btnAttack.SetActive(true);
            }
        }
        else if (SceneManager.GetSceneByName("Combat").isLoaded)
        {
            combatDialogues[0].TriggerDialogue();
        }
        else
        {
            Debug.Log("Something went wrong");
        }
    }

    public void BuildingPlacedEvent(Component sender, object data)
    {
        if(sender is BuildingManager && !secondVisit)
            buildingsPlaced++;

        switch (buildingsPlaced)
        {
            case 1:
                outpostDialogues[1].TriggerDialogue();
                break;
            case 2:
                outpostDialogues[2].TriggerDialogue();
                break;
            default:
                return;
        }
    }

    public void CrewmateAssignedEvent(Component sender, object data)
    {
        crewmatesAssigned++;
        if(crewmatesAssigned == 2 && !secondVisit)
        {
            outpostDialogues[3].TriggerDialogue();
            btnAttack.SetActive(true);
        }
    }

    public void AllEnemiesDeadEvent(Component sender, object data)
    {
        combatDialogues[1].TriggerDialogue();
        GameObject.Find("Ship").GetComponent<Ship>().detectionRange = 30;
    }
}
