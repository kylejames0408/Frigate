using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Zone : MonoBehaviour
{
    public List<GameObject> crewMembersInZone;
    public List<GameObject> enemiesInZone;

    public List<GameObject> housesInZone;

    public string zoneName;

    float onMeshThreshold = 3;

    public Vector3 zoneCenter;
    public GameObject centerObject;

    public bool zoneLootCollected;

    // Start is called before the first frame update
    void Start()
    {
        zoneName = gameObject.name;

        TerrainCollider tCollider = gameObject.GetComponent<TerrainCollider>();

        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();

        //zoneCenter = boxCollider.center;

        zoneLootCollected = false;

        zoneCenter = centerObject.transform.position;
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
        //Adds gameobjects within the zone into their respective lists
        if (collider.gameObject.tag == "PlayerCharacter" || collider.gameObject.tag == "Enemy" || collider.gameObject.tag == "EnemyHouse")
        {
            //Debug.Log(collider.gameObject.name + " Enter");

            AddToZoneList(collider.gameObject);

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
    /// Adds game objects to zone list based on their type
    /// </summary>
    /// <param name="agents"></param>
    public void AddToZoneList(GameObject objectType)
    {
        if(objectType.gameObject.tag == "PlayerCharacter")
        {
            if (crewMembersInZone.Contains(objectType) == false)
            {
                crewMembersInZone.Add(objectType.gameObject);
            }
        }

        if (objectType.gameObject.tag == "Enemy")
        {
            if (enemiesInZone.Contains(objectType) == false)
            {
                enemiesInZone.Add(objectType.gameObject);
            }
        }

        if (objectType.gameObject.tag == "EnemyHouse")
        {
            if (housesInZone.Contains(objectType) == false)
            {
                housesInZone.Add(objectType.gameObject);
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
