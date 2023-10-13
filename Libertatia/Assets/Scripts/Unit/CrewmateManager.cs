using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CrewmateManager : MonoBehaviour
{
    public List<GameObject> unitsSelected = new List<GameObject>();
    private static CrewmateManager _instance;
    public static CrewmateManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void ClickSelect(GameObject unitToAdd)
    {
        DeselectAll();
        AddSelection(unitToAdd);
    }
    public void DeselectAll()
    {
        foreach(var unit in unitsSelected)
        {
            unit.transform.GetChild(0).gameObject.SetActive(false);
        }
        unitsSelected.Clear();
    }
    private void AddSelection(GameObject unitToAdd)
    {
        if(unitToAdd.GetComponent<Crewmate>().IsBuilding)
        {
            return;
        }

        unitsSelected.Add(unitToAdd);
        unitToAdd.transform.GetChild(0).gameObject.SetActive(true);
    }
    public void RemoveSelection(GameObject unitToRemove)
    {
        unitsSelected.Remove(unitToRemove);
        //sets the first child to be active: an indicator showing that the unit is selected
        unitToRemove.transform.GetChild(0).gameObject.SetActive(false);
    }

}
