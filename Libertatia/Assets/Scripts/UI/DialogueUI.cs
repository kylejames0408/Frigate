using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public struct DialogueSegment
{
    public string speakerName;
    [TextArea(3, 10)] public string[] dialogueSentences;
    public string taskTitle;
    [TextArea(3, 10)] public string taskDescription;
    public UnityEvent taskStartedCallback;
    //public UnityEvent<int> taskEndedCallback;
}
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
    [SerializeField] private DialogueSegment currentDialogueSegment;
    [SerializeField] private int currentDialogueIndex;
    [SerializeField] private int currentTaskDescriptionIndex;

    private void Awake()
    {
        if (tmpSpeakerName == null) { tmpSpeakerName = transform.Find("Speaker Name").GetComponent<TextMeshProUGUI>(); }
        if (tmpDialogue == null) { tmpDialogue = transform.Find("Dialogue Text").GetComponent<TextMeshProUGUI>(); }
        if (btnContinue == null) { btnContinue = transform.Find("Dialogue Continue Button").GetComponent<Button>(); }
        if (tmpContinueButtonText == null) { tmpContinueButtonText = btnContinue.GetComponentInChildren<TextMeshProUGUI>(); }
        if (tmpTaskTitle == null) {tmpTaskTitle = transform.Find("Task Group Name").GetComponent<TextMeshProUGUI>();   }
        if (tmpTaskDescription == null) { tmpTaskDescription = transform.Find("Task List").GetComponent<TextMeshProUGUI>(); }

        btnContinue.onClick.AddListener(DisplayNextSentence);
    }

    internal void StartDialogueSegment(DialogueSegment dialogueSegment)
    {
        currentDialogueSegment = dialogueSegment;
        currentDialogueIndex = 0;

        OpenDialogueInterface();
        CloseTaskInterface();

        tmpSpeakerName.text = dialogueSegment.speakerName;
        if (currentDialogueSegment.dialogueSentences.Length > 0)
        {
            DisplayNextSentence();
        }
    }
    internal void DisplayNextSentence()
    {
        if(currentDialogueIndex < currentDialogueSegment.dialogueSentences.Length)
        {
            string dialogueSentence = currentDialogueSegment.dialogueSentences[currentDialogueIndex];
            if (tmpContinueButtonText.text == "Continue" || tmpContinueButtonText.text == "" || tmpContinueButtonText.text == "Close")
            {
                StartCoroutine(TypeSentence(dialogueSentence));
                tmpContinueButtonText.text = "Skip";
            }
            else if (tmpContinueButtonText.text == "Skip")
            {
                StopAllCoroutines();
                tmpDialogue.text = dialogueSentence;
                tmpContinueButtonText.text = "Continue";
                currentDialogueIndex++;
            }

            if (currentDialogueIndex == currentDialogueSegment.dialogueSentences.Length)
            {
                tmpContinueButtonText.text = "Close";
            }
        }
        else
        {
            EndDialogue();
        }
    }
    IEnumerator TypeSentence(string dialogueSentence)
    {
        // empties the dialogue box for the new sentence
        tmpDialogue.text = string.Empty;

        // turns the sentence into a char array and loops through to type each individual letter
        foreach (char letter in dialogueSentence.ToCharArray())
        {
            tmpDialogue.text += letter;

            yield return new WaitForSeconds(textWriteSpeed);
        }

        tmpContinueButtonText.text = "Continue";
        currentDialogueIndex++;
        yield return null;
    }
    private void EndDialogue()
    {
        CloseDialogueInterface();
        StopAllCoroutines();

        // clear the dialogue box text
        tmpSpeakerName.text = string.Empty;
        tmpDialogue.text = string.Empty;

        // Start Dialogue
        if (currentDialogueSegment.taskDescription.Length > 0)
        {
            tmpTaskTitle.text = currentDialogueSegment.taskTitle;
            tmpTaskDescription.text = currentDialogueSegment.taskDescription;
            OpenTaskInterface();
        }

        currentDialogueSegment.taskStartedCallback.Invoke();
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
