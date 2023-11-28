using UnityEngine;

// TODO:
// - Outline/walkway
// - Building size is used for offsets - maybe collision
// - Make editor buttons - generate paint & reset
// - extra - add drag drop or dynamic placement

public class TerrainPainter : MonoBehaviour
{
    public float[,,] splat;
    public float[,,] alphaMap;
    public float[,,] originalAlphaMap;
    public GameObject[] buildings;
    public Terrain terrain;
    public BuildingManager bm;

    private void Awake()
    {
        if(terrain == null)
        {
            terrain = GetComponent<Terrain>();
        }
        originalAlphaMap = terrain.terrainData.GetAlphamaps(0,0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            UpdateTerrain(buildings);
        }
    }

    public void UpdateTerrain(GameObject[] buildings)
    {
        float scaleWidth = terrain.terrainData.alphamapWidth/terrain.terrainData.size.x;
        float scaleDepth = terrain.terrainData.alphamapHeight/terrain.terrainData.size.z;

        float buildingWidth = 10.0f;
        float buildingDepth = 10.0f;
        Vector3 buildingPosition = buildings[0].transform.position;
        Vector3 terrainPosition = terrain.GetPosition();
        Vector3 relativeBuildingPosition = buildingPosition - terrainPosition;

        int sampleWidth = (int)(20.0f * scaleWidth);
        int sampleDepth = (int)(20.0f * scaleDepth);

        int mapX = (int)(scaleWidth * (relativeBuildingPosition.x - buildingWidth));
        int mapZ = (int)(scaleDepth * (relativeBuildingPosition.z - buildingDepth));

        // The X/Z doesnt really matter since we are just overwriting the data obtained. We can technically just create a new one
        alphaMap = terrain.terrainData.GetAlphamaps(mapX, mapZ, sampleWidth, sampleDepth);
        for (int y = 0; y < sampleWidth; y++)
        {
            for (int x = 0; x < sampleDepth; x++)
            {
                alphaMap[x, y, 0] = 0.0f;
                alphaMap[x, y, 1] = 1.0f;
            }
        }
        terrain.terrainData.SetAlphamaps(mapX, mapZ, alphaMap);
    }
    public void UpdateTerrain()
    {
        Building[] buildings = bm.Buildings;
        Vector3 buildingPosition = buildings[0].transform.position;
        Vector3 relativeBuildingPosition = buildingPosition - terrain.GetPosition();
        alphaMap = terrain.terrainData.GetAlphamaps((int)relativeBuildingPosition.x, (int)relativeBuildingPosition.z, 10, 10);
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                alphaMap[x, y, 0] = 1.0f;
            }
        }
        terrain.terrainData.SetAlphamaps((int)relativeBuildingPosition.x, (int)relativeBuildingPosition.z, alphaMap);


        //splat = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, 2];
        //for (int y = 0; y < terrain.terrainData.alphamapHeight; y++)
        //{
        //    for (int x = 0; x < terrain.terrainData.alphamapWidth; x++)
        //    {
        //        // Get the normalized terrain coordinate that corresponds to the point.
        //        float normX = x * 1.0f / (terrain.terrainData.alphamapWidth - 1);
        //        float normY = y * 1.0f / (terrain.terrainData.alphamapHeight - 1);

        //        // Get the steepness value at the normalized coordinate.
        //        var angle = terrain.terrainData.GetSteepness(normX, normY);

        //        // Steepness is given as an angle, 0..90 degrees. Divide
        //        // by 90 to get an alpha blending value in the range 0..1.
        //        var frac = angle / 90.0;
        //        splat[x, y, 0] = (float)frac;
        //        splat[x, y, 1] = (float)(1 - frac);
        //    }
        //}

        //terrain.terrainData.SetAlphamaps(0, 0, splat);
    }

    private void OnDestroy()
    {
        terrain.terrainData.SetAlphamaps(0, 0, originalAlphaMap);
    }
}
