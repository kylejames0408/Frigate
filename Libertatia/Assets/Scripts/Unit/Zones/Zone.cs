using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Zone : MonoBehaviour
{
    public List<GameObject> crewMembersInZone;
    public List<GameObject> enemiesInZone;

    public string zoneName;

    float onMeshThreshold = 3;

    public Vector3 zoneCenter;

    // Start is called before the first frame update
    void Start()
    {
        zoneName = gameObject.name;

        TerrainCollider tCollider = gameObject.GetComponent<TerrainCollider>();

        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();

        //zoneCenter = boxCollider.center;

    }

    private void Update()
    {
        foreach(GameObject crewMember in crewMembersInZone.ToList())
        {
            CheckUnitHealth(crewMember);
        }

        foreach(GameObject enemy in enemiesInZone.ToList())
        {
            CheckUnitHealth(enemy);
        }

    }

    public void OnTriggerEnter(Collider collider)
    {
        //If a crew member or enemy is within the zone, add them to the list
        if (collider.gameObject.tag == "PlayerCharacter" || collider.gameObject.tag == "Enemy")
        {
            //Debug.Log(collider.gameObject.name + " Enter");

            AddToUnitsInZoneList(collider.gameObject);

        }
    }

    public void OnTriggerExit(Collider other)
    {
        //Removes crew member or enemy from the list if they exit the zone
        if (other.gameObject.tag == "PlayerCharacter" || other.gameObject.tag == "Enemy")
        {
            //Debug.Log(other.gameObject.name + " Exit");

            RemoveUnitsInZoneList(other.gameObject);
        }
    }

    /// <summary>
    /// Adds unit to units in zone list based on their type
    /// </summary>
    /// <param name="agents"></param>
    public void AddToUnitsInZoneList(GameObject unit)
    {
        if(unit.gameObject.tag == "PlayerCharacter")
        {
            if (crewMembersInZone.Contains(unit) == false)
            {
                crewMembersInZone.Add(unit.gameObject);
            }
        }

        if (unit.gameObject.tag == "Enemy")
        {
            if (enemiesInZone.Contains(unit) == false)
            {
                enemiesInZone.Add(unit.gameObject);
            }
        }

    }

    /// <summary>
    /// Removes unit from units in zone list based on their type
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnitsInZoneList(GameObject unit)
    {
        Character character = unit.GetComponent<Character>();

        if (unit.gameObject.tag == "PlayerCharacter")
        {
            if (crewMembersInZone.Contains(unit))
            {
                crewMembersInZone.Remove(unit);
            }
        }

        if (unit.gameObject.tag == "Enemy")
        {
            if (enemiesInZone.Contains(unit))
            {
                enemiesInZone.Remove(unit);
            }
        }
    }

    /// <summary>
    /// Removes units from lists if their health reaches 0
    /// </summary>
    /// <param name="unit"></param>
    public void CheckUnitHealth(GameObject unit)
    {
        Character character = unit.gameObject.GetComponent<Character>();

        if (character.currentHealth <= 0)
        {
            RemoveUnitsInZoneList(character.gameObject);
        }
    }
   


}
