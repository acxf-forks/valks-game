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
        planetSphere = new SphereMeshChunkRenderer(transform, terrainSphereSettings);
        waterSphere = new SphereMeshChunkRenderer(transform, waterSphereSettings);
    }

    private void Update()
    {
        planetSphere.RenderNearbyChunks(terrainSphereSettings.renderRadius);
        waterSphere.RenderNearbyChunks(waterSphereSettings.renderRadius);
    }
}