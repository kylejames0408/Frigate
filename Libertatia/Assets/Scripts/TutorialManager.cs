using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{

    private static TutorialManager instance;

    public List<DialogueTrigger> outpostDialogues;

    public List<DialogueTrigger> combatDialogues;

    enum Scene
    {
        Oupost,
        Combat
    }

    Scene currentScene;

    int buildingsPlaced = 0;
    int crewmatesAssigned = 0;

    bool firstFlag = false;
    bool secondFlag = false;

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
        if(SceneManager.GetActiveScene().name == "Outpost - Playtest")
        {
            //check to see if this is the first time, or if its when they're coming back from combat
            currentScene = Scene.Oupost;
            outpostDialogues[0].TriggerDialogue();
        }
        else if (SceneManager.GetActiveScene().name == "CombatTest - Playtest")
        {
            currentScene = Scene.Combat;
            combatDialogues[0].TriggerDialogue();
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildingPlacedEvent(Component sender, object data)
    {
        if(sender is BuildingManager)
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
        if(crewmatesAssigned == 2)
        {
            outpostDialogues[3].TriggerDialogue();
        }
    }

    public void AllEnemiesDeadEvent(Component sender, object data)
    {
        combatDialogues[1].TriggerDialogue();
        GameObject.Find("Ship").GetComponent<Ship>().detectionRange = 60;
    }
}
