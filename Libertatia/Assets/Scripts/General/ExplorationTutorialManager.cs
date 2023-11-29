using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExplorationTutorialManager : MonoBehaviour
{
    public List<DialogueTrigger> explorationDialogues;
    private bool secondVisit = false;

    // Start is called before the first frame update
    void Start()
    {
        ////check to see if this is the first time, or if its when they're coming back from combat
        //if (GameManager.explorationVisitNumber == 0)
        //{
        //    explorationDialogues[0].TriggerDialogue();
        //    GameManager.outpostVisitNumber++;
        //    // Disable depart button in ship UI
        //}
        //else if (GameManager.explorationVisitNumber == 1)
        //{
        //    explorationDialogues[1].TriggerDialogue();
        //    secondVisit = true;
        //    GameManager.explorationVisitNumber++;
        //}
    }
}
