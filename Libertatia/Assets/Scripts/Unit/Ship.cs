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
        // the rect that is the canvas
        GameObject canvas = GameObject.Find("BoxSelectCanvas");
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // the style used to set the text size and 
        GUIStyle GUIBoxStyle = new GUIStyle(GUI.skin.box);
        GUIBoxStyle.fontSize = (int)(canvasRect.rect.height * 0.023f);
        GUIBoxStyle.alignment = TextAnchor.MiddleCenter;

        if (inRange)
        {
            GUI.Box(new Rect(canvasRect.rect.width * 0.35f, canvasRect.rect.height * 0.05f, canvasRect.rect.width * 0.21f, canvasRect.rect.height * 0.05f),
                "Press E to return to outpost.", GUIBoxStyle);
        }

    }
}
