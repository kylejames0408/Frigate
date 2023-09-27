using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ship : MonoBehaviour
{
    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> enemyList = new List<GameObject>();
    public int enemyCount;

    public int detectionRange;
    private bool inRange;

    // Start is called before the first frame update
    void Start()
    {
        detectionRange = 60;
        inRange = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(enemyList.Count);

        foreach (GameObject unit in unitList)
        {
            if(Vector3.Distance(transform.position, unit.transform.position) <= detectionRange)
            {
                //Debug.Log("Go home");
                inRange = true;
      
            }
        }

        if(inRange)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadScene("Outpost");
            }
        }

    }

    private void OnGUI()
    {
        if(inRange)
        {
            GUI.Box(new Rect(40, 150, 250, 20), "Press E to return to base.");
        }

    }
}
