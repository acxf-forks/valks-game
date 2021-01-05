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

public enum Dir { UP, DOWN, EAST, WEST, NORTH, SOUTH }

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
        int r = 2;

        /*
         * (+x) = East, (+z) = North, (+y) = Up
         * (-x) = West, (-z) = South, (-y) = Down
         */

        vertices = new List<Vector3>() 
        {
            new Vector3(0, r, 0),  // UP   
            new Vector3(0, -r, 0), // DOWN 
            new Vector3(r, 0, 0),  // EAST 
            new Vector3(-r, 0, 0), // WEST 
            new Vector3(0, 0, r),  // NORTH
            new Vector3(0, 0, -r)  // SOUTH
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

        mesh.Clear(); // Remove any previous mesh data
        mesh.vertices = vertices.ToArray();

        List<int> triangles = new List<int>();
        for (int i = 0; i < polygons.Count; i++) 
            for (int j = 0; j < polygons[i].vertices.Count; j++) 
                triangles.Add(polygons[i].vertices[j]);

        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();

        GetMidPointVertex(Dir.UP, Dir.EAST,  new Color(1, 0, 0));
        GetMidPointVertex(Dir.UP, Dir.SOUTH, new Color(0, 1, 0));
        GetMidPointVertex(Dir.SOUTH, Dir.EAST, new Color(0, 0, 1));
    }

    /*
        * Gets the midpoint vector between 2 vectors.
        */
    private Vector3 GetMidPointVertex(Dir a, Dir b, Color c) 
    {
        Vector3 val = (vertices[(int)b] - vertices[(int)a]) / 2;
        var obj = Instantiate(debugPoint, val, Quaternion.identity); // Debug
        obj.GetComponent<Renderer>().material.color = c;
        return val;
    }
}

public class Polygon 
{
    public List<int> vertices;

    public Polygon(Dir vertexA, Dir vertexB, Dir vertexC) 
    {
        vertices = new List<int>() { (int)vertexA, (int)vertexB, (int)vertexC };
    }
}