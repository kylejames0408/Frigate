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

    public GameObject combatUI;

    // Start is called before the first frame update
    void Start()
    {
        detectionRange = 30;
        inRange = false;

        unitList.AddRange(GameObject.FindGameObjectsWithTag("PlayerCharacter"));
        enemyList.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        combatUI = GameObject.FindGameObjectWithTag("CombatUI");
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject unit in unitList)
        {
            if (unit.activeSelf != false)
            {
                //If a unit is within range of the ship, the player can return to the outpost
                if (Vector3.Distance(transform.position, unit.transform.position) <= detectionRange)
                {
                    //Debug.Log("Go home");
                    inRange = true;

                }
                else
                    inRange = false;
            }

        }

        if (inRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                //GameManager.data.resources.wood += 50;
                //GameManager.data.resources.doubloons += 10;
                //GameManager.data.resources.food += 100;

                CombatResourcesUI combatResources = combatUI.GetComponent<CombatResourcesUI>();

                GameManager.data.resources.wood += combatResources.woodAmount;
                GameManager.data.resources.doubloons += combatResources.doubloonAmount;
                GameManager.data.resources.food += combatResources.foodAmount;
                CeneManager.LoadOutpostFromCombat();
            }
        }

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
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
