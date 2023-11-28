using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

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
    public CombatResourcesUI resourceUI;

    public bool lootDropped;
    public int lootValue;

    private void Awake()
    {
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
        if (resourceUI == null) { resourceUI = FindObjectOfType<CombatResourcesUI>(); }
        if (combatUI == null) { combatUI = GameObject.FindGameObjectWithTag("CombatUI"); };
    }
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 70;
        currentHealth = 70;
        attackRange = 3;
        attackRate = 3;
        damage = 10;
        detectionRange = 10;
        maxSpeed = 3.5f;

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

        lootDropped = false;

        omuiGameObject = GameObject.FindGameObjectWithTag("ManagementUI");
    }

    // Update is called once per frame
    void Update()
    {
        if(crewMemberGameObjects.Count == 0)
        {
            crewMemberGameObjects.AddRange(GameObject.FindGameObjectsWithTag("PlayerCharacter"));

            for (int i = 0; i < crewMemberGameObjects.Count; i++)
            {
                Character crewMember = crewMemberGameObjects[i].GetComponent<Character>();
                crewMembers.Add(crewMember);
            }
        }

        Attack(crewMembers);

        healthbar.UpdateHealthBar(maxHealth, currentHealth);
        Death();

        if(gameObject.activeSelf != false)
        {
            //DetectCrewMember();
        }

        if (Vector3.Distance(transform.position, targetPos) < 1.0f)
        {
            characterState = State.Idle;
        }

        //controls agent speed based on state
        switch (characterState)
        {
            case State.Idle:
                charAgent.speed = 0;
                break;
            case State.Moving:
                charAgent.speed = maxSpeed;
                break;
            case State.Attacking:
                charAgent.speed = 0;
                break;
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
            if(lootDropped == false)
            {
                //increase doubloon amount upon killing an enemy
                resourceUI.doubloonAmount += lootValue;
                resourceUI.UpdateDubloonUI(resourceUI.doubloonAmount);

                //Create a pop up message for resources gained
                Vector3 messagePos = transform.position + new Vector3(0, 1.5f, 0);
                GameObject popUpMessage = Instantiate(resourceText, messagePos, Quaternion.identity) as GameObject;
                popUpMessage.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+" + lootValue;

                lootDropped = true;
            }

            crewMembers.Remove(this);
        }

    }
}
