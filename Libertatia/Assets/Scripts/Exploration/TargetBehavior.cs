using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetBehavior : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("TEST");

        if (collision.gameObject.layer == 9 && collision.gameObject.CompareTag("Island"))
        {
            collision.gameObject.layer = 0;
        }
        else if (collision.gameObject.layer == 9)
        {
            Destroy(this.gameObject);
        }
    }
}