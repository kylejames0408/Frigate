using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.CanvasScaler;

public class Enemy : Character
{
    public List<Character> crewMembers;
    public List<GameObject> crewMemberGameObjects;
    public CrewmateManager cm;

    public float detectionRange;

    //public GameObject unitSelection;
    //public UnitSelections unitSelectionList;

    public GameObject combatUI;
    public GameObject resourceText;

    public bool lootDropped;
    public int lootValue;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 50;
        currentHealth = 50;
        attackRange = 3;
        attackRate = 4;
        damage = 10;
        detectionRange = 10;

        lootValue = 5;

        charAgent = GetComponent<NavMeshAgent>();

        //Finds all crew members in the scene and adds them into the gameobject list
        crewMemberGameObjects.AddRange(GameObject.FindGameObjectsWithTag("PlayerCharacter"));

        //take the gameobject crew members and turn them into character objects
        for(int i = 0; i < crewMemberGameObjects.Count; i++)
        {
            Character crewMember = crewMemberGameObjects[i].GetComponent<Character>();
            crewMembers.Add(crewMember);
        }

        //unitSelection = GameObject.FindGameObjectWithTag("Unit Selections");
        //unitSelectionList = unitSelection.GetComponent<UnitSelections>();

        combatUI = GameObject.FindGameObjectWithTag("CombatUI");
        lootDropped = false;
    }

    // Update is called once per frame
    void Update()
    {
        Attack(crewMembers);

        healthbar.UpdateHealthBar(maxHealth, currentHealth);
        Death();

        if(gameObject.activeSelf != false)
        {
            //DetectCrewMember();
        }

    }

    /// <summary>
    /// Moves to crew members if they are within the detection range
    /// </summary>
    public void DetectCrewMember()
    {
        foreach(Character crewMember in crewMembers)
        {
            if(crewMember.isActiveAndEnabled)
            {
                //Moves towards crew members if they are within range
                if (Vector3.Distance(transform.position, crewMember.transform.position) < detectionRange)
                {
                    //Debug.Log("IN RANGE");
                    charAgent.SetDestination(crewMember.transform.position);
                }
            }
           
        }
    }

    public override void Death()
    {
        base.Death();

        if(currentHealth <= 0)
        {
            //unitSelectionList.enemies.Remove(gameObject);

            if(lootDropped == false)
            {
                CombatResourcesUI combatResource = combatUI.GetComponent<CombatResourcesUI>();

                //increase doubloon amount upon killing an enemy
                combatResource.doubloonAmount += lootValue;
                combatResource.UpdateDubloonUI(combatResource.doubloonAmount);

                //Create a pop up message for resources gained
                Vector3 messagePos = transform.position + new Vector3(0, 1.5f, 0);
                GameObject popUpMessage = Instantiate(resourceText, messagePos, Quaternion.identity) as GameObject;
                popUpMessage.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+" + lootValue;

                lootDropped = true;
            }
  

            cm.Enemies.Remove(gameObject);
        }

    }
}
