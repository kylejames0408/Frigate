using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExplorationTutorialManager : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Editor")]
    [SerializeField] private bool skipTutorial = false;
#endif
    [Header("Data")]
    private static Progress tutorialProgress; // can't see progress in inspector if static
    [SerializeField] private DialogueSegment[] dialogueSegments;
    [Header("References")]
    [SerializeField] private IslandManager im;
    [Header("UI")]
    [SerializeField] private DialogueUI dialogueUI;
    [SerializeField] private ShipUI shipUI;
    [SerializeField] private IslandUI islandUI;

    public static bool HasCompletedTutorial
    {
        get { return tutorialProgress.hasCompletedTutorial; }
    }

    private void Awake()
    {
        if(dialogueUI == null) { dialogueUI = FindAnyObjectByType<DialogueUI>(); }
        if(shipUI == null) { shipUI = FindAnyObjectByType<ShipUI>(); }
        if(islandUI == null) { islandUI = FindAnyObjectByType<IslandUI>(); }
    }

    void Start()
    {
        tutorialProgress = PlayerDataManager.LoadProgress();
#if UNITY_EDITOR
        tutorialProgress.hasCompletedTutorial = tutorialProgress.hasCompletedTutorial || skipTutorial;
#endif
        if (!tutorialProgress.hasCompletedTutorial)
        {
            if (tutorialProgress.explorationVisitCount == 0)
            {
                dialogueUI.StartDialogueSegment(dialogueSegments[0]);
                tutorialProgress.explorationVisitCount++;
                im.onIslandSelected.AddListener(OnOutpostIslandSelectedCallback1);
            }
            else if (tutorialProgress.explorationVisitCount == 1)
            {
                dialogueUI.StartDialogueSegment(dialogueSegments[1]);
                tutorialProgress.explorationVisitCount++;
                im.onIslandSelected.AddListener(OnEnemyIslandSelectedCallback);
            }
        }
    }
    private void OnDestroy()
    {
        PlayerDataManager.SaveTutorialProgress(tutorialProgress);
    }

    private void OnOutpostIslandSelectedCallback1(IslandType islandType)
    {
        if(islandType == IslandType.OUTPOST)
        {
            islandUI.DisableDepartButton();
        }
        else
        {
            islandUI.EnableDepartButton();
        }
    }
    private void OnEnemyIslandSelectedCallback(IslandType islandType)
    {
        if (islandType == IslandType.ENEMY)
        {
            islandUI.DisableDepartButton();
        }
        else
        {
            islandUI.EnableDepartButton();
        }
    }
}
