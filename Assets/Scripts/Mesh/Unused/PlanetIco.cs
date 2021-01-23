using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Create a icosahedron. Vertices are cached meaning only
 * vertices absolutely required are added.
 */
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlanetIco : MonoBehaviour
{
	private static int planetCount = 0;

	public string planetName;

	public int recursionLevel = 3;
	public float radius = 0.5f;

	private Mesh mesh;
	private List<Vector3> vertList;
	private List<int> triList;
	private List<TriangleIndices> faces;
	private Vector2[] UVs;
	private Vector3[] normales;
	private Color[] colors;

	private void Awake()
    {
		recursionLevel = Mathf.Clamp(recursionLevel, 0, 6);
	}

    private void Start()
    {
		gameObject.layer = LayerMask.NameToLayer("Planets");
		gameObject.name = $"({++planetCount}) Planet - " + planetName;

		GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Planet");
		GenerateMesh();

		//gameObject.AddComponent<SphereCollider>();
		var collider = gameObject.AddComponent<MeshCollider>();
		collider.sharedMesh = mesh;
	}

	[Range(0, 1)]
	public float baseRoughness = 1f;
	public int numLayers = 2;
	public Vector3 centre = new Vector3(0, 0, 0);
	public float roughness = 1f;
	public float persistence = 1f;
	public float minValue = 1f;
	public float strength = 1f;

	public float Evaluate(Noise noise, Vector3 point)
	{
		float noiseValue = 0;
		float frequency = baseRoughness;
		float amplitude = 1;

		for (int i = 0; i < numLayers; i++)
		{
			float v = noise.Evaluate(point * frequency + centre);
			noiseValue += (v + 1) * .5f * amplitude;
			frequency *= roughness;
			amplitude *= persistence;
		}

		noiseValue = noiseValue - minValue;
		return noiseValue * strength;
	}

	private void OnValidate()
    {
		GenerateMesh();
    }

    private void GenerateMesh()
	{
		mesh = GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		mesh.name = gameObject.name;

		CreateBaseVertices();
		CreateFacesAndAddMoreVertices();
		CreateUVs();
		CreateNormals();
		CreateColors();

		Noise noise = new Noise();

		

		for (int i = 0; i < vertList.Count; i++) 
		{
			float elevation = Mathf.Max(0, Evaluate(noise, vertList[i]));
			elevation = radius * (1 + elevation);

			vertList[i] = vertList[i] * elevation;
		}

		mesh.vertices = vertList.ToArray();
		mesh.triangles = triList.ToArray();
		mesh.uv = UVs;
		mesh.normals = normales;
		mesh.colors = colors;

		mesh.RecalculateBounds();
		mesh.Optimize();

		GetComponent<MeshFilter>().sharedMesh = mesh;
		mesh.RecalculateBounds();
	}

	private void CreateBaseVertices() 
	{
		vertList = new List<Vector3>();

		// create 12 vertices of a icosahedron
		float t = (1f + Mathf.Sqrt(5f)) / 2f;

		vertList.Add(new Vector3(-1f, t, 0f).normalized * radius);
		vertList.Add(new Vector3(1f, t, 0f).normalized * radius);
		vertList.Add(new Vector3(-1f, -t, 0f).normalized * radius);
		vertList.Add(new Vector3(1f, -t, 0f).normalized * radius);

		vertList.Add(new Vector3(0f, -1f, t).normalized * radius);
		vertList.Add(new Vector3(0f, 1f, t).normalized * radius);
		vertList.Add(new Vector3(0f, -1f, -t).normalized * radius);
		vertList.Add(new Vector3(0f, 1f, -t).normalized * radius);

		vertList.Add(new Vector3(t, 0f, -1f).normalized * radius);
		vertList.Add(new Vector3(t, 0f, 1f).normalized * radius);
		vertList.Add(new Vector3(-t, 0f, -1f).normalized * radius);
		vertList.Add(new Vector3(-t, 0f, 1f).normalized * radius);
	}

	private void CreateFacesAndAddMoreVertices() 
	{
		Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();

		// create 20 triangles of the icosahedron
		faces = new List<TriangleIndices>();

		// 5 faces around point 0
		faces.Add(new TriangleIndices(0, 11, 5));
		faces.Add(new TriangleIndices(0, 5, 1));
		faces.Add(new TriangleIndices(0, 1, 7));
		faces.Add(new TriangleIndices(0, 7, 10));
		faces.Add(new TriangleIndices(0, 10, 11));

		// 5 adjacent faces
		faces.Add(new TriangleIndices(1, 5, 9));
		faces.Add(new TriangleIndices(5, 11, 4));
		faces.Add(new TriangleIndices(11, 10, 2));
		faces.Add(new TriangleIndices(10, 7, 6));
		faces.Add(new TriangleIndices(7, 1, 8));

		// 5 faces around point 3
		faces.Add(new TriangleIndices(3, 9, 4));
		faces.Add(new TriangleIndices(3, 4, 2));
		faces.Add(new TriangleIndices(3, 2, 6));
		faces.Add(new TriangleIndices(3, 6, 8));
		faces.Add(new TriangleIndices(3, 8, 9));

		// 5 adjacent faces
		faces.Add(new TriangleIndices(4, 9, 5));
		faces.Add(new TriangleIndices(2, 4, 11));
		faces.Add(new TriangleIndices(6, 2, 10));
		faces.Add(new TriangleIndices(8, 6, 7));
		faces.Add(new TriangleIndices(9, 8, 1));


		// refine triangles
		for (int i = 0; i < recursionLevel; i++)
		{
			List<TriangleIndices> faces2 = new List<TriangleIndices>();
			foreach (var tri in faces)
			{
				// replace triangle by 4 triangles
				int a = GetMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
				int b = GetMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
				int c = GetMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);

				faces2.Add(new TriangleIndices(tri.v1, a, c));
				faces2.Add(new TriangleIndices(tri.v2, b, a));
				faces2.Add(new TriangleIndices(tri.v3, c, b));
				faces2.Add(new TriangleIndices(a, b, c));
			}
			faces = faces2;
		}

		triList = new List<int>();
		for (int i = 0; i < faces.Count; i++)
		{
			triList.Add(faces[i].v1);
			triList.Add(faces[i].v2);
			triList.Add(faces[i].v3);
		}
	}

	private void CreateUVs() 
	{
		var nVertices = mesh.vertices;
		UVs = new Vector2[nVertices.Length];

		for (var i = 0; i < nVertices.Length; i++)
		{
			var unitVector = nVertices[i].normalized;
			Vector2 ICOuv = new Vector2(0, 0);
			ICOuv.x = (Mathf.Atan2(unitVector.x, unitVector.z) + Mathf.PI) / Mathf.PI / 2;
			ICOuv.y = (Mathf.Acos(unitVector.y) + Mathf.PI) / Mathf.PI - 1;
			UVs[i] = new Vector2(ICOuv.x, ICOuv.y);
		}
	}

	private void CreateNormals() 
	{
		normales = new Vector3[vertList.Count];
		for (int i = 0; i < normales.Length; i++)
			normales[i] = vertList[i].normalized;
	}

	private void CreateColors() 
	{
		bool test = false;

		colors = new Color[mesh.vertexCount];
		for (int i = 0; i < colors.Length / 3; i++)
		{
			if (test)
			{
				colors[i * 3 + 0] = new Color(0, 1f, 0);
				colors[i * 3 + 1] = new Color(0, 1f, 0);
				colors[i * 3 + 2] = new Color(0, 1f, 0);
			}
			else
			{
				colors[i * 3 + 0] = new Color(0, 0, 1f);
				colors[i * 3 + 1] = new Color(0, 0, 1f);
				colors[i * 3 + 2] = new Color(0, 0, 1f);
			}

			test = !test;
		}
	}

	private struct TriangleIndices
	{
		public int v1;
		public int v2;
		public int v3;

		public TriangleIndices(int v1, int v2, int v3)
		{
			this.v1 = v1;
			this.v2 = v2;
			this.v3 = v3;
		}
	}

	// return index of point in the middle of p1 and p2
	private static int GetMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
	{
		// first check if we have it already
		bool firstIsSmaller = p1 < p2;
		long smallerIndex = firstIsSmaller ? p1 : p2;
		long greaterIndex = firstIsSmaller ? p2 : p1;
		long key = (smallerIndex << 32) + greaterIndex;

		int ret;
		if (cache.TryGetValue(key, out ret))
		{
			return ret;
		}

		// not in cache, calculate it
		Vector3 point1 = vertices[p1];
		Vector3 point2 = vertices[p2];
		Vector3 middle = new Vector3
		(
			(point1.x + point2.x) / 2f,
			(point1.y + point2.y) / 2f,
			(point1.z + point2.z) / 2f
		);

		// add vertex makes sure point is on unit sphere
		int i = vertices.Count;
		vertices.Add(middle.normalized * radius);

		// store it, return index
		cache.Add(key, i);

		return i;
	}
}