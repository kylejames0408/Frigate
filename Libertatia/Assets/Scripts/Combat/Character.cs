using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Character : MonoBehaviour
{
    [SerializeField] protected int maxHealth;
    public int currentHealth;

    public int attackRange;
    public float attackRate;
    public int damage;

    public NavMeshAgent charAgent;

    public HealthBar healthbar;

    public Material[] materials;
    public Renderer rend;

    //where the character is moving to
    public Vector3 targetPos;

    public State characterState;

    public float maxSpeed;

    [SerializeField] protected GameObject omuiGameObject;

    //enum for the state of the unit
    public enum State
    {
        Idle,
        Moving,
        Attacking,
        Dead,
    }

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 100;
        currentHealth = 100;
        attackRange = 2;
        attackRate = 2;
        damage = 50;
        maxSpeed = 4.5f;

        healthbar.UpdateHealthBar(maxHealth, currentHealth);

        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = materials[0];

        //sets characters to be idle by default
        characterState = State.Idle;

        omuiGameObject = GameObject.FindGameObjectWithTag("ManagementUI");
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
                if(unit.currentHealth > 0)
                {
                    characterState = State.Attacking;

                    if(gameObject.tag == "PlayerCharacter")
                    {
                        Crewmate crewmate = GetComponent<Crewmate>();
                        crewmate.State = CrewmateState.ATTACKING;

                        CombatManagementUI cmui = omuiGameObject.GetComponent<CombatManagementUI>();
                        cmui.UpdateCard(crewmate.ID, crewmate.StateIcon);
                    }

                    //attacks and decreases their health based on attack rate
                    attackRate -= Time.deltaTime;
                    if (attackRate <= 0)
                    {
                        unit.currentHealth -= damage;
                        //Debug.Log("Attack " + unit.name + " " + unit.currentHealth);

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
        //Removes unit if the healthbar reaches 0
        //if(currentHealth <= 0)
        if(healthbar.healthBarSprite.fillAmount <= 0)
        {
            characterState = State.Dead;
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Gets the Vector3 of the nearest hostile unit
    /// </summary>
    /// <param name="units"></param>
    /// <returns></returns>
    public Vector3 GetClosestUnit(List<GameObject> units)
    {
        Transform[] unitPositions = new Transform[units.Count];

        for (int i = 0; i < units.Count; i++)
        {
            unitPositions[i] = units[i].transform;
        }

        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        //for each target in the zone
        foreach (Transform potentialTarget in unitPositions)
        {
            Vector3 dirToTarget = potentialTarget.position - currentPosition;
            float distSqrToTarget = dirToTarget.sqrMagnitude;

            //if the distance to a unit is closer than the current shortest distance
            if (distSqrToTarget < closestDistanceSqr)
            {
                //it becomes the new closest target
                closestDistanceSqr = distSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget.position;
    }
}
