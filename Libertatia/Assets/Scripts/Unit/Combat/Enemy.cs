using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.CanvasScaler;

public class Enemy : Character
{
    public List<Character> crewMembers;
    public List<GameObject> crewMemberGameObjects;

    public float detectionRange;

    public GameObject unitSelection;
    public UnitSelections unitSelectionList;



    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        attackRange = 2;
        attackRate = 4;
        damage = 25;
        detectionRange = 10;

        charAgent = GetComponent<NavMeshAgent>();

        //Finds all crew members in the scene and adds them into the gameobject list
        crewMemberGameObjects.AddRange(GameObject.FindGameObjectsWithTag("PlayerCharacter"));

        //take the gameobject crew members and turn them into character objects
        for(int i = 0; i < crewMemberGameObjects.Count; i++)
        {
            Character crewMember = crewMemberGameObjects[i].GetComponent<Character>();
            crewMembers.Add(crewMember);
        }

        unitSelection = GameObject.FindGameObjectWithTag("Unit Selections");
        unitSelectionList = unitSelection.GetComponent<UnitSelections>();
    }

    // Update is called once per frame
    void Update()
    {
        Attack(crewMembers);
        Death();

        if(gameObject.active != false)
        {
            DetectCrewMember();
        }

    }

    /// <summary>
    /// Moves to crew members if they are within the detection range
    /// </summary>
    public void DetectCrewMember()
    {
        foreach(Character crewMember in crewMembers)
        {
            if (Vector3.Distance(transform.position, crewMember.transform.position) < detectionRange)
            {
                //Debug.Log("IN RANGE");
                charAgent.SetDestination(crewMember.transform.position);
            }
        }
    }

    public override void Death()
    {
        base.Death();

        if(health <= 0)
        {
            unitSelectionList.enemies.Remove(gameObject);
        }

        


    }
}
