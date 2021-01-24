using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public PlanetSettings planetSettings;
    private static int planetCount = 0;

    private void Awake()
    {
        planetCount++;
    }

    private void Start()
    {
        planetSettings.chunks = ValidateChunkCount(planetSettings.chunks);

        GeneratePlanet();
    }

    private void GeneratePlanet() 
    {
        gameObject.name = $"({planetCount}) Planet - " + planetSettings.name;
        new PlanetMeshChunkGenerator().Create(this, planetSettings);
    }

    /*!
     * 4 triangles can be fit into 1 triangle. Likewise 4 "chunks" can be fit into
     * a "base form face". Therefore the number of chunks should be a multiple of 4
     * unless the chunk count is less than 4 then the chunk count should always be 1.
     * 
     * @return The number of chunks to generate per base face.
     */
    private int ValidateChunkCount(int chunks) => chunks < 4 ? 1 : Mathf.Max(4, chunks / 4 * 4);
}
