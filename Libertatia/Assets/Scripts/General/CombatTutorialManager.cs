using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatTutorialManager : MonoBehaviour
{
    public List<DialogueTrigger> combatDialogues;
    private bool secondVisit = false;

    public GameObject leaveIslandButton;
    public CombatManagementUI cMUI;
    //public ShipUI shipUI;
    [SerializeField] private Dictionary<int, CrewmateCard> crewmateCards;

    [SerializeField] private CrewmateManager cm;

    int zonesClaimed = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
        if (cMUI == null) { cMUI = GameObject.Find("INT-Combat").GetComponentInChildren<CombatManagementUI>(); }

        if (GameManager.combatVisitNumber == 0)
        {
            combatDialogues[0].TriggerDialogue();
            GameManager.combatVisitNumber++;
            leaveIslandButton.SetActive(false);

            //Set all zones to uninteractable except the first
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ZoneClaimedEvent()
    {
        zonesClaimed++;
        switch (zonesClaimed)
        {
            case 1:
                // Trigger next dialogue [1]
                combatDialogues[1].TriggerDialogue();
                // Activate next zone
                break;
            case 2:
                // Trigger next dialogue [2]
                combatDialogues[2].TriggerDialogue();
                // Activate all zones
                break;
        }
    }
}
