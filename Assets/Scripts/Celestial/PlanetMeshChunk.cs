using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlanetMeshChunk : MonoBehaviour
{
    private List<Vector3> vertices;
    private List<int> triangles;

    private Mesh mesh;

    private static int count = 0;

    public void Create(PlanetMeshChunkRenderer renderer, List<Vector3> vertices) 
    {
        this.vertices = vertices;

        count++;
        gameObject.name = $"Chunk {count}";
        gameObject.SetActive(false);
        GetComponent<MeshRenderer>().material = renderer.settings.material;
        this.vertices = new List<Vector3> { vertices[0], vertices[1], vertices[2] };
        triangles = new List<int>();

        SubdivideFace(0, 1, 2, 2);

        mesh = GetComponent<MeshFilter>().mesh;
        mesh.vertices = this.vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    /*!
     * The number of triangle recursions in this chunk.
     * 
     * @param a - Top
     * @param b - Bottom Right
     * @param c - Bottom Left
     */
    private void SubdivideFace(int top, int bottomRight, int bottomLeft, int n)
    {
        vertices.Add(PlanetUtils.GetMidPointVertex(vertices[top], vertices[bottomRight]));
        vertices.Add(PlanetUtils.GetMidPointVertex(vertices[bottomRight], vertices[bottomLeft]));
        vertices.Add(PlanetUtils.GetMidPointVertex(vertices[bottomLeft], vertices[top]));

        var middleRight = vertices.Count - 3;
        var middleBottom = vertices.Count - 2;
        var middleLeft = vertices.Count - 1;

        // Only draw the last recursion
        if (n == 1)
        {
            triangles.AddRange(new List<int> { top, middleRight, middleLeft }); // Upper Top
            triangles.AddRange(new List<int> { middleLeft, middleBottom, bottomLeft }); // Lower Left
            triangles.AddRange(new List<int> { middleBottom, middleLeft, middleRight }); // Lower Mid
            triangles.AddRange(new List<int> { middleRight, bottomRight, middleBottom }); // Lower Right

            return;
        }

        SubdivideFace(top, middleRight, middleLeft, n - 1);
        SubdivideFace(middleLeft, middleBottom, bottomLeft, n - 1);
        SubdivideFace(middleBottom, middleLeft, middleRight, n - 1);
        SubdivideFace(middleRight, bottomRight, middleBottom, n - 1);
    }

    public Vector3 GetCenterPoint() => PlanetUtils.GetCenterPoint(vertices[0], vertices[1], vertices[2]);
}
