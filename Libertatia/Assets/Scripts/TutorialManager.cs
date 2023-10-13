using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{

    private static TutorialManager instance;

    public List<DialogueTrigger> outpostDialogues;
    public List<string> outpostTaskLists;

    public List<DialogueTrigger> combatDialogues;
    public List<string> combatTaskLists;

    private static GameObject AttackButton;

    private bool secondVisit = false;

    enum Scene
    {
        Oupost,
        Combat
    }

    Scene currentScene;

    int buildingsPlaced = 0;
    int crewmatesAssigned = 0;

    public static TutorialManager Instance
    {
        get
        {
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetSceneByName("Outpost").isLoaded) // SceneManager.GetActiveScene().name == "PlayerData"
        {
            //check to see if this is the first time, or if its when they're coming back from combat
            currentScene = Scene.Oupost;
            if (!GameManager.Instance.outpostVisited)
            {
                outpostDialogues[0].TriggerDialogue();
                GameManager.Instance.outpostVisited = true;
            }
            else
            {
                outpostDialogues[5].TriggerDialogue();
                secondVisit = true;
            }


            AttackButton = GameObject.Find("BTN-Attack");
            if(AttackButton != null )
            {
                AttackButton.SetActive(false);
            }
        }
        else if (SceneManager.GetSceneByName("CombatTest").isLoaded)
        {
            currentScene = Scene.Combat;
            combatDialogues[0].TriggerDialogue();
        }
        else
        {
            //Debug.Log("Something went wrong");
        }
    }

    // Update is called once per frame
    void Update()
    {

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
            AttackButton.SetActive(true);
        }
    }

    public void AllEnemiesDeadEvent(Component sender, object data)
    {
        combatDialogues[1].TriggerDialogue();
        GameObject.Find("Ship").GetComponent<Ship>().detectionRange = 50;
    }
}
