using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ShapeSettings : ScriptableObject
{
    [Range(1, 20)]
    public float radius;
    [Range(2, 128)]
    public int resolution = 10;
    public NoiseSettings[] noiseSettings;
}