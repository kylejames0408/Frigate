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
        UnitSelections.Instance.unitList.Remove(gameObject);
    }
}
