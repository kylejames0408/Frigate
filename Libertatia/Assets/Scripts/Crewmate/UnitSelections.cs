using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class UnitSelections : MonoBehaviour
{
    // Components
    [SerializeField] private CrewmateManager cm;

    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();

    private static UnitSelections _instance;
    public static UnitSelections Instance
    {
        get
        {
            return _instance;
        }
    }

    public List<GameObject> enemies;

    public GameEvent allEnemiesDead;

    private bool eventTriggered;

    private string sceneName;

    public GameObject crewMemberPrefab;

    private void Start()
    {
        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }

        eventTriggered = false;

        sceneName = SceneManager.GetActiveScene().name;
        unitList.Clear();

        //spawns crew members based on the crew size in playerdata
        //for (int i = 0; i < 6; i++)
        //for (int i = 0; i < GameManager.Data.crewmates.Count; i++)
        //{

          //  GameObject unit = Instantiate(crewMemberPrefab, new Vector3(-5 - 5, 0) + new Vector3(
            //    UnityEngine.Random.Range(-1.0f, 1.0f) * 5, -5, UnityEngine.Random.Range(-1.0f, 1.0f) * 5), Quaternion.identity);

        //}


        unitList = GameObject.FindGameObjectsWithTag("PlayerCharacter").ToList<GameObject>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList<GameObject>();
    }

    private void Awake()
    {
        //if an instance of this already exists and it isn't this one
        if(_instance != null && _instance != this)
        {
            //destroy this instance
            Destroy(gameObject);
        }
        else
        {
            //make this the instance
            _instance = this;
        }
    }

    public void Update()
    {
        //for(int i = 0; i<enemies.Count; i++)
        //{
        //    if(enemies[i] != null)
        //    {
        //        AttackEnemy(enemies[i]);
        //    }
        //}

        if (sceneName == "Combat")
        {
            TriggerEvent();
        }

        //Debug.Log(pData.crew.amount);

    }

    /// <summary>
    /// Selects a single clicked unit
    /// </summary>
    /// <param name="unitToAdd"></param>
    public void ClickSelect(GameObject unitToAdd)
    {
        DeselectAll();
        AddSelection(unitToAdd);
    }

    /// <summary>
    /// Selects multiple units while holding shift
    /// </summary>
    /// <param name="unitToAdd"></param>
    public void ShiftClickSelect(GameObject unitToAdd)
    {
        if(unitsSelected.Contains(unitToAdd))
        {
            RemoveSelection(unitToAdd);
        }
        else
        {
            AddSelection(unitToAdd);
        }
    }

    /// <summary>
    /// Selects multiple units when dragging a box
    /// </summary>
    /// <param name="unitToAdd"></param>
    public void DragSelect(GameObject unitToAdd)
    {
        if(!unitsSelected.Contains(unitToAdd))
        {
            AddSelection(unitToAdd);
        }
    }

    /// <summary>
    /// Deselects every unit in the selected list
    /// </summary>
    public void DeselectAll()
    {
        foreach(var unit in unitsSelected)
        {
            unit.GetComponent<UnitMovement>().enabled = false;
            //unit.transform.GetChild(0).gameObject.SetActive(false);

            //Switches the character's shader when selected
            Character unitChar = unit.GetComponent<Character>();
            unitChar.rend.sharedMaterial = unitChar.materials[0];

            //unit.transform.GetChild(1).gameObject.SetActive(false);
        }
        unitsSelected.Clear();
    }

    /// <summary>
    /// Adds the selected units into a list
    /// </summary>
    /// <param name="unitToAdd"></param>
    private void AddSelection(GameObject unitToAdd)
    {
        if(sceneName == "Outpost")
        {
        }


        unitsSelected.Add(unitToAdd);
        //sets the first child to be active: an indicator showing that the unit is selected
        //unitToAdd.transform.GetChild(0).gameObject.SetActive(true);

        //Switches the character's shader when selected
        Character unit = unitToAdd.GetComponent<Character>();
        unit.rend.sharedMaterial = unit.materials[1];

        Crewmate crewmate = unitToAdd.GetComponent<Crewmate>();
        cm.SelectCrewmate(crewmate.cardIndex);

        //sets the second child to be active: a sphere showing detection range
        //unitToAdd.transform.GetChild(1).gameObject.SetActive(true);

        //Debug.Log(unitToAdd.tag);

        //prevents the player from moving enemy units
        if (unitToAdd.tag != "Enemy")
        {
            unitToAdd.GetComponent<UnitMovement>().enabled = true;
        }

    }

    /// <summary>
    /// Removes the units from the selected units list
    /// </summary>
    /// <param name="unitToRemove"></param>
    public void RemoveSelection(GameObject unitToRemove)
    {
        unitsSelected.Remove(unitToRemove);
        //sets the first child to be active: an indicator showing that the unit is selected
        //unitToRemove.transform.GetChild(0).gameObject.SetActive(false);

        //Switches the character's shader when deselected
        Character unit = unitToRemove.GetComponent<Character>();
        unit.rend.sharedMaterial = unit.materials[0];

        //sets the second child to be active: a sphere showing detection range
        //unitToRemove.transform.GetChild(1).gameObject.SetActive(false);

        unitToRemove.GetComponent<UnitMovement>().enabled = false;
    }

    public void MoveToEnemy(GameObject enemy)
    {
        foreach (GameObject e in enemies)
        {
            e.transform.GetChild(0).gameObject.SetActive(false);
        }
        enemy.transform.GetChild(0).gameObject.SetActive(true);
    }








    /// <summary>
    /// Triggers tutorial event once enemies are all dead
    /// </summary>
    /// <param name="enemy"></param>
    ///
    //TO BE REMOVED
    public void TriggerEvent()
    {

        if(eventTriggered == false)
        {
            if (enemies.Count == 0)
            {
                allEnemiesDead.Raise(this, enemies);
                eventTriggered = true;
            }
        }

    }


}
