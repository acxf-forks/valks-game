using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Icosahedron is the closest you can get to a near perfect uniform
 * tessellation projected onto a icosphere.
 * 
 * Reference 
 * https://en.wikipedia.org/wiki/Geodesic_polyhedron
 */

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Planet : MonoBehaviour
{
    private Mesh mesh;

    private List<Vector3> vertices;
    private List<Polygon> polygons;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    private void Start()
    {
        float t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        vertices = new List<Vector3>()
        {
            new Vector3(-1, t, 0).normalized,
            new Vector3(1, t, 0).normalized,
            new Vector3(-1, -t, 0).normalized,
            new Vector3(1, -t, 0).normalized,
            new Vector3(0, -1, t).normalized,
            new Vector3(0, 1, t).normalized,
            new Vector3(0, -1, -t).normalized,
            new Vector3(0, 1, -t).normalized,
            new Vector3(t, 0, -1).normalized,
            new Vector3(t, 0, 1).normalized,
            new Vector3(-t, 0, -1).normalized,
            new Vector3(-t, 0, 1).normalized
        };

        polygons = new List<Polygon>();

        int subdivisions = 0;

        if (subdivisions == 0)
        {
            polygons.Add(new Polygon(0, 11, 5));
            polygons.Add(new Polygon(0, 5, 1));
            polygons.Add(new Polygon(0, 1, 7));
            polygons.Add(new Polygon(0, 7, 10));
            polygons.Add(new Polygon(0, 10, 11));
            polygons.Add(new Polygon(1, 5, 9));
            polygons.Add(new Polygon(5, 11, 4));
            polygons.Add(new Polygon(11, 10, 2));
            polygons.Add(new Polygon(10, 7, 6));
            polygons.Add(new Polygon(7, 1, 8));
            polygons.Add(new Polygon(3, 9, 4));
            polygons.Add(new Polygon(3, 4, 2));
            polygons.Add(new Polygon(3, 2, 6));
            polygons.Add(new Polygon(3, 6, 8));
            polygons.Add(new Polygon(3, 8, 9));
            polygons.Add(new Polygon(4, 9, 5));
            polygons.Add(new Polygon(2, 4, 11));
            polygons.Add(new Polygon(6, 2, 10));
            polygons.Add(new Polygon(8, 6, 7));
            polygons.Add(new Polygon(9, 8, 1));
        }
        else 
        {
            SubdivideFace(0, 5, 11, subdivisions);
            SubdivideFace(0, 1, 5, subdivisions);
            SubdivideFace(0, 7, 1, subdivisions);
            SubdivideFace(0, 10, 7, subdivisions);
            SubdivideFace(0, 11, 10, subdivisions);
            SubdivideFace(1, 9, 5, subdivisions);
            SubdivideFace(5, 4, 11, subdivisions);
            SubdivideFace(11, 2, 10, subdivisions);
            SubdivideFace(10, 6, 7, subdivisions);
            SubdivideFace(7, 8, 1, subdivisions);
            SubdivideFace(3, 4, 9, subdivisions);
            SubdivideFace(3, 2, 4, subdivisions);
            SubdivideFace(3, 6, 2, subdivisions);
            SubdivideFace(3, 8, 6, subdivisions);
            SubdivideFace(3, 9, 8, subdivisions);
            SubdivideFace(4, 5, 9, subdivisions);
            SubdivideFace(2, 11, 4, subdivisions);
            SubdivideFace(6, 10, 2, subdivisions);
            SubdivideFace(8, 7, 6, subdivisions);
            SubdivideFace(9, 1, 8, subdivisions);
        }

        // GENERATE MESH
        mesh.Clear(); // Remove any previous mesh data
        mesh.vertices = vertices.ToArray();

        List<int> triangles = new List<int>();
        for (int i = 0; i < polygons.Count; i++)
            for (int j = 0; j < polygons[i].vertices.Count; j++)
                triangles.Add(polygons[i].vertices[j]);

        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
    }

    /*
     * Subdivides a specified triangle given the 3 points of the triangle.
     * 
     * top = The top vertex
     * bottomLeft = The bottom left vertex
     * bottomRight = The bottom right vertex
     * index = Keep track of the vertices index
     * nMax = The max amount of subdivisions
     * n = Keeping track of the current subdivision
     */
    private void SubdivideFace(int top, int bottomLeft, int bottomRight, int n)
    {
        if (n == 0) return;

        vertices.Add(GetMidPointVertex(top, bottomRight, new Color(1, 0, 0)));        // R (6)
        vertices.Add(GetMidPointVertex(top, bottomLeft, new Color(0, 1, 0)));         // G (7)
        vertices.Add(GetMidPointVertex(bottomLeft, bottomRight, new Color(0, 0, 1))); // B (8)

        // G R
        //  B

        // We subtract 3 because it's like subtracting 1 polygon because we need to account for 0 index
        int index = vertices.Count - 3;

        // Only draw the last subdivision.
        if (n == 1)
        {
            polygons.Add(new Polygon(top, index, index + 1)); // Upper Top R G
            polygons.Add(new Polygon(index + 1, index + 2, bottomLeft)); // Lower Left
            polygons.Add(new Polygon(index + 1, index, index + 2)); // Lower Mid
            polygons.Add(new Polygon(index, bottomRight, index + 2)); // Lower Right
        }

        /*
         * When you draw a triangle within a triangle it gets turned upside down, so that's
         * why we have to swap the 2nd and 3rd vertices.
         */
        SubdivideFace(top, index + 1, index, n - 1);
        SubdivideFace(index + 1, bottomLeft, index + 2, n - 1);
        SubdivideFace(index + 1, index + 2, index, n - 1);
        SubdivideFace(index, index + 2, bottomRight, n - 1);
    }

    /*
     * Gets the midpoint vertex between two vertices.
     */
    private Vector3 GetMidPointVertex(int a, int b, Color c)
    {
        // Calculate midpoint
        Vector3 midpoint = (vertices[a] + vertices[b]) / 2;

        // Normalized * r will project the midpoints onto a icosphere.
        return midpoint.normalized;
    }
}

public class Polygon
{
    public List<int> vertices;

    public Polygon(int a, int b, int c)
    {
        vertices = new List<int>() { a, b, c };
    }
}