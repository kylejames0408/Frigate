using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

public class Resource : MonoBehaviour
{
    [SerializeField] ResourceType resourceType;
    [SerializeField] int amount;
    Damageable damageable;
    public bool isHover;

    //HoverVisual
    //private Renderer renderer;
    private Color emissionColor;

    void Awake()
    {
        damageable = GetComponent<Damageable>();
        damageable.onDestroy.AddListener(GiveResource);
        damageable.onHit.AddListener(HitResource);

        //renderer = GetComponent<Renderer>();
        //if (renderer)
        //    emissionColor = renderer.material.GetColor("_EmissionColor");
    }

    void GiveResource()
    {
        BuildingManager.Instance.AddResource(resourceType, amount);
    }

    void HitResource()
    {
        //visual
        //transform.DOComplete();
        //transform.DOShakeScale(.5f, .2f, 10, 90, true);
    }

    private void OnMouseEnter()
    {
        //isHover = true;
        //if (renderer)
        //    renderer.material.SetColor("_EmissionColor", Color.grey);
    }
    private void OnMouseExit()
    {
        //isHover = false;
        //if (renderer)
        //    renderer.material.SetColor("_EmissionColor", emissionColor);
    }
}
