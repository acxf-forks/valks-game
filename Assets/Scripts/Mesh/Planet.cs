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
    public Transform debugPoint;
    public int r = 2;

    private Mesh mesh;

    private List<Vector3> vertices;
    private List<Polygon> polygons;

    public enum Dir { UP, DOWN, EAST, WEST, NORTH, SOUTH }

    public Vector3 GetVertex(int index) 
    {
        return vertices[index];
    }

    public int GetIndex(Planet.Dir dir)
    {
        return (int)dir;
    }

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    private void Start()
    {
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
            new Polygon(Dir.EAST,  Dir.UP,   Dir.NORTH),
            new Polygon(Dir.NORTH, Dir.UP,   Dir.WEST),
            new Polygon(Dir.SOUTH, Dir.UP,   Dir.EAST),
            new Polygon(Dir.WEST,  Dir.UP,   Dir.SOUTH),
            new Polygon(Dir.NORTH, Dir.DOWN, Dir.EAST),
            new Polygon(Dir.WEST,  Dir.DOWN, Dir.NORTH),
            new Polygon(Dir.EAST,  Dir.DOWN, Dir.SOUTH),
            new Polygon(Dir.SOUTH, Dir.DOWN, Dir.WEST) 
        };

        SubdivideFace((int)Dir.UP, (int)Dir.SOUTH, (int)Dir.EAST);

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

    private void SubdivideFace(int top, int bottomLeft, int bottomRight)
    {
        vertices.Add(GetMidPointVertex(top, bottomRight, new Color(1, 0, 0)));    // R (6)
        vertices.Add(GetMidPointVertex(top, bottomLeft, new Color(0, 1, 0)));   // G (7)
        vertices.Add(GetMidPointVertex(bottomLeft, bottomRight, new Color(0, 0, 1))); // B (8)

        // G R
        //  B

        int i = 6;

        polygons.Add(new Polygon(top, i, i + 1)); // Upper
        polygons.Add(new Polygon(i + 1, i + 2, bottomLeft)); // Lower Left
        polygons.Add(new Polygon(i + 1, i, i + 2)); // Lower Mid
        polygons.Add(new Polygon(i, bottomRight, i + 2)); // Lower Right
    }

    /*
     * Gets the midpoint vertex between two vertices.
     */
    private Vector3 GetMidPointVertex(int a, int b, Color c) 
    {
        // Calculate midpoint
        Vector3 val = (vertices[a] + vertices[b]) / 2;

        // DEBUG
        var obj = Instantiate(debugPoint, val, Quaternion.identity);
        obj.GetComponent<Renderer>().material.color = c;

        return val;
    }
}

public class Polygon 
{
    public List<int> vertices;

    public Polygon(Planet.Dir a, Planet.Dir b, Planet.Dir c) 
    {
        vertices = new List<int>() { (int)a, (int)b, (int)c };
    }

    public Polygon(int a, int b, int c)
    {
        vertices = new List<int>() { a, b, c };
    }
}