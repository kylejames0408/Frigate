using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class Zone : MonoBehaviour
{
    public List<GameObject> crewMembersInZone;
    public List<GameObject> enemiesInZone;

    public List<GameObject> housesInZone;

    public string zoneName;

    float onMeshThreshold = 3;

    public Vector3 zoneCenter;

    //zone center empty gameobject + flag prefab
    public GameObject centerObject;

    private LineRenderer outlineRenderer;

    private GradientAlphaKey[] gradientAlphaKey;
    private GradientColorKey[] redColorKey;
    private GradientColorKey[] blueColorKey;
    private GradientColorKey[] greenColorKey;
    private GradientColorKey[] yellowColorKey;
    private GradientColorKey[] grayColorKey;

    public bool zoneLootCollected;

    public bool mouseHoveringZone;

    //Checks to see if you can move units to the zone
    public bool isClickable;

    // Used for raising zone claim events
    public GameEvent zoneClaimed;
    public bool zoneClaimTriggered;

    // Start is called before the first frame update
    void Start()
    {
        zoneName = gameObject.name;

        TerrainCollider tCollider = gameObject.GetComponent<TerrainCollider>();

        BoxCollider boxCollider = gameObject.GetComponent<BoxCollider>();

        outlineRenderer = gameObject.GetComponent<LineRenderer>();

        gradientAlphaKey = new GradientAlphaKey[] { new GradientAlphaKey(0.5f, 0.0f), new GradientAlphaKey(0.5f, 1.0f) };
        redColorKey = new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) };
        blueColorKey = new GradientColorKey[] { new GradientColorKey(Color.blue, 0.0f), new GradientColorKey(Color.blue, 1.0f) };
        greenColorKey = new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.green, 1.0f) };
        yellowColorKey = new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.yellow, 1.0f) };
        grayColorKey = new GradientColorKey[] { new GradientColorKey(Color.gray, 0.0f), new GradientColorKey(Color.gray, 1.0f) };

        //zoneCenter = boxCollider.center;

        zoneLootCollected = false;

        zoneCenter = centerObject.transform.position;

        isClickable = true;

        zoneClaimTriggered = false;
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

        if (!isClickable)
        {
            DrawOutline(grayColorKey);
        }
        else if (mouseHoveringZone)
        {
            DrawOutline(blueColorKey);
        }
        else if((zoneLootCollected && enemiesInZone.Count == 0) || zoneName == "Safe Zone")
        {
            DrawOutline(greenColorKey);
        }
        else if(crewMembersInZone.Count > 0 && (!zoneLootCollected || enemiesInZone.Count > 0))
        {
            DrawOutline(yellowColorKey);
        }
        else
        {
            DrawOutline(redColorKey);
        }

        if(zoneLootCollected && !zoneClaimTriggered && zoneName != "Safe Zone")
        {
            zoneClaimed.Raise(this, this);
            zoneClaimTriggered = true;
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

    public void DrawOutline(GradientColorKey[] colorKey)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(colorKey, gradientAlphaKey);
        outlineRenderer.colorGradient = gradient;
    }

    private void OnMouseEnter()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            mouseHoveringZone = true;
        }
    }

    private void OnMouseExit()
    {
        mouseHoveringZone = false;
    }
}
