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
    private List<Rigidbody> attractedEntities = new List<Rigidbody>();

    public string planetName;
    public int radius = 2;
    public float gravity = -10f;

    public Transform debugPoint;
    private Mesh mesh;

    private List<Vector3> vertices;
    private List<int> triangles;

    private static int planetCount = 0;
    
    public Vector3[] centerVertices;

    private void Awake()
    {
        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Texture");
        mesh = GetComponent<MeshFilter>().mesh;
        triangles = new List<int>();
    }

    private void Start()
    {
        GenerateMesh();

        var collider = gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = mesh;

        // Get center vertices.
        centerVertices = new Vector3[(triangles.Count / 3)];

        for (int i = 0; i < triangles.Count; i+=3) 
        {
            var centerVertex = GetCenterVertex(triangles[i], triangles[i + 1], triangles[i + 2]);

            // DEBUG
            /*var cube = Instantiate(debugPoint, centerVertex, Quaternion.identity);

            var dir = (centerVertex - Vector3.zero).normalized;
            cube.forward = dir;
            cube.position = centerVertex + (dir * 0.05f);*/

            centerVertices[i / 3] = centerVertex;
        }

        gameObject.layer = LayerMask.NameToLayer("Planets");
        gameObject.name = $"({++planetCount}) Planet - " + planetName;
    }

    private void FixedUpdate()
    {
        /*foreach (var entity in attractedEntities) 
        {
            Attract(entity);
        }*/
    }

    public void AddAttractedEntity(Rigidbody rb) 
    {
        attractedEntities.Add(rb);
    }

    private void Attract(Rigidbody rb) 
    {
        Transform t = rb.transform;
        Vector3 gravityUp = (t.transform.position - transform.position).normalized;
        // Aligns to planets surface
        //Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * transform.rotation;
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 50 * Time.deltaTime);
        
        rb.AddForce(gravityUp * gravity);

        // DEBUG: Draw visual
        Debug.DrawRay(t.position, t.forward * 10f, Color.green);
        Debug.DrawLine(transform.position, t.position, Color.blue);
    }

    private void GenerateMesh() 
    {
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

        var subdivisions = 2;

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
            triangles.AddRange(new List<int> { top, index, index + 1 }); // Upper Top
            triangles.AddRange(new List<int> { index + 1, index + 2, bottomLeft }); // Lower Left
            triangles.AddRange(new List<int> { index + 1, index, index + 2 }); // Lower Mid
            triangles.AddRange(new List<int> { index, bottomRight, index + 2 }); // Lower Right
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
        var midpoint = (vertices[a] + vertices[b]) / 2;

        // Normalized will project the midpoints onto a icosphere (with a radius of 1)
        return midpoint.normalized * radius;
    }

    public Vector3 GetCenterVertex(int a, int b, int c) => ((vertices[a] + vertices[b] + vertices[c]) / 3).normalized * radius;
}