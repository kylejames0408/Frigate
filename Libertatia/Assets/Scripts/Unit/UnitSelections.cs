using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelections : MonoBehaviour
{
    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();

    private static UnitSelections _instance;
    public static UnitSelections Instance { get { return _instance; } }

    public GameObject[] enemies;

    private void Awake()
    {
        //if an instance of this already exists and it isn't this one
        if(_instance != null && _instance != this)
        {
            //destroy this instance
            Destroy(this.gameObject);
        }
        else
        {
            //make this the instance
            _instance = this;
        }

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    public void Update()
    {
        foreach(GameObject enemy in enemies)
        {
            if(enemy != null)
            {
                AttackEnemy(enemy);
            }
        }
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
        if(!unitsSelected.Contains(unitToAdd))
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
            unit.transform.GetChild(0).gameObject.SetActive(false);
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
        unitsSelected.Add(unitToAdd);
        //sets the first child to be active: an indicator showing that the unit is selected
        unitToAdd.transform.GetChild(0).gameObject.SetActive(true);

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
    private void RemoveSelection(GameObject unitToRemove)
    {
        unitsSelected.Remove(unitToRemove);
        //sets the first child to be active: an indicator showing that the unit is selected
        unitToRemove.transform.GetChild(0).gameObject.SetActive(false);

        //sets the second child to be active: a sphere showing detection range
        //unitToRemove.transform.GetChild(1).gameObject.SetActive(false);

        unitToRemove.GetComponent<UnitMovement>().enabled = false;
    }



    /// <summary>
    /// Attack enemy units upon coming into range
    /// </summary>
    /// <param name="enemy"></param>
    /// 
    //TO BE UPDATED
    public void AttackEnemy(GameObject enemy)
    {
        //Debug.Log(enemy.tag);

        //enemy.transform.GetChild(0).gameObject.SetActive(true);

        for(int i = 0; i < unitList.Count; i++)
        {
            if (Vector3.Distance(unitList[i].transform.position, enemy.transform.position) < 2)
            {
                if(enemy.tag == "Enemy")
                {
                    Destroy(enemy);
                }
              
            }
        }
  

    }
}
