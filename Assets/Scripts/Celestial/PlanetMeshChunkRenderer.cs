using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetMeshChunkRenderer
{
    private List<Vector3> baseFormVertices;
    public List<PlanetMeshChunk> chunks;

    public ShapeGenerator shapeGenerator;
    public ShapeSettings shapeSettings;

    private Transform parent;

    public List<Vector3> sphereVertices;
    public Noise noise;

    private GameObject test;

    private Transform cam;

    public enum ShapeType { Noise, Sphere }
    public ShapeType shapeType;

    public PlanetMeshChunkRenderer(Transform _parent, ShapeGenerator _shapeGenerator, ShapeType _shapeType)
    {
        shapeType = _shapeType;
        
        test = GameObject.Find("Render Debug Point");

        parent = _parent;
        shapeGenerator = _shapeGenerator;
        shapeSettings = _shapeGenerator.shapeSettings;

        noise = new Noise();

        var radius = shapeGenerator.shapeSettings.radius;
        var t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;

        // The base vertices that make up a base form icosahedron
        baseFormVertices = new List<Vector3>()
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

        chunks = new List<PlanetMeshChunk>();
        sphereVertices = new List<Vector3>();

        HandleBaseFormFaces();

        PlanetMeshChunk.count = 0;
    }

    /*!
     * Sets nearby chunks active and far away chunks no longer active.
     */
    public void RenderNearbyChunks(float _distance) 
    {
        if (chunks == null || chunks.Count == 0)
            return;

        // Check which chunks to render
        for (int i = 0; i < chunks.Count; i++)
        {
            if ((Vector3.Distance(chunks[i].GetCenterPoint(), test.transform.position) < _distance) || shapeSettings.renderEverything)
            {
                if (chunks[i])
                    chunks[i].gameObject.SetActive(true);
            }
            else
            {
                chunks[i].gameObject.SetActive(false);
            }
        }
    }

    private void HandleBaseFormFaces()
    {
        var chunks = shapeSettings.chunks; // number of chunk recursions per base face

        if (shapeType == ShapeType.Sphere)
            chunks = shapeSettings.oceanChunks;

        GenerateChunks(new List<Vector3> { baseFormVertices[0], baseFormVertices[11], baseFormVertices[5] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[0], baseFormVertices[5], baseFormVertices[1] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[0], baseFormVertices[1], baseFormVertices[7] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[0], baseFormVertices[7], baseFormVertices[10] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[0], baseFormVertices[10], baseFormVertices[11] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[1], baseFormVertices[5], baseFormVertices[9] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[5], baseFormVertices[11], baseFormVertices[4] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[11], baseFormVertices[10], baseFormVertices[2] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[10], baseFormVertices[7], baseFormVertices[6] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[7], baseFormVertices[1], baseFormVertices[8] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[3], baseFormVertices[9], baseFormVertices[4] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[3], baseFormVertices[4], baseFormVertices[2] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[3], baseFormVertices[2], baseFormVertices[6] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[3], baseFormVertices[6], baseFormVertices[8] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[3], baseFormVertices[8], baseFormVertices[9] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[4], baseFormVertices[9], baseFormVertices[5] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[2], baseFormVertices[4], baseFormVertices[11] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[6], baseFormVertices[2], baseFormVertices[10] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[8], baseFormVertices[6], baseFormVertices[7] }, chunks);
        GenerateChunks(new List<Vector3> { baseFormVertices[9], baseFormVertices[8], baseFormVertices[1] }, chunks);
    }

    /*!
     * The number of chunk recursions per base face.
     */
    private void GenerateChunks(List<Vector3> _vertices, int n)
    {
        _vertices.Add(SphereUtils.GetMidPointVertex(_vertices[0], _vertices[1])); // Right Middle (3)
        _vertices.Add(SphereUtils.GetMidPointVertex(_vertices[1], _vertices[2])); // Bottom Middle (4)
        _vertices.Add(SphereUtils.GetMidPointVertex(_vertices[2], _vertices[0])); // Left middle (5)

        // Only draw the last recursion
        if (n == 1) 
        {
            GenerateChunk(new List<Vector3> { _vertices[0], _vertices[3], _vertices[5] }); // Top
            GenerateChunk(new List<Vector3> { _vertices[5], _vertices[4], _vertices[2] }); // Bottom Left
            GenerateChunk(new List<Vector3> { _vertices[4], _vertices[5], _vertices[3] }); // Bottom Middle
            GenerateChunk(new List<Vector3> { _vertices[3], _vertices[1], _vertices[4] }); // Bottom Right

            return;
        }

        GenerateChunks(new List<Vector3> { _vertices[0], _vertices[3], _vertices[5] }, n - 1); // Top
        GenerateChunks(new List<Vector3> { _vertices[5], _vertices[4], _vertices[2] }, n - 1); // Bottom Left
        GenerateChunks(new List<Vector3> { _vertices[4], _vertices[5], _vertices[3] }, n - 1); // Bottom Middle
        GenerateChunks(new List<Vector3> { _vertices[3], _vertices[1], _vertices[4] }, n - 1); // Bottom Right
    }

    private void GenerateChunk(List<Vector3> _vertices) 
    {
        // Create chunk gameObject
        var chunkObj = new GameObject();

        // Set parent
        chunkObj.transform.parent = parent.transform;

        // Add PlanetMeshChunk script to chunk gameObject
        var chunk = chunkObj.AddComponent<PlanetMeshChunk>();
        chunk.Create(this, _vertices);
        chunks.Add(chunk);
    }
}
