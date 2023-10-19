using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Zone : MonoBehaviour
{
    public List<GameObject> unitsInZone;

    public string zoneName;

    float onMeshThreshold = 3;


    // Start is called before the first frame update
    void Start()
    {
        zoneName = gameObject.name;

        TerrainCollider tCollider = gameObject.GetComponent<TerrainCollider>();
    }

    public void OnTriggerEnter(Collider collider)
    {
        //If a crew member or enemy is within the zone, add them to the list
        if (collider.gameObject.tag == "PlayerCharacter" || collider.gameObject.tag == "Enemy")
        {
            //Debug.Log(collider.gameObject.name + " Enter");

            if (unitsInZone.Contains(collider.gameObject) == false)
            {
                unitsInZone.Add(collider.gameObject);
            }

        }
    }

    public void OnTriggerExit(Collider other)
    {
        //Removes crew member or enemy from the list if they exit the zone
        if (other.gameObject.tag == "PlayerCharacter" || other.gameObject.tag == "Enemy")
        {
            //Debug.Log(other.gameObject.name + " Exit");

            if (unitsInZone.Contains(other.gameObject))
            {
                unitsInZone.Remove(other.gameObject);
            }

        }
    }

    /// <summary>
    /// Adds unit to units in zone list
    /// </summary>
    /// <param name="agents"></param>
    public void AddToUnitsInZoneList(GameObject unit)
    {
        if (unitsInZone.Contains(unit) == false)
        {
            unitsInZone.Add(unit.gameObject);
        }
    }

    /// <summary>
    /// Removes unit from units in zone list
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveUnitsInZoneList(GameObject unit)
    {
        if (unitsInZone.Contains(unit))
        {
            unitsInZone.Remove(unit);
        }
    }


}
