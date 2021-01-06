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

        //polygons = new List<Polygon>();
        polygons = new List<Polygon>() // For Base Form Reference
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

        // We choose 6 because there are 6 directions of the base form, so we want the vertices after that.
        // Here we are subdividing one face of the base form octahedron.
        SubdivideFace((int)Dir.UP, (int)Dir.SOUTH, (int)Dir.EAST, 1);

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
        SubdivideFace(top, index + 1, index,            n - 1);
        SubdivideFace(index + 1, bottomLeft, index + 2, n - 1);
        SubdivideFace(index + 1, index + 2, index,      n - 1);
        SubdivideFace(index, index + 2, bottomRight,    n - 1);
    }

    /*
     * Gets the midpoint vertex between two vertices.
     */
    private Vector3 GetMidPointVertex(int a, int b, Color c) 
    {
        // Calculate midpoint
        Vector3 val = (vertices[a] + vertices[b]) / 2;
        //val += new Vector3(0, 0.1f, 0);

        // DEBUG
        /*var obj = Instantiate(debugPoint, val, Quaternion.identity);
        obj.GetComponent<Renderer>().material.color = c;*/

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