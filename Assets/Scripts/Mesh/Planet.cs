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
    public Transform debugPoint;
    private Mesh mesh;

    private List<Vector3> vertices;
    private List<Polygon> polygons;

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    private void Start()
    {
        GenerateMesh();

        // DEBUG
        for (int i = 0; i < polygons.Count; i++)
        {
            var cube = Instantiate(debugPoint, polygons[i].GetCenterVertex(), Quaternion.identity);

            float color = (i / (float)polygons.Count);

            cube.GetComponent<Renderer>().material.color = new Color(color, 0, 0);
            cube.rotation = Quaternion.LookRotation(Vector3.zero - cube.position);
            cube.position = Vector3.MoveTowards(cube.position, Vector3.zero, -0.05f);
        }
    }

    private void GenerateMesh() 
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

        int subdivisions = 2;

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

        // GENERATE MESH
        mesh.Clear(); // Remove any previous mesh data
        mesh.vertices = vertices.ToArray();

        List<int> triangles = new List<int>();
        for (int i = 0; i < polygons.Count; i++)
            for (int j = 0; j < polygons[i].indices.Count; j++)
                triangles.Add(polygons[i].indices[j]);

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

        vertices.Add(GetMidPointVertex(top, bottomRight));       
        vertices.Add(GetMidPointVertex(top, bottomLeft));        
        vertices.Add(GetMidPointVertex(bottomLeft, bottomRight));

        // G R
        //  B

        // We subtract 3 because it's like subtracting 1 polygon because we need to account for 0 index
        int index = vertices.Count - 3;

        // Only draw the last subdivision.
        if (n == 1)
        {
            polygons.Add(new Polygon(vertices[top], vertices[index], vertices[index + 1], top, index, index + 1)); // Upper Top
            polygons.Add(new Polygon(vertices[index + 1], vertices[index + 2], vertices[bottomLeft], index + 1, index + 2, bottomLeft)); // Lower Left
            polygons.Add(new Polygon(vertices[index + 1], vertices[index], vertices[index + 2], index + 1, index, index + 2)); // Lower Mid
            polygons.Add(new Polygon(vertices[index], vertices[bottomRight], vertices[index + 2], index, bottomRight, index + 2)); // Lower Right
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
    private Vector3 GetMidPointVertex(int a, int b)
    {
        // Calculate midpoint
        Vector3 midpoint = (vertices[a] + vertices[b]) / 2;

        // Normalized will project the midpoints onto a icosphere (with a radius of 1)
        return midpoint.normalized;
    }
}

public class Polygon
{
    public List<Vector3> vertices;
    public List<int> indices;

    public Polygon(Vector3 vA, Vector3 vB, Vector3 vC, int a, int b, int c)
    {
        vertices = new List<Vector3>() { vA, vB, vC };
        indices = new List<int>() { a, b, c };
    }

    /*
     * Calculate the center point of polygon
     */
    public Vector3 GetCenterVertex()
    {
        return ((vertices[0] + vertices[1] + vertices[2]) / 3).normalized;
    }
}