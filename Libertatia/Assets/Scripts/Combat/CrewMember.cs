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

        charAgent = GetComponent<NavMeshAgent>();

        enemyGameobjects.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        for (int i = 0; i < enemyGameobjects.Count; i++)
        {
            Character crewMember = enemyGameobjects[i].GetComponent<Character>();
            enemies.Add(crewMember);
        }

        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Attack(enemies);
        healthbar.UpdateHealthBar(maxHealth, currentHealth);
        Death();
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
