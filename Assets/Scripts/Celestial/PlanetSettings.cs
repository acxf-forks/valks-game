using System.Collections;
using UnityEngine;

[CreateAssetMenu()]
public class PlanetSettings : ScriptableObject
{
    public string planetName;
    [TextArea]
    public string description;

    public float treeDensity = 0.2f;
}