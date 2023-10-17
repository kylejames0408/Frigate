using UnityEngine;

// Components that all buildings share
[CreateAssetMenu(fileName = "BuildingComponents", menuName = "Libertatia/BuildingComponents", order = 2)]
public class BuildingComponents : ScriptableObject
{
    // Materials
    public Material placingMaterial;
    public Material collisionMaterial;
    public Material needsAssignmentMaterial;
    public Material buildingMaterial;
    // Triggerables
    public ParticleSystem buildParticle;
    public ParticleSystem finishParticle;
}
