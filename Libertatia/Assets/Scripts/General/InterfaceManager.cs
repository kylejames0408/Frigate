using UnityEngine;

public class InterfaceManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject intResources;
    [SerializeField] private GameObject intBuilding;
    [SerializeField] private GameObject intCrewmate;
    [SerializeField] private GameObject intShip;
    [SerializeField] private GameObject intOutpost;
    [SerializeField] private DialogueUI intDialogue;
    [SerializeField] private GameObject intDev;
    [SerializeField] private PauseMenu intPause;
    [SerializeField] private GameObject intConfirmation;
    [SerializeField] private GameObject intIsland;

    private void Awake()
    {
        if (intDialogue == null) { intDialogue = FindObjectOfType<DialogueUI>(); }
        if (intPause == null) { intPause = FindObjectOfType<PauseMenu>(); }
    }

    internal void EndTutorial()
    {
        intDialogue.CloseDialogueInterface();
    }
    internal void PauseGame()
    {
        intPause.OpenInterface();
    }
    internal void UnpauseGame()
    {
        intPause.CloseInterface();
    }
}
