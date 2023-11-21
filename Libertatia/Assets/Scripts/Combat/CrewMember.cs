using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrewMember : Character
{
    public List<Character> enemies;
    public List<GameObject> enemyGameobjects;

    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        currentHealth = 100;
        attackRange = 3;
        attackRate = 2;
        damage = 25;
        maxSpeed = 3.5f;

        charAgent = GetComponent<NavMeshAgent>();

        enemyGameobjects.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        for (int i = 0; i < enemyGameobjects.Count; i++)
        {
            Character crewMember = enemyGameobjects[i].GetComponent<Character>();
            enemies.Add(crewMember);
        }

        lineRenderer = GetComponent<LineRenderer>();

        omuiGameObject = GameObject.FindGameObjectWithTag("ManagementUI");
    }

    // Update is called once per frame
    void Update()
    {
        Attack(enemies);
        healthbar.UpdateHealthBar(maxHealth, currentHealth);
        Death();

        Crewmate crewmate = GetComponent<Crewmate>();

        //if the crew member is within a certain distance to the target pos, makes them idle
        if (Vector3.Distance(transform.position, targetPos) < 1.0f)
        {
            characterState = State.Idle;
            crewmate.State = CrewmateState.IDLE;

            OutpostManagementUI omui = omuiGameObject.GetComponent<OutpostManagementUI>();
            omui.UpdateCard(crewmate.ID, crewmate.StateIcon);
       
        }


        //Debug.Log(crewmate.StateIcon);


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

    public override void Death()
    {
        base.Death();

        if (currentHealth <= 0)
        {
            //unitSelectionList.enemies.Remove(gameObject);

            //GameObject.Find("Unit Selections").GetComponent<UnitSelections>().unitList.Remove(gameObject);
            //GameManager.Data.crewmates.RemoveAt(0);
        }

    }
}
