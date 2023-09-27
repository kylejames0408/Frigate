using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> enemyList = new List<GameObject>();
    public int enemyCount;

    public int detectionRange;

    // Start is called before the first frame update
    void Start()
    {
        detectionRange = 10;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(enemyList.Count);

        foreach (GameObject unit in unitList)
        {
            if(Vector3.Distance(transform.position, unit.transform.position) <= 40)
            {
                //Debug.Log("Go home");
      
            }
        }
    }

    //private void OnGUI()
    //{
    //    GUI.Box(new Rect(40, 130, 250, 80), "Go home");
    //}
}
