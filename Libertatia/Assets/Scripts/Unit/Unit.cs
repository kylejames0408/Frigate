using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UnitSelections.Instance.unitList.Add(gameObject);
    }

    private void OnDestroy()
    {
        UnitSelections selection = UnitSelections.Instance;
        if(selection != null)
        {
            selection.enemies.Remove(gameObject);
        }
    }
}
