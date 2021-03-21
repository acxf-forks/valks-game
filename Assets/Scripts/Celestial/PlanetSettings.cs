using System.Collections;
using UnityEngine;

[CreateAssetMenu()]
public class PlanetSettings : ScriptableObject
{
    public string planetName;
    [TextArea]
    public string description;
}