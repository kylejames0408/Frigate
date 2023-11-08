using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpMessage : MonoBehaviour
{
    [SerializeField] private float textHeight;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 1f);
        transform.position += new Vector3(0, textHeight, 0);
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

}
