using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelections : MonoBehaviour
{
    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> unitsSelected = new List<GameObject>();

    private static UnitSelections _instance;
    public static UnitSelections Instance { get { return _instance; } }

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
    }

    /// <summary>
    /// Selects a single clicked unit
    /// </summary>
    /// <param name="unitToAdd"></param>
    public void ClickSelect(GameObject unitToAdd)
    {
        DeselectAll();
        addSelection(unitToAdd);
    }

    /// <summary>
    /// Selects multiple units while holding shift
    /// </summary>
    /// <param name="unitToAdd"></param>
    public void ShiftClickSelect(GameObject unitToAdd)
    {
        if(!unitsSelected.Contains(unitToAdd))
        {
            removeSelection(unitToAdd);
        }
        else
        {
            addSelection(unitToAdd);
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
            addSelection(unitToAdd);
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
        }
        unitsSelected.Clear();
    }

    /// <summary>
    /// Adds the selected units into a list
    /// </summary>
    /// <param name="unitToAdd"></param>
    private void addSelection(GameObject unitToAdd)
    {
        unitsSelected.Add(unitToAdd);
        //sets the first child to be active: an indicator showing that the unit is selected
        unitToAdd.transform.GetChild(0).gameObject.SetActive(true);

        //Debug.Log(unitToAdd.tag);

        //prevents the player from moving enemy units
        if(unitToAdd.tag != "Enemy")
        {
            unitToAdd.GetComponent<UnitMovement>().enabled = true;
        }

    }

    /// <summary>
    /// Removes the units from the selected units list
    /// </summary>
    /// <param name="unitToRemove"></param>
    private void removeSelection(GameObject unitToRemove)
    {
        unitsSelected.Remove(unitToRemove);
        //sets the first child to be active: an indicator showing that the unit is selected
        unitToRemove.transform.GetChild(0).gameObject.SetActive(false);

        unitToRemove.GetComponent<UnitMovement>().enabled = false;
    }

}
