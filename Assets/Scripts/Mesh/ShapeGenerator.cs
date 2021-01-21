using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeGenerator
{
    public ShapeSettings shapeSettings;
    INoiseFilter[] noiseFilters;
    public MinMax elevationMinMax;

    public void UpdateSettings(ShapeSettings shapeSettings)
    {
        this.shapeSettings = shapeSettings;

        noiseFilters = new INoiseFilter[shapeSettings.noiseSettings.Length];
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(shapeSettings.noiseSettings[i]);
        }
        elevationMinMax = new MinMax();
    }

    public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        if (noiseFilters.Length > 0)
        {
            if (noiseFilters[0].Enabled)
            {
                firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
                elevation = firstLayerValue;
            }
        }

        for (int i = 1; i < noiseFilters.Length; i++)
        {
            if (noiseFilters[i].Enabled)
            {
                float mask = firstLayerValue;
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }
        
        return elevation;
    }

    public float GetScaledElevation(float unscaledElevation)
    {
        float elevation = Mathf.Max(0, unscaledElevation);
        elevation = shapeSettings.radius * (1 + elevation);
        elevationMinMax.AddValue(elevation);

        return elevation;
    }
}
