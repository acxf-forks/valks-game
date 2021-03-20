using System.Collections;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public PlanetSettings planetSettings;
    public SphereSettings sphereSettings;

    private static int planetCount = 0;
    private SphereMeshChunkRenderer sphere;

    private void Awake()
    {
        planetCount++;
    }

    private void Start()
    {
        gameObject.name = $"({planetCount}) Planet - " + planetSettings.name;

        sphere = new SphereMeshChunkRenderer(transform, sphereSettings);
    }

    private void Update()
    {
        sphere.RenderNearbyChunks(sphereSettings.renderRadius);
    }
}