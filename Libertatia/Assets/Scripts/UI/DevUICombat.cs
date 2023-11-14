using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DevUICombat : MonoBehaviour
{
    private bool isOpen = false;
    private float interfaceAnimSpeed = 0.6f;

    [SerializeField] private CrewmateManager cm;
    [SerializeField] private ResourcesUI rui;

    [SerializeField] private Button btnArrow;
    [SerializeField] private Button btnNewMate;

    private void Awake()
    {
        isOpen = false;

        if (cm == null) { cm = FindObjectOfType<CrewmateManager>(); }
        if (rui == null) { rui = FindObjectOfType<ResourcesUI>(); }

        btnArrow.onClick.AddListener(OpenCloseMenu);
        btnNewMate.onClick.AddListener(NewMate);
    }

    private void OpenCloseMenu()
    {
        if (isOpen)
        {
            isOpen = false;
            transform.DOMoveX(331 + Screen.width, interfaceAnimSpeed); // doesont do local
        }
        else
        {
            isOpen = true;
            transform.DOMoveX(Screen.width, interfaceAnimSpeed);
        }
    }
    // Manager
    private void NewMate()
    {
        cm.SpawnNewCrewmate();
    }
}
