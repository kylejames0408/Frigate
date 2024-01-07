using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct Dialogue
{
    public string speakerName;
    [TextArea(3, 10)] public string[] sentences;
    public Tasks tasks;
    public UnityEvent dialogueEndCallback;
}
[System.Serializable]
public struct Tasks
{
    public string title;
    [TextArea(3, 10)] public string[] descriptions;
}

public class TutorialManager : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Editor")]
    [SerializeField] private bool skipTutorial = false;
#endif
    [Header("Data")]
    [SerializeField] private static TutorialProgress tutorialProgress;
    [Header("Dialogue")]
    [SerializeField] private List<Dialogue> outpostDialogues;
    [Header("References")]
    [SerializeField] private BuildingManager bm;
    [SerializeField] private Ship ship;
    [Header("UI")]
    [SerializeField] private BuildingUI buildingUI;
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private ShipUI shipUI;
    [SerializeField] private OutpostManagementUI outpostMUI;

    int shipCrewCount = 0;

    public static bool HasCompletedTutorial
    {
        get { return tutorialProgress.hasCompletedTutorial; }
    }
    private bool HasNotMadeFirstDeparture
    {
        get { return tutorialProgress.outpostVisitCount == 0; }
    }
    private bool IsFirstReturn
    {
        get { return tutorialProgress.outpostVisitCount == 1; }
    }

    private void Start()
    {
        tutorialProgress = PlayerDataManager.LoadProgress();
#if UNITY_EDITOR
        tutorialProgress.hasCompletedTutorial = tutorialProgress.hasCompletedTutorial || skipTutorial;
#endif
        if (!tutorialProgress.hasCompletedTutorial)
        {
            bm.onBuildingPlaced.AddListener(OnBuildingPlacedCallback); // dont want to be called more than once
            bm.onCrewmateAssigned.AddListener(OnCrewmateAssignedCallback);
            ship.onCrewmateAssigned.AddListener(OnCrewmateAssignedToBoatCallback);
            if (GameManager.Phase == GamePhase.BUILDING)
            {
                if (HasNotMadeFirstDeparture)
                {
                    dialogueUI.StartDialogue(outpostDialogues[0]);
                    shipUI.TutorialMode(true);
                    buildingUI.TutorialMode(true);
                }
                else if (IsFirstReturn)
                {
                    EndTutorial(); // maybe add all this crap
                    dialogueUI.StartDialogue(outpostDialogues[5]);
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
        tutorialProgress.hasCompletedTutorial = true;
    }
    internal static void LoadOutpost()
    {
        tutorialProgress.outpostVisitCount++;
    }

    // Callbacks
    internal void OnBuildingPlacedCallback(int buildingCardID, string buildingName)
    {
        if (!IsFirstReturn)
        {
            switch (++tutorialProgress.buildingsPlaced)
            {
                case 1:
                    Dialogue nextBuilding = new Dialogue();
                    nextBuilding.speakerName = outpostDialogues[0].speakerName;
                    nextBuilding.tasks.title = outpostDialogues[0].tasks.title;
                    nextBuilding.dialogueEndCallback = new UnityEvent();
                    if (buildingName == "Farm")
                    {
                        nextBuilding.sentences = new string[]
                        {
                            "Yarr! Now place the house"
                        };
                        nextBuilding.tasks.descriptions = new string[]
                        {
                            "- Build the house"
                        };
                    }
                    if (buildingName == "House")
                    {
                        nextBuilding.sentences = new string[]
                        {
                            "Yarr! Now place the farm"
                        };
                        nextBuilding.tasks.descriptions = new string[]
                        {
                            "- Build the farm"
                        };
                    }
                    outpostMUI.DisableBuildingCard(buildingCardID);
                    dialogueUI.StartDialogue(nextBuilding);
                    break;
                case 2:
                    outpostMUI.DisableBuildingCard(buildingCardID);
                    bm.onBuildingPlaced.RemoveListener(OnBuildingPlacedCallback);
                    dialogueUI.StartDialogue(outpostDialogues[1]);
                    break;
            }
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
            dialogueUI.StartDialogue(outpostDialogues[2]);
            bm.onCrewmateAssigned.RemoveListener(OnCrewmateAssignedCallback);
        }
    }
    internal void OnCrewmateAssignedToBoatCallback(int crewmateCardID)
    {
        outpostMUI.DisableDraggingCrewmateCard(crewmateCardID);
        if(++shipCrewCount == 4)
        {
            dialogueUI.StartDialogue(outpostDialogues[3]);
            ship.onCrewmateAssigned.RemoveListener(OnCrewmateAssignedToBoatCallback);
        }
    }
    internal void OnAllEnemiesEliminatedCallback()
    {
        //if (!tutorialProgress.hasCompletedTutorial)
        //{
        //    if (tutorialProgress.combatVisitCount == 1)
        //    {
        //        dialogueUI.StartDialogue(combatDialogues[1]);
        //    }
        //    GameObject.Find("Ship").GetComponent<Ship>().detectionRange = 30;
        //}
    }
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