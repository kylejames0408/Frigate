using System;
using UnityEditor;
using UnityEngine;

public class TerrainManager
{
    public event EventHandler<ProgressEventArgs> Progress;
    public event EventHandler<EventArgs> Complete;

    public void Split(Terrain sourceTerrain, ChunkData chunkData, int divider)
    {
        int chunkCount = divider * divider;

        for (int i = 0; i < chunkCount; i++)
        {
            Progress?.Invoke(this, new ProgressEventArgs($"Spliting {sourceTerrain.name}", $"Chunk {i}", (float)i / chunkCount));

            TerrainData tData = new TerrainData();
            GameObject chunk = Terrain.CreateTerrainGameObject(tData);
            Terrain terrain = chunk.GetComponent<Terrain>();
            
            string name = GetName(divider, i);
            AssetDatabase.CreateAsset(tData, $"{chunkData.Path}/{name}.asset");

            CloneTerrainData(sourceTerrain.terrainData, ref tData);
            CloneTerrain(sourceTerrain, ref terrain);

            chunk.name = name;
            chunk.transform.position = GetPosition(sourceTerrain, divider, i);

            tData.heightmapResolution = chunkData.HeigthmapResolution;
            tData.alphamapResolution = chunkData.ControlTextureResolution;
            tData.SetDetailResolution(chunkData.DetailResolution, chunkData.DetailResolutioinPerPatch);
            tData.baseMapResolution = chunkData.BaseTextureResolution;
            tData.size = new Vector3(chunkData.Width, chunkData.Height, chunkData.Length);


            terrain.terrainData.SetHeightsDelayLOD(0, 0, GetHeight(sourceTerrain.terrainData, divider, i));
            terrain.terrainData.SetAlphamaps(0, 0, GetSplat(sourceTerrain.terrainData, divider, i));
            for (int detailLayer = 0; detailLayer < sourceTerrain.terrainData.detailPrototypes.Length; detailLayer++)
            {
                terrain.terrainData.SetDetailLayer(0, 0, detailLayer, GetDetail(sourceTerrain.terrainData, divider, detailLayer, i));
            }
            for (int treeIndex = 0; treeIndex < sourceTerrain.terrainData.treeInstanceCount; treeIndex++)
            {
                var tree = sourceTerrain.terrainData.treeInstances[treeIndex];
                GetTree(ref terrain, sourceTerrain.terrainData, tree, divider, i);
            }
            AssetDatabase.SaveAssets();
        }
        Complete?.Invoke(this, null);
    }
    /// <summary>
    /// Создаёт клон Terrain
    /// </summary>
    /// <param name="sourceTerrain">Исходный Terrain</param>
    /// <returns>Новый Terrain</returns>
    private void CloneTerrain(Terrain sourceTerrain, ref Terrain destinationTerrain)
        {
            destinationTerrain.bakeLightProbesForTrees     = sourceTerrain.bakeLightProbesForTrees;
            destinationTerrain.basemapDistance             = sourceTerrain.basemapDistance;
            //destinationTerrain.castShadows                 = sourceTerrain.castShadows;
            destinationTerrain.shadowCastingMode           = sourceTerrain.shadowCastingMode;
            destinationTerrain.collectDetailPatches        = sourceTerrain.collectDetailPatches;
            destinationTerrain.detailObjectDensity         = sourceTerrain.detailObjectDensity;
            destinationTerrain.detailObjectDistance        = sourceTerrain.detailObjectDistance;
            destinationTerrain.drawHeightmap               = sourceTerrain.drawHeightmap;
            destinationTerrain.drawTreesAndFoliage         = sourceTerrain.drawTreesAndFoliage;
            destinationTerrain.editorRenderFlags           = sourceTerrain.editorRenderFlags;
            destinationTerrain.heightmapMaximumLOD         = sourceTerrain.heightmapMaximumLOD;
            destinationTerrain.heightmapPixelError         = sourceTerrain.heightmapPixelError;
            destinationTerrain.legacyShininess             = sourceTerrain.legacyShininess;
            destinationTerrain.legacySpecular              = sourceTerrain.legacySpecular;
            destinationTerrain.lightmapIndex               = sourceTerrain.lightmapIndex;
            destinationTerrain.lightmapScaleOffset         = sourceTerrain.lightmapScaleOffset;
            destinationTerrain.materialTemplate            = sourceTerrain.materialTemplate;
            destinationTerrain.materialType                = sourceTerrain.materialType;
            destinationTerrain.realtimeLightmapIndex       = sourceTerrain.realtimeLightmapIndex;
            destinationTerrain.realtimeLightmapScaleOffset = sourceTerrain.realtimeLightmapScaleOffset;
            destinationTerrain.reflectionProbeUsage        = sourceTerrain.reflectionProbeUsage;
            destinationTerrain.treeBillboardDistance       = sourceTerrain.treeBillboardDistance;
            destinationTerrain.treeCrossFadeLength         = sourceTerrain.treeCrossFadeLength;
            destinationTerrain.treeDistance                = sourceTerrain.treeDistance;
            destinationTerrain.treeMaximumFullLODCount     = sourceTerrain.treeMaximumFullLODCount;
    }
    /// <summary>
    /// Создаёт клон TerrainData
    /// </summary>
    /// <param name="sourceTerrainData"></param>
    /// <returns>Новый Terrain</returns>
    private void CloneTerrainData(TerrainData sourceTerrainData, ref TerrainData destinationTerrainData)
    {
        //destinationTerrainData.splatPrototypes = sourceTerrainData.splatPrototypes;
        destinationTerrainData.terrainLayers = sourceTerrainData.terrainLayers;
        destinationTerrainData.detailPrototypes = sourceTerrainData.detailPrototypes;
        destinationTerrainData.treePrototypes = sourceTerrainData.treePrototypes;
    }
    private string GetName(int divider, int index)
    {
        float xWShift = index % divider;
        float zWShift = index / divider;

        string name = $"chunk {xWShift} - 0 - {zWShift}";

        return name;
    }
    /// <summary>
    /// Возвращает позицию чанка
    /// </summary>
    /// <param name="parentTerrain"></param>
    /// <param name="divider"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private Vector3 GetPosition(Terrain parentTerrain, int divider, int index)
    {
        var parentPosition = parentTerrain.GetPosition();
        var chunkPosition = Vector3.zero;

        float spaceShiftX = parentTerrain.terrainData.size.z / divider;
        float spaceShiftZ = parentTerrain.terrainData.size.x / divider;

        float xWShift = (index % divider) * spaceShiftX;
        float zWShift = (index / divider) * spaceShiftZ;

        chunkPosition = new Vector3(chunkPosition.x + zWShift, chunkPosition.y, chunkPosition.z + xWShift);

        // Shift last position
        chunkPosition = new Vector3(chunkPosition.x + parentPosition.x, chunkPosition.y + parentPosition.y, chunkPosition.z + parentPosition.z);

        return chunkPosition;
    }
    /// <summary>
    /// Возвращает двумерный массив карты высот
    /// </summary>
    /// <param name="terrainData"></param>
    /// <param name="divider"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private float[,] GetHeight(TerrainData terrainData, int divider, int index)
    {
        float[,] sourceHeight = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
        float[,] destinationHeight = new float[terrainData.heightmapResolution / divider + 1, terrainData.heightmapResolution / divider + 1];

        // Shift calc
        int heightShift = terrainData.heightmapResolution / divider;

        int endX = terrainData.heightmapResolution / divider + 1;
        int endY = terrainData.heightmapResolution / divider + 1;

        int xShift = (index % divider) * heightShift;
        int yShift = (index / divider) * heightShift;

        for (int x = 0; x < endX; x++) {
            for (int y = 0; y < endY; y++) {
                destinationHeight[x, y] = sourceHeight[x + xShift, y + yShift];
            }
        }

        return destinationHeight;
    }
    /// <summary>
    /// Возвращает трехмерный массив Alphamaps
    /// </summary>
    /// <param name="terrainData"></param>
    /// <param name="divider"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private float[,,] GetSplat(TerrainData terrainData, int divider, int index)
    {
        float[,,] sourceSplat = terrainData.GetAlphamaps(0, 0, terrainData.alphamapResolution, terrainData.alphamapResolution);
        float[,,] destinationSplat = new float[terrainData.alphamapResolution / divider, terrainData.alphamapResolution / divider, terrainData.alphamapLayers];

        // Shift calc
        int splatShift = terrainData.alphamapResolution / divider;

        int endX = terrainData.alphamapResolution / divider;
        int endY = terrainData.alphamapResolution / divider;

        int xShift = (index % divider) * splatShift;
        int yShift = (index / divider) * splatShift;

        for (int s = 0; s < terrainData.alphamapLayers; s++) {
            for (int x = 0; x < endX; x++) {
                for (int y = 0; y < endY; y++) {
                    destinationSplat[x, y, s] = sourceSplat[x + xShift, y + yShift, s];
                }
            }
        }

        return destinationSplat;
    }
    /// <summary>
    /// Возвращает двумерный массив Detail
    /// </summary>
    /// <param name="terrainData"></param>
    /// <param name="divider"></param>
    /// <param name="layer"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private int[,] GetDetail(TerrainData terrainData, int divider, int layer, int index)
    {
        int[,] sourceDetail = terrainData.GetDetailLayer(0, 0, terrainData.detailResolution, terrainData.detailResolution, layer);
        int[,] destinationDetail = new int[terrainData.detailResolution / divider, terrainData.detailResolution / divider];

        // Shift calc
        int detailShift = terrainData.detailResolution / divider;

        int endX = terrainData.detailResolution / divider;
        int endY = terrainData.detailResolution / divider;

        int xShift = (index % divider) * detailShift;
        int yShift = (index / divider) * detailShift;

        for (int x = 0; x < endX; x++) {
            for (int y = 0; y < endY; y++) {
                destinationDetail[x, y] = sourceDetail[x + xShift, y + yShift];
            }
        }

        return destinationDetail;
    }

    private void GetTree(ref Terrain terrain, TerrainData terrainData, TreeInstance tree, int divider, int index)
    {
        float spaceShiftX = terrainData.size.x / divider;
        float spaceShiftZ = terrainData.size.z / divider;

        float xShiftMin = (index % divider) * spaceShiftX;
        float zShiftMin = (index / divider) * spaceShiftZ;

        float xShiftMax = (xShiftMin + spaceShiftX) / terrainData.size.x;
        float zShiftMax = (zShiftMin + spaceShiftZ) / terrainData.size.z;

        xShiftMin /= terrainData.size.x;
        zShiftMin /= terrainData.size.z;

        float xMin = terrain.terrainData.size.x / terrainData.size.x;
        float xMax = (terrainData.size.x - terrain.terrainData.size.x) / terrainData.size.x;
        float zMin = terrain.terrainData.size.z / terrainData.size.z;
        float zMax = (terrainData.size.z - terrain.terrainData.size.z) / terrainData.size.z;

        float xWShift = (index % divider) * spaceShiftX;
        float zWShift = (index / divider) * spaceShiftZ;

        if (tree.position.x < zShiftMin || tree.position.x >= zShiftMax)
            return;

        if (tree.position.z < xShiftMin || tree.position.z >= xShiftMax)
            return;

        tree.position = new Vector3((tree.position.x - zShiftMin) * divider, tree.position.y, (tree.position.z - xShiftMin) * divider);

        terrain.AddTreeInstance(tree);
    }
}
