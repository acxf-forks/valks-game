using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    public SphereSettings sphereSettings;
    private SphereMeshChunkRenderer planetChunkRenderer;
    private static int planetCount = 0;

    private void Awake()
    {
        planetCount++;
        planetChunkRenderer = new SphereMeshChunkRenderer(this, sphereSettings);
    }

    private void Start()
    {
        gameObject.name = $"({planetCount}) Planet - " + sphereSettings.name;
    }

    private void Update()
    {
        planetChunkRenderer.RenderNearbyChunks(sphereSettings.renderRadius);
    }
}
