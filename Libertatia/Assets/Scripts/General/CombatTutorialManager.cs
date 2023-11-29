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
    [SerializeField] private ZoneManager zm;

    int zonesClaimed = 0;

    bool zonesTurnedOff = false;

    // Start is called before the first frame update
    void Start()
    {
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
        if (zm == null) { zm = FindObjectOfType<ZoneManager>(); }
        if (cMUI == null) { cMUI = GameObject.Find("INT-Combat").GetComponentInChildren<CombatManagementUI>(); }

        if (GameManager.combatVisitNumber == 0)
        {
            combatDialogues[0].TriggerDialogue();
            GameManager.combatVisitNumber++;
            leaveIslandButton.SetActive(false);

            //Set all zones to uninteractable except the first 2
            
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!zonesTurnedOff && zm.zones.Count > 0 && GameManager.combatVisitNumber == 1)
        {
            for (int i = 0; i < zm.zones.Count; i++)
            {
                zm.zones[i].GetComponent<Zone>().isClickable = false;
            }
            zm.zones[4].GetComponent<Zone>().isClickable = true;
            zonesTurnedOff = true;
        }
    }

    public void ActivateFirstZone()
    {
        zm.zones[2].GetComponent<Zone>().isClickable = true;
    }

    public void ZoneClaimedEvent(Component sender, object data)
    {
        if (GameManager.combatVisitNumber != 1)
        {
            Debug.Log("Returned");
            return;
        }

        zonesClaimed++;

        switch (zonesClaimed)
        {
            case 1:
                // Trigger next dialogue [1]
                combatDialogues[1].TriggerDialogue();
                // Activate next zone
                zm.zones[3].GetComponent<Zone>().isClickable = true;
                break;
            case 2:
                // Trigger next dialogue [2]
                combatDialogues[2].TriggerDialogue();
                // Activate all zones
                for (int i = 0; i < zm.zones.Count; i++)
                {
                    zm.zones[i].GetComponent<Zone>().isClickable = true;
                }
                break;
        }
    }
}
