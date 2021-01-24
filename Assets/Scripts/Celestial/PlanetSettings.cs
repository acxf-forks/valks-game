using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlanetSettings
{
    public string name;

    [Range(1, 20)]
    public int radius = 10;

    [Tooltip("The number of chunk recursions per base face.")]
    [Range(1, 3)]
    public int chunks = 1;

    [Tooltip("The material of the planet.")]
    public Material material;
}
