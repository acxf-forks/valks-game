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
 * OCTAHEDRAL CALCULATIONS (for one face of the base form)
 * Triangles(T) = b^2
 * Vertices     = 4T + 2
 * Faces        = 8T
 * Edges        = 12T
 * 
 * Reference https://en.wikipedia.org/wiki/Geodesic_polyhedron
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
        int r = 2;

        /*
         * (+x) = East, (+z) = North, (+y) = Up
         * (-x) = West, (-z) = South, (-y) = Down
         */

        vertices = new List<Vector3>() 
        {
            new Vector3(0, r, 0),  // UP    (0)
            new Vector3(0, -r, 0), // DOWN  (1)
            new Vector3(r, 0, 0),  // EAST  (2)
            new Vector3(-r, 0, 0), // WEST  (3)
            new Vector3(0, 0, r),  // NORTH (4)
            new Vector3(0, 0, -r)  // SOUTH (5)
        };

        /*
         * Creating the base form ({3,4+}1,0) of a geodesic octahedron can be tedious.
         * Drawing a picture on paper should help.
         * 
         * Note that the lower is the same as the upper except the first 
         * and third vertices are swapped and the second vertex is made 
         * negative. 
         * 
         * Recall: Drawing triangles clockwise will show their fronts while
         * drawing them counter-clockwise will show their rears.
         */

        polygons = new List<Polygon>() 
        {
            new Polygon(2, 0, 4), // East Up North
            new Polygon(4, 0, 3), // West Up North
            new Polygon(5, 0, 2), // East Up South
            new Polygon(3, 0, 5), // West Up South
            new Polygon(4, 1, 2), // East Down North
            new Polygon(3, 1, 4), // West Down North
            new Polygon(2, 1, 5), // East Down South
            new Polygon(5, 1, 3)  // West Down South
        };

        mesh.Clear(); // Remove any previous mesh data
        mesh.vertices = vertices.ToArray();

        List<int> triangles = new List<int>();
        for (int i = 0; i < polygons.Count; i++) 
            for (int j = 0; j < polygons[i].vertices.Count; j++) 
                triangles.Add(polygons[i].vertices[j]);

        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
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