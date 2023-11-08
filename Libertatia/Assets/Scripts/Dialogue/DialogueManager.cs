using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    //public Animator dialogueAnimator;           // the public reference to the dialogue box's animator (so it can animate on and off screen)
    //public Animator taskAnimator;               // the public referenc to the task list's animator (so it can animate on and off screen)

    public float textWriteSpeed;                // a public float so you can set how fast characters in a sentence populate the dialogue box
    public int openX = 1600;
    public int closedX = 2400;
    public float animTimeDialogue = 0.5f;

    public Transform dialogueTrans;
    private TextMeshProUGUI speakerNameText;    // the reference to the dialogue box name text (who the speaker is)
    private TextMeshProUGUI dialogueBoxText;    // the reference to the dialogue box text (what the speaker is saying)
    private TextMeshProUGUI continueButtonText; // the reference to the continue button (to swap between continue and skip)

    public Transform taskListTrans;
    private TextMeshProUGUI taskListName;       // the reference to the task list name text (a summary of the tasks that need to be done)
    private TextMeshProUGUI taskListText;       // the reference to the task list text (the individual tasks listed)

    private Queue<string> sentences;            // a Queue to hold all the sentences in the dialogue. It's like a list, but better for our use case

    private string currentSentence;             // a string used to hold the currently displayed sentence. This is for skipping character typing

    private Dialogue currentDialogue;           // a field used to store the currently active dialogue

    private void Awake()
    {
        sentences = new Queue<string>();
        currentSentence = string.Empty;
        currentDialogue = null;

        speakerNameText = GameObject.Find("Speaker Name").GetComponent<TextMeshProUGUI>();
        dialogueBoxText = GameObject.Find("Dialogue Text").GetComponent<TextMeshProUGUI>();
        continueButtonText = GameObject.Find("Dialogue Continue Button").GetComponentInChildren<TextMeshProUGUI>();

        taskListName = GameObject.Find("Task Group Name").GetComponent<TextMeshProUGUI>();
        taskListText = GameObject.Find("Task List").GetComponent<TextMeshProUGUI>();
    }

    // Starts the dialogue display. Brings up the box, queues up all the sentences for this dialogue, sets up the speaker name and displays the first sentence
    public void StartDialogue(Dialogue dialogue)
    {
        currentDialogue = null;
        speakerNameText = GameObject.Find("Speaker Name").GetComponent<TextMeshProUGUI>();
        speakerNameText = GameObject.Find("Speaker Name").GetComponent<TextMeshProUGUI>();
        dialogueBoxText = GameObject.Find("Dialogue Text").GetComponent<TextMeshProUGUI>();
        continueButtonText = GameObject.Find("Dialogue Continue Button").GetComponentInChildren<TextMeshProUGUI>();
        taskListName = GameObject.Find("Task Group Name").GetComponent<TextMeshProUGUI>();
        taskListText = GameObject.Find("Task List").GetComponent<TextMeshProUGUI>();

        OpenDialogue();
        CloseTaskList();

        currentDialogue = dialogue;

        speakerNameText.text = currentDialogue.speakerName;

        sentences.Clear();

        foreach(string sentence in currentDialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    // Call this method to display the next sentence in the Queue. This is done through the continue button which calls this method
    public void DisplayNextSentence()
    {
        // if there are no sentences left to display and the last sentence has finished typing, the dialogue will end
        if (sentences.Count == 0 && dialogueBoxText.text == currentSentence)
        {
            EndDialogue();
            return;
        }

        // if the last sentence has finished typing or the first sentence hasn't been displayed yet, display the next sentence
        if(dialogueBoxText.text == currentSentence || currentSentence == string.Empty)
        {
            currentSentence = sentences.Dequeue();
            continueButtonText.text = "Skip";
            StartCoroutine(TypeSentence(currentSentence));
        }
        // else [if] the current sentence hasn't finished typing, stop typing and autofill the current sentence in the dialogue box
        else
        {
            StopAllCoroutines();
            if (sentences.Count == 0)
                continueButtonText.text = "Close";
            else
                continueButtonText.text = "Continue";
            dialogueBoxText.text = currentSentence;
        }
    }

    // A coroutine for typing the sentence so the letters can populate it over time.
    IEnumerator TypeSentence(string sentence)
    {
        // empties the dialogue box for the new sentence
        dialogueBoxText.text = "";

        // turns the sentence into a char array and loops through to type each individual letter
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueBoxText.text += letter;

            yield return new WaitForSeconds(textWriteSpeed);    // use this to write a new letter each set time step
            //yield return null;                                // use this to write a new letter each frame
        }


        if (dialogueBoxText.text == sentence)
            if (sentences.Count == 0)
                continueButtonText.text = "Close";
            else
                continueButtonText.text = "Continue";
        yield return null;

    }

    // A simple method to end the dialogue event and close the dialogue box.
    void EndDialogue()
    {
        // trigger closing dialogue box animation
        CloseDialogue();

        StopAllCoroutines();

        // clear the dialogue box text
        speakerNameText.text = string.Empty;
        dialogueBoxText.text = string.Empty;

        // invoke the callback method of a given dialogue event if it has one
        if (currentDialogue.callback.GetPersistentEventCount() != 0)
            currentDialogue.callback.Invoke();
        else
            //Debug.Log("No callback here!");

        if (currentDialogue.hasTasks)
        {
            OpenTaskList();
        }

        currentDialogue = null;
        currentSentence = string.Empty;
    }

    // A method to set a task list after a dialogue event
    public void SetTaskList(TaskList taskList)
    {
        taskListName.text = taskList.taskListName;
        taskListText.text = "";
        foreach(string task in taskList.tasks)
        {
            taskListText.text += task + "\n";
        }
    }

    private void OpenTaskList()
    {
        taskListTrans.DOMoveX(openX, animTimeDialogue);
    }
    private void CloseTaskList()
    {
        taskListTrans.DOMoveX(closedX, animTimeDialogue);
    }
    private void OpenDialogue()
    {
        dialogueTrans.DOMoveX(openX, animTimeDialogue);
    }
    private void CloseDialogue()
    {
        dialogueTrans.DOMoveX(closedX, animTimeDialogue);
    }
}
