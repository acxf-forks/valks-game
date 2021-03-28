using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    public ShapeSettings shapeSettings;
    public MinMax elevationMinMax;


	[Range(0.1f,5)]
	public float amplitude = 2;
    private INoiseFilter[] noiseFilters;

    public void UpdateSettings(ShapeSettings _shapeSettings) 
    {
        shapeSettings = _shapeSettings;

        noiseFilters = new INoiseFilter[shapeSettings.noiseLayers.Length];
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(shapeSettings.noiseLayers[i].noiseSettings);
        }

        elevationMinMax = new MinMax();
    }

    public float CalculateAdditionalElevation(Vector3 pointOnUnitSphere)
    {
        float elevation = 0;

        for (int i = 0; i < noiseFilters.Length; i++)
        {
            if (shapeSettings.noiseLayers[i].enabled)
            {
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere);
            }
        }
		//elevationMinMax.AddValue(elevation);
        return elevation;
    }
}
