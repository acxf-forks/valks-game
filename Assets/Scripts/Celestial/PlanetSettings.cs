using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlanetSettings
{
    public string name;

    [Range(1, 20)]
    public int radius = 10;

    [Tooltip("The number of chunks per base face. (Should be a multiple of 4)")]
    [Range(4, 32)]
    public int chunks = 4;

    [Tooltip("The material of the planet.")]
    public Material material;
}
