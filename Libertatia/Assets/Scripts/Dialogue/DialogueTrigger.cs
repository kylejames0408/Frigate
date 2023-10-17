using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;   // a field holding the dialogue used by the intended trigger (e.g. put this on a pirate that talks, write what he says in it)

    public TaskList tasks; // a field holding the tasks given to the player from the dialogue if one is given

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
        
        if(dialogue.hasTasks)
            FindObjectOfType<DialogueManager>().SetTaskList(tasks);
    }
}
