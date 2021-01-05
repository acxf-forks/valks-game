using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Polyhedron : MonoBehaviour
{
    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    private void Start()
    {
        /*
         * Drawing a triangle clockwise exposes the front while
         * counter-clockwise exposes the back.
         */

        vertices = new Vector3[] {
            // UPPER
            // (+x)(+z) face
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 1),

            // (-x)(+z) face
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 0),
            new Vector3(-1, 0, 0),

            // (x)(-z) face
            new Vector3(0, 0, -1),
            new Vector3(0, 1, 0),
            new Vector3(1, 0, 0),

            // (-x)(-z) face
            new Vector3(-1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, -1),

            /*
             * Note that the lower is the same as the upper except the first 
             * and third vertices are swapped and the second vertex is made 
             * negative. 
             * 
             * Recall: Drawing triangles clockwise will show their fronts.
             */

            // LOWER
            // (+x)(+z) face
            new Vector3(0, 0, 1),
            new Vector3(0, -1, 0),
            new Vector3(1, 0, 0),

            // (-x)(+z) face
            new Vector3(-1, 0, 0),
            new Vector3(0, -1, 0),
            new Vector3(0, 0, 1),

            // (x)(-z) face
            new Vector3(1, 0, 0),
            new Vector3(0, -1, 0),
            new Vector3(0, 0, -1),

            // (-x)(-z) face
            new Vector3(0, 0, -1),
            new Vector3(0, -1, 0),
            new Vector3(-1, 0, 0)
        };

        triangles = new int[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
            triangles[i] = i;

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
