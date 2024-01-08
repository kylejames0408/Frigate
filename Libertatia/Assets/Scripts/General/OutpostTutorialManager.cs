using UnityEngine;
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

public class OutpostTutorialManager : MonoBehaviour
{
    private static OutpostTutorialManager instance;
#if UNITY_EDITOR
    [Header("Editor")]
    [SerializeField] private bool skipTutorial = false;
#endif
    [Header("Data")]
    [SerializeField] private Progress tutorialProgress;
    //[SerializeField] private TutorialSegment startTS;
    //[SerializeField] private TutorialSegment endTS;
    [SerializeField] private DialogueSegment[] startDS;
    [SerializeField] private DialogueSegment[] endDS;
    [Header("References")]
    [SerializeField] private BuildingManager bm;
    [SerializeField] private Ship ship;
    [Header("UI")]
    [SerializeField] private BuildingUI buildingUI;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private ShipUI shipUI;
    [SerializeField] private OutpostManagementUI outpostMUI;
    // misc temp variables
    private int shipCrewCount = 0;
    private int buildingCount = 0;

    public static bool HasCompletedTutorial
    {
        get { return instance.tutorialProgress.hasCompletedTutorial; }
    }

    private void Start()
    {
        if(instance == null) { instance = this; }
        tutorialProgress = PlayerDataManager.LoadProgress();
#if UNITY_EDITOR
        tutorialProgress.hasCompletedTutorial = tutorialProgress.hasCompletedTutorial || skipTutorial;
#endif
        if (!tutorialProgress.hasCompletedTutorial)
        {
            bm.onBuildingPlaced.AddListener(OnBuildingPlacedCallback);
            bm.onCrewmateAssigned.AddListener(OnCrewmateAssignedCallback);
            ship.onCrewmateAssigned.AddListener(OnCrewmateAssignedToBoatCallback);
            if (GameManager.Phase == GamePhase.BUILDING)
            {
                if (tutorialProgress.outpostVisitCount == 0)
                {
                    dialogueUI.StartDialogueSegment(startDS[0]);
                    shipUI.TutorialMode(true);
                    buildingUI.TutorialMode(true);
                }
                else if (tutorialProgress.outpostVisitCount == 1)
                {
                    EndTutorial(); // maybe add all this crap
                    dialogueUI.StartDialogueSegment(endDS[0]);
                    shipUI.TutorialMode(false);
                    buildingUI.TutorialMode(false);
                }
            }
        }
    }
    private void OnDestroy()
    {
        PlayerDataManager.SaveTutorialProgress(tutorialProgress);
    }

    internal static void EndTutorial()
    {
        instance.tutorialProgress.hasCompletedTutorial = true;
    }
    internal static void LoadOutpost()
    {
        instance.tutorialProgress.outpostVisitCount++;
    }

    // Callbacks
    internal void OnBuildingPlacedCallback(int buildingCardID, string buildingName)
    {
        switch (++buildingCount)
        {
            case 1:
                DialogueSegment nextBuildingDS = new DialogueSegment();
                nextBuildingDS.speakerName = startDS[0].speakerName;
                nextBuildingDS.taskTitle = startDS[0].taskTitle;
                nextBuildingDS.taskStartedCallback = new UnityEvent();
                if (buildingName == "Farm")
                {
                    nextBuildingDS.dialogueSentences = new string[]
                    {
                            "Yarr! Now place the house"
                    };
                    nextBuildingDS.taskDescription = "- Build the house";
                }
                else if (buildingName == "House")
                {
                    nextBuildingDS.dialogueSentences = new string[]
                    {
                            "Yarr! Now place the farm"
                    };
                    nextBuildingDS.taskDescription = "- Build the farm";
                }
                outpostMUI.DisableBuildingCard(buildingCardID);
                dialogueUI.StartDialogueSegment(nextBuildingDS);
                break;
            case 2:
                outpostMUI.DisableBuildingCard(buildingCardID);
                bm.onBuildingPlaced.RemoveListener(OnBuildingPlacedCallback);
                dialogueUI.StartDialogueSegment(startDS[1]);
                break;
        }
    }
    internal void OnCrewmateAssignedCallback(int crewmateCardID)
    {
        outpostMUI.DisableDraggingCrewmateCard(crewmateCardID);
        bool allHousesFilled = true;
        for (int i = 0; i < bm.Buildings.Length; i++)
        {
            if (bm.Buildings[i].CanAssign())
            {
                allHousesFilled = false;
            }
        }

        if (allHousesFilled)
        {
            dialogueUI.StartDialogueSegment(startDS[2]);
            bm.onCrewmateAssigned.RemoveListener(OnCrewmateAssignedCallback);
        }
    }
    internal void OnCrewmateAssignedToBoatCallback(int crewmateCardID)
    {
        outpostMUI.DisableDraggingCrewmateCard(crewmateCardID);
        if(++shipCrewCount == 4)
        {
            dialogueUI.StartDialogueSegment(startDS[3]);
            ship.onCrewmateAssigned.RemoveListener(OnCrewmateAssignedToBoatCallback);
        }
    }
    //internal void OnAllEnemiesEliminatedCallback()
    //{
    //    //if (!tutorialProgress.hasCompletedTutorial)
    //    //{
    //    //    if (tutorialProgress.combatVisitCount == 1)
    //    //    {
    //    //        dialogueUI.StartDialogue(combatDialogues[1]);
    //    //    }
    //    //    GameObject.Find("Ship").GetComponent<Ship>().detectionRange = 30;
    //    //}
    //}
    // Public Callbacks - available in insspector
    public void HighlightBuildingCardsCallback()
    {
        outpostMUI.HighlightBuildingCards();
    }
    public void HighlightCrewmateCardsCallback()
    {
        outpostMUI.HighlightCrewmateCards();
    }
    public void OpenShipUICallback()
    {
        shipUI.OpenInterface();
    }
    public void PanCameraToBoatCallback()
    {
        // Lerp camera transform to the boat x = -22, z = -160
        Vector3 shipCamPos = new Vector3(-22, Camera.main.transform.position.y, -160);
        CameraManager.Instance.PanTo(shipCamPos);
    }
}