using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlanetMeshChunk : MonoBehaviour
{
    private List<Vector3> vertices;
    private List<int> triangles;

    private Mesh mesh;

    public static int count = 0;

    public MeshRenderer meshRenderer;

    public MinMax elevationMinMax;
    public Texture2D texture;
    public int textureResolution = 50;

    public void Create(PlanetMeshChunkRenderer _renderer, List<Vector3> _vertices) 
    {
        



        texture = new Texture2D(textureResolution, 1);
        vertices = _vertices;
        elevationMinMax = new MinMax();
        count++;
        gameObject.name = $"Chunk {count}";
        
        meshRenderer = GetComponent<MeshRenderer>();

        if (_renderer.shapeType == PlanetMeshChunkRenderer.ShapeType.Terrain)
            meshRenderer.material = _renderer.settings.terrainMaterial;

        if (_renderer.shapeType == PlanetMeshChunkRenderer.ShapeType.Ocean)
            meshRenderer.material = _renderer.settings.oceanMaterial;

        meshRenderer.receiveShadows = false;
        vertices = new List<Vector3> { _vertices[0], _vertices[1], _vertices[2] };
        triangles = new List<int>();

        SubdivideFace(0, 1, 2, _renderer.settings.chunkTriangles);

        var settings = _renderer.settings;
        var radius = settings.radius;

        for (int i = 0; i < vertices.Count; i++)
        {
            float elevation = 0;
            if (_renderer.shapeType == PlanetMeshChunkRenderer.ShapeType.Terrain)
                elevation = Mathf.Max(0, _renderer.noise.Evaluate(vertices[i] * settings.frequency + settings.center));
            elevation = radius * (1 + elevation + (_renderer.shapeType == PlanetMeshChunkRenderer.ShapeType.Ocean ? settings.oceanDepth : 0));
            elevation -= settings.minValue * -1;
            elevationMinMax.AddValue(elevation);
            vertices[i] = vertices[i].normalized * elevation;
        }

        //meshRenderer.sharedMaterial.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));

        mesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = vertices.Select(s => s.normalized).ToArray();

        gameObject.SetActive(false);
    }

    /*!
     * The number of triangle recursions in this chunk.
     * 
     * @param a - Top
     * @param b - Bottom Right
     * @param c - Bottom Left
     */
    private void SubdivideFace(int _top, int _bottomRight, int _bottomLeft, int n)
    {
        vertices.Add(SphereUtils.GetMidPointVertex(vertices[_top], vertices[_bottomRight]));
        vertices.Add(SphereUtils.GetMidPointVertex(vertices[_bottomRight], vertices[_bottomLeft]));
        vertices.Add(SphereUtils.GetMidPointVertex(vertices[_bottomLeft], vertices[_top]));

        var middleRight = vertices.Count - 3;
        var middleBottom = vertices.Count - 2;
        var middleLeft = vertices.Count - 1;

        // Only draw the last recursion
        if (n == 1)
        {
            triangles.AddRange(new List<int> { _top, middleRight, middleLeft }); // Upper Top
            triangles.AddRange(new List<int> { middleLeft, middleBottom, _bottomLeft }); // Lower Left
            triangles.AddRange(new List<int> { middleBottom, middleLeft, middleRight }); // Lower Mid
            triangles.AddRange(new List<int> { middleRight, _bottomRight, middleBottom }); // Lower Right

            return;
        }

        SubdivideFace(_top, middleRight, middleLeft, n - 1);
        SubdivideFace(middleLeft, middleBottom, _bottomLeft, n - 1);
        SubdivideFace(middleBottom, middleLeft, middleRight, n - 1);
        SubdivideFace(middleRight, _bottomRight, middleBottom, n - 1);
    }

    public Vector3 GetCenterPoint() => SphereUtils.GetCenterPoint(vertices[0], vertices[1], vertices[2]);
}
