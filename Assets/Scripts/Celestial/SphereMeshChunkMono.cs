using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMeshChunkMono : MonoBehaviour
{
    public SphereSettings sphereSettings;
    private SphereMeshChunkRenderer planetChunkRenderer;

    private void Start() 
    {
        planetChunkRenderer = new SphereMeshChunkRenderer(transform, sphereSettings);
    }

    private void Update()
    {
        planetChunkRenderer.RenderNearbyChunks(sphereSettings.renderRadius);
    }
}
