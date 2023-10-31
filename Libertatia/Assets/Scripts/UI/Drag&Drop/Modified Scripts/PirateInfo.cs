using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyDragAndDrop.Core;

public class PirateInfo : MonoBehaviour
{
    public Text Name;
    public Image Image;
    public Sprite BlankImage;

    private void Awake()
    {
        //In the future, we will have sprites put into asset bundles so they can be loaded through script rather than reference
    }

    public void Initialize(PirateInfo demo)
    {
        Name.text = demo.Name.text;
        Image.sprite = demo.Image.sprite;
        Image.color = demo.Image.color;
        Image.gameObject.SetActive(false);
        Image.gameObject.SetActive(true);
    }

    public void Clear()
    {
        Name.text = "Unassigned";
        Image.sprite = BlankImage;
        Image.gameObject.SetActive(false);
        Image.gameObject.SetActive(true);
    }
}
