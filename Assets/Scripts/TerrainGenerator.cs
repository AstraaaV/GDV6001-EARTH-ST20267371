using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;


#if UNITY_EDITOR
using UnityEditor;
#endif
public class TerrainGenerator : MonoBehaviour
{
    [Header("Terrain Settings")]
    public int width = 100;
    public int depth = 100;
    public float noiseScale = 0.1f;
    public float heightMultiplier = 10f;
    public int seed = 0;

    [Header("Terrain Materials")]
    public Material grassMaterial;
    public Material rockMaterial;
    public Material snowMaterial;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    private void Start()
    {
        GenerateTerrain();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            seed = Random.Range(0, 99999);
            GenerateTerrain();
        }
    }

    public void GenerateTerrain()
    {
        mesh = new Mesh();
        var meshf = GetComponent<MeshFilter>();
        meshf.sharedMesh = mesh;

        vertices = new Vector3[(width + 1) * (depth + 1)];
        Random.InitState(seed);

        for(int z = 0, i = 0; z <= depth; z++)
        {
            for(int x = 0; x <= width; x++, i++)
            {
                float sampleX = (x + seed) * noiseScale;
                float sampleZ = (z + seed) * noiseScale;
                float nx = sampleX * 0.005f;
                float nz = sampleZ * 0.005f;

                float baseNoise = Mathf.PerlinNoise(nx, nz);
                float ridgeNoise = 1f - Mathf.Abs(Mathf.PerlinNoise(nx * 0.7f, nz * 0.7f) * 2f - 1f);
                float detailNoise = Mathf.PerlinNoise(nx * 3f, nz * 3f) * 0.2f;
                float y = (baseNoise * 0.6f + ridgeNoise * 0.4f + detailNoise) * heightMultiplier;
                y = Mathf.Pow(y / heightMultiplier, 1.5f) * heightMultiplier;
                y += Mathf.PerlinNoise(nx * 0.2f + 10, nz * 0.2f + 10) * 2f - 1f;

                vertices[i] = new Vector3(x, y, z);
            }
        }

        triangles = new int[width * depth * 6];
        for(int z = 0, vert = 0, tris = 0; z < depth; z++, vert++)
        {
            for(int x = 0; x < width; x++, vert++, tris += 6)
            {
                triangles[tris + 0] = vert;
                triangles[tris + 1] = vert + width + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + width + 1;
                triangles[tris + 5] = vert + width + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var renderer = GetComponent<MeshRenderer>();

        if (renderer.sharedMaterial == null)
        {
            Debug.Log("Assign a material.");
        }

        Color[] colours = new Color[vertices.Length];

        for(int i = 0; i < vertices.Length; i++)
        {
            float heightPercent = vertices[i].y / heightMultiplier;

            if (heightPercent < 0.4f)
                colours[i] = Color.green;
            else if (heightPercent < 0.75f)
                colours[i] = Color.gray;
            else
                colours[i] = Color.white;
        }

        mesh.colors = colours;

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(!Application.isPlaying)
        {
            GenerateTerrain();
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
        }
    }
#endif
}
