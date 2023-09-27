using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    private static TutorialManager instance;

    public DialogueTrigger firstDialogue;
    public DialogueTrigger secondDialogue;
    public DialogueTrigger thirdDialogue;
    public DialogueTrigger fourthDialogue;
    public DialogueTrigger fifthDialogue;

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
        firstDialogue.TriggerDialogue();
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
                secondDialogue.TriggerDialogue();
                break;
            case 2:
                thirdDialogue.TriggerDialogue();
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
            fourthDialogue.TriggerDialogue();
        }
    }
}
