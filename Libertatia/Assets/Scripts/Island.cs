using UnityEngine;
using UnityEngine.Events;

public class Island : MonoBehaviour
{
    [SerializeField] private int id = -1;
    [SerializeField] private string islandName = "Island";
    [SerializeField] private Sprite icon;
    [SerializeField] private bool isHovered = false;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private IslandType type = IslandType.DEFAULT;
    [SerializeField] private int difficulty = 0;
    [SerializeField] private IslandResources resources;

    // Colors
    [SerializeField] private Color normalEmission = Color.black;
    [SerializeField] private Color hoveredEmission = new Color(0.3f, 0.3f, 0.3f);
    // Events
    public UnityEvent<int> onSelect; // trying this vs saving through manager

    public int ID
    {
        get { return id; }
    }
    public string Name
    {
        get { return islandName; }
    }
    public Sprite Icon
    {
        get { return icon; }
    }
    public int Difficulty
    {
        get { return difficulty; }
    }
    public string Resources
    {
        get { return resources.ToString(); }
    }
    public IslandType Type
    {
        get { return type; }
    }

    private void Awake()
    {
        if(meshRenderer == null) { meshRenderer = GetComponent<MeshRenderer>(); }
        id = gameObject.GetInstanceID();
        isHovered = false;
    }
    private void Update()
    {
        if(isHovered)
        {
            if(Input.GetMouseButtonDown(0))
            {
                HandleSelection();
            }
        }
    }
    private void OnMouseEnter()
    {
        meshRenderer.material.SetColor("_EmissionColor", hoveredEmission);
        isHovered = true;
    }
    private void OnMouseExit()
    {
        meshRenderer.material.SetColor("_EmissionColor", normalEmission);
        isHovered = false;
    }

    private void HandleSelection()
    {
        onSelect.Invoke(id);
    }
}
