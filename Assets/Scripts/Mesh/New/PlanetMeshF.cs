using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMeshF
{
    private List<Vector3> vertices;

    public void Create(PlanetSettingsF settings)
    {
        var radius = settings.radius;
        var t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        vertices = new List<Vector3>()
        {
            new Vector3(-1, t, 0).normalized * radius,
            new Vector3(1, t, 0).normalized * radius ,
            new Vector3(-1, -t, 0).normalized * radius,
            new Vector3(1, -t, 0).normalized * radius,
            new Vector3(0, -1, t).normalized * radius,
            new Vector3(0, 1, t).normalized * radius ,
            new Vector3(0, -1, -t).normalized * radius,
            new Vector3(0, 1, -t).normalized * radius,
            new Vector3(t, 0, -1).normalized * radius,
            new Vector3(t, 0, 1).normalized * radius ,
            new Vector3(-t, 0, -1).normalized * radius,
            new Vector3(-t, 0, 1).normalized * radius
        };

        GenerateChunks();
    }

    private void GenerateChunks() 
    {
        /*0, 5, 11,
        0, 1, 5, 
        0, 7, 1, 
        0, 10, 7,
        0, 11, 10
        1, 9, 5, 
        5, 4, 11,
        11, 2, 10
        10, 6, 7,
        7, 8, 1, 
        3, 4, 9, 
        3, 2, 4, 
        3, 6, 2, 
        3, 8, 6, 
        3, 9, 8, 
        4, 5, 9, 
        2, 11, 4,
        6, 10, 2,
        8, 7, 6, 
        9, 1, 8, */
    }
}
