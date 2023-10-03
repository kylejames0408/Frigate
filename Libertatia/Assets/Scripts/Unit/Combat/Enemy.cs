using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public List<Character> crewMembers;
    public List<GameObject> crewMemberGameObjects;

    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        attackRange = 2;
        attackRate = 4;
        damage = 25;

        //Finds all crew members in the scene and adds them into the gameobject list
        crewMemberGameObjects.AddRange(GameObject.FindGameObjectsWithTag("PlayerCharacter"));

        //take the gameobject crew members and turn them into character objects
        for(int i = 0; i < crewMemberGameObjects.Count; i++)
        {
            Character crewMember = crewMemberGameObjects[i].GetComponent<Character>();
            crewMembers.Add(crewMember);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Attack(crewMembers);
        Death();
    }
}
