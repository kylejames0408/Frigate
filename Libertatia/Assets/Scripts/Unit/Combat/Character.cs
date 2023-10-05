using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Character : MonoBehaviour
{
    public int health;

    //public List<Character> crewMembers;
    //public List<Character> enemies;

    public int attackRange;
    public float attackRate;
    public int damage;

    public NavMeshAgent charAgent;



    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        attackRange = 2;
        attackRate = 2;
        damage = 50;
    }

    // Update is called once per frame
    void Update()
    {
        //Attack();
        //Death();
    }

    /// <summary>
    /// Attacks opposing unit when in range
    /// </summary>
    public void Attack(List<Character> characterList)
    {
        foreach (Character unit in characterList)
        {
            //if opposing unit is within attack range
            if (Vector3.Distance(transform.position, unit.transform.position) < attackRange)
            {
                if(unit.health > 0)
                {
                    //attacks and decreases their health based on attack rate
                    attackRate -= Time.deltaTime;
                    if (attackRate <= 0)
                    {
                        unit.health -= damage;
                        Debug.Log("Attack " + unit.name + " " + unit.health);
                        attackRate = 2;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Removes unit when health reaches 0
    /// </summary>
    public virtual void Death()
    {
        if(health <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    
}
