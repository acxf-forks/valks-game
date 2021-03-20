using System.Collections;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public PlanetSettings planetSettings;
    public SphereSettings terrainSphereSettings;
    public SphereSettings waterSphereSettings;

    private static int planetCount = 0;
    private SphereMeshChunkRenderer planetSphere;
    private SphereMeshChunkRenderer waterSphere;

    public void Create()
    {
        gameObject.name = $"({planetCount++}) Planet - " + planetSettings.name;

        var parentTerrainChunks = new GameObject("Terrain Chunks").transform;
        parentTerrainChunks.parent = transform;

        var parentWaterChunks = new GameObject("Water Chunks").transform;
        parentWaterChunks.parent = transform;

        planetSphere = new SphereMeshChunkRenderer(parentTerrainChunks, terrainSphereSettings);
        waterSphere = new SphereMeshChunkRenderer(parentWaterChunks, waterSphereSettings);
    }

    private void Update()
    {
        planetSphere.RenderNearbyChunks(terrainSphereSettings.renderRadius);
        waterSphere.RenderNearbyChunks(waterSphereSettings.renderRadius);
    }
}