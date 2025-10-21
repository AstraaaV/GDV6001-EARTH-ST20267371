using System.Diagnostics;
using UnityEngine;

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
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[(width + 1) * (depth + 1)];
        Random.InitState(seed);

        for(int z = 0, i = 0; z <= depth; z++)
        {
            for(int x = 0; x <= width; x++, i++)
            {
                float sampleX = (x + seed) * noiseScale;
                float sampleZ = (z + seed) * noiseScale;
                float y = Mathf.PerlinNoise(sampleX, sampleZ);
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

        GetComponent<MeshCollider>().sharedMesh = mesh;

        var renderer = GetComponent<MeshRenderer>();
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        renderer.sharedMaterial = new Material(shader);
        renderer.sharedMaterial.color = Color.green;
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
