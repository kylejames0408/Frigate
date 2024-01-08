using UnityEngine;

[CreateAssetMenu(fileName = "TutorialDialogue", menuName = "TutorialDialogue")]
public class TutorialSegment : ScriptableObject
{
    [SerializeField] private DialogueSegment[] dialogueSegments;

    public DialogueSegment[] DialogueSegments
    {
        get { return dialogueSegments; }
    }
}
