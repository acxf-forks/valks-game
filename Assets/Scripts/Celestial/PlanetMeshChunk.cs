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

    public Texture2D texture;
    public int textureResolution = 50;

    public void Create(PlanetMeshChunkRenderer _renderer, List<Vector3> _vertices) 
    {
        texture = new Texture2D(textureResolution, 1);
        vertices = _vertices;
        count++;
        gameObject.name = $"Chunk {count}";

        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.receiveShadows = false;
        vertices = new List<Vector3> { _vertices[0], _vertices[1], _vertices[2] };
        triangles = new List<int>();

        var settings = _renderer.shapeSettings;

        if (_renderer.shapeType == PlanetMeshChunkRenderer.ShapeType.Noise) 
        {
            meshRenderer.material = settings.terrainMaterial;
            meshRenderer.sharedMaterial.SetVector("_renderOffset", settings.renderOffset * -1);
        }
            

        if (_renderer.shapeType == PlanetMeshChunkRenderer.ShapeType.Sphere)
            meshRenderer.material = settings.oceanMaterial;

        var chunkTriangles = settings.chunkTriangles;
        if (_renderer.shapeType == PlanetMeshChunkRenderer.ShapeType.Sphere)
            chunkTriangles = settings.oceanTriangles;

        SubdivideFace(0, 1, 2, chunkTriangles);
        
        var radius = settings.radius;
        var renderOffset = settings.renderOffset;

        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = vertices[i].normalized;

            switch (_renderer.shapeType) 
            {
                case PlanetMeshChunkRenderer.ShapeType.Noise:
					Vector3 seaLevel = vertices[i] * radius * (1 + settings.oceanDepth);
					float elevation = _renderer.shapeGenerator.CalculateAdditionalElevation(vertices[i]) * _renderer.shapeGenerator.amplitude;
                    vertices[i] = renderOffset + seaLevel + vertices[i] * elevation;
                    _renderer.shapeGenerator.elevationMinMax.AddValue(elevation * 10);
                    break;
                case PlanetMeshChunkRenderer.ShapeType.Sphere:
                    vertices[i] = renderOffset + vertices[i] * radius * (1 + settings.oceanDepth);
                    break;
            }
        }

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
