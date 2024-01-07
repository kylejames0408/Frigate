using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueUI : MonoBehaviour
{
    [Header("Dials")]
    [SerializeField] private float textWriteSpeed;
    [SerializeField] private float interfacePadding = 20.0f;
    [SerializeField] private float animTimeDialogue = 0.5f;
    [Header("Dialogue UI")]
    [SerializeField] private RectTransform dialogueCardRectTransform;
    [SerializeField] private TextMeshProUGUI tmpSpeakerName;
    [SerializeField] private TextMeshProUGUI tmpDialogue;
    [SerializeField] private TextMeshProUGUI tmpContinueButtonText;
    [SerializeField] private Button btnContinue;
    [Header("Task UI")]
    [SerializeField] private RectTransform taskCardRectTransform;
    [SerializeField] private TextMeshProUGUI tmpTaskTitle;
    [SerializeField] private TextMeshProUGUI tmpTaskDescription;
    // Tracking
    private Queue<string> sentences;
    private string currentSentence;
    private int currentTaskCount;
    // Events
    private UnityEvent endOfDialogueCallback;

    private void Awake()
    {
        sentences = new Queue<string>();
        currentSentence = string.Empty;

        if (tmpSpeakerName == null) { tmpSpeakerName = transform.Find("Speaker Name").GetComponent<TextMeshProUGUI>(); }
        if (tmpDialogue == null) { tmpDialogue = transform.Find("Dialogue Text").GetComponent<TextMeshProUGUI>(); }
        if (btnContinue == null) { btnContinue = transform.Find("Dialogue Continue Button").GetComponent<Button>(); }
        if (tmpContinueButtonText == null) { tmpContinueButtonText = btnContinue.GetComponentInChildren<TextMeshProUGUI>(); }
        if (tmpTaskTitle == null) {tmpTaskTitle = transform.Find("Task Group Name").GetComponent<TextMeshProUGUI>();   }
        if (tmpTaskDescription == null) { tmpTaskDescription = transform.Find("Task List").GetComponent<TextMeshProUGUI>(); }

        btnContinue.onClick.AddListener(DisplayNextSentence);
    }

    // Starts the dialogue display. Brings up the box, queues up all the sentences for this dialogue, sets up the speaker name and displays the first sentence
    internal void StartDialogue(Dialogue dialogue)
    {
        OpenDialogueInterface();
        CloseTaskInterface();

        tmpSpeakerName.text = dialogue.speakerName;
        endOfDialogueCallback = dialogue.dialogueEndCallback;

        sentences.Clear();
        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();

        currentTaskCount = dialogue.tasks.descriptions.Length;
        if (dialogue.tasks.title.Length > 0)
        {
            tmpTaskTitle.text = dialogue.tasks.title;
            tmpTaskDescription.text = "";
            foreach (string description in dialogue.tasks.descriptions)
            {
                tmpTaskDescription.text += description + "\n";
            }
        }
    }

    // Call this method to display the next sentence in the Queue. This is done through the continue button which calls this method
    internal void DisplayNextSentence()
    {
        // if there are no sentences left to display and the last sentence has finished typing, the dialogue will end
        if (sentences.Count == 0 && tmpDialogue.text == currentSentence)
        {
            EndDialogue();
            return;
        }

        // if the last sentence has finished typing or the first sentence hasn't been displayed yet, display the next sentence
        if (tmpDialogue.text == currentSentence || currentSentence == string.Empty)
        {
            currentSentence = sentences.Dequeue();
            tmpContinueButtonText.text = "Skip";
            StartCoroutine(TypeSentence(currentSentence));
        }
        // else [if] the current sentence hasn't finished typing, stop typing and autofill the current sentence in the dialogue box
        else
        {
            StopAllCoroutines();
            if (sentences.Count == 0)
            {
                tmpContinueButtonText.text = "Close";
            }
            else
            {
                tmpContinueButtonText.text = "Continue";
            }
            tmpDialogue.text = currentSentence;
        }
    }

    // A coroutine for typing the sentence so the letters can populate it over time.
    IEnumerator TypeSentence(string sentence)
    {
        // empties the dialogue box for the new sentence
        tmpDialogue.text = "";

        // turns the sentence into a char array and loops through to type each individual letter
        foreach (char letter in sentence.ToCharArray())
        {
            tmpDialogue.text += letter;

            yield return new WaitForSeconds(textWriteSpeed);
        }


        if (tmpDialogue.text == sentence)
        {
            if (sentences.Count == 0)
            {
                tmpContinueButtonText.text = "Close";
            }
            else
            {
                tmpContinueButtonText.text = "Continue";
            }
        }

        yield return null;

    }

    // A simple method to end the dialogue event and close the dialogue box.
    void EndDialogue()
    {
        CloseDialogueInterface(); // trigger closing dialogue box animation
        StopAllCoroutines();

        // clear the dialogue box text
        tmpSpeakerName.text = string.Empty;
        tmpDialogue.text = string.Empty;

        if (currentTaskCount > 0)
        {
            OpenTaskInterface();
        }

        if(endOfDialogueCallback.GetPersistentEventCount() > 0)
        {
            endOfDialogueCallback.Invoke();
        }

        currentSentence = string.Empty;
    }

    internal void OpenDialogueInterface()
    {
        float menuWidth = dialogueCardRectTransform.rect.width;
        dialogueCardRectTransform.DOMoveX(Screen.width - menuWidth - interfacePadding, animTimeDialogue);
    }
    internal void CloseDialogueInterface()
    {
        dialogueCardRectTransform.DOMoveX(Screen.width, animTimeDialogue);
    }
    private void OpenTaskInterface()
    {
        float menuWidth = taskCardRectTransform.rect.width;
        taskCardRectTransform.DOMoveX(Screen.width - menuWidth - interfacePadding, animTimeDialogue);
    }
    private void CloseTaskInterface()
    {
        taskCardRectTransform.DOMoveX(Screen.width, animTimeDialogue);
    }
}
