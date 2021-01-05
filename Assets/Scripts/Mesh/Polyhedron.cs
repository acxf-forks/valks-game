using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Geodesic polyhedron are represented by geodesic notation in the form
 * {3,q+}b,c where 3 represents the number of vertices for one face 
 * (a triangle), q represents the number of valence vertices from a center
 * vertex. The + symbol indicates the valence of the vertices being increased.
 * b,c represent the subdivision description with 1,0 representing base form.
 * 
 * OCTAHEDRAL CALCULATIONS
 * Triangles(T) = b^2
 * Vertices     = 4T + 2
 * Faces        = 8T
 * Edges        = 12T
 * 
 * Reference https://en.wikipedia.org/wiki/Geodesic_polyhedron
 */

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

        // {3,4}1,0

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
