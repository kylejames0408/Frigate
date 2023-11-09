using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyHouse : MonoBehaviour
{
    public GameObject resourceText;
    public int lootValue;
    public string resourceType;

    // Start is called before the first frame update
    void Start()
    {
        //lootValue = 40;
        //resourceType = "default";
    }

    public void CreatePopUpText()
    {
        Vector3 messagePos = transform.position + new Vector3(0, 5.5f, 0);
        GameObject popUpMessage = Instantiate(resourceText, messagePos, Quaternion.identity) as GameObject;
        popUpMessage.transform.GetChild(0).GetComponent<TextMeshPro>().text = "+" + lootValue;
    }
}
