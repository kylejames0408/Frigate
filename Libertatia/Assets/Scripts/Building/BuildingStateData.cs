using UnityEngine;

[CreateAssetMenu(fileName = "BuildingStateData", menuName = "BuildingStateData", order = 1)]
public class BuildingStateData : ScriptableObject
{
    [Header("Placing")]
    public Material matPlacing;
    [Header("Colliding")]
    public Material matColliding;
    [Header("Recruiting")]
    public Sprite iconRecruiting;
    public Material matRecruiting;
    [Header("Constructing")]
    public Sprite iconConstructing;
    public Material matConstructing;
    public ParticleSystem particleConstructing;
    [Header("Built")]
    public Sprite iconBuilt;
    public ParticleSystem particleBuilt;
    [Header("Assigning")]
    public Sprite iconEmptyAsssignment;
}