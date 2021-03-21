using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ColourSettings : ScriptableObject
{
    public Gradient terrainGradient;
    public Color deepOceanColour;
    public Color shallowOceanColour;
}
