using UnityEngine;
using System.Collections.Generic;

// Generates a singular voxel chunk
public class Chunks : MonoBehaviour
{
    [Header("Chunk Settings")]
    public int chunkSize = 16;
    public float noiseScale = 0.05f;
    public float heightMultiplier = 5f;
    public int chunkX;
    public int chunkZ;

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));

        CreateChunk();
    }

    private void CreateChunk()
    {
        Mesh mesh = new Mesh();
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                int worldX = chunkX * chunkSize + x;
                int worldZ = chunkZ * chunkSize + z;

                float height = Mathf.PerlinNoise(worldX * noiseScale, worldZ * noiseScale) * heightMultiplier;
                
                for (int y = 0; y < Mathf.FloorToInt(height); y++)
                {
                    AddCube(new Vector3(x, y, z), verts, tris);
                }
            }
        }

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    private void AddCube(Vector3 pos, List<Vector3> verts, List<int> tris)
    {
        int vertCount = verts.Count;

        Vector3[] cubeVerts =
        {
            pos + new Vector3(0, 0, 0),
            pos + new Vector3(1, 0, 0),
            pos + new Vector3(1, 1, 0),
            pos + new Vector3(0, 1, 0),
            pos + new Vector3(0, 1, 1),
            pos + new Vector3(1, 1, 1),
            pos + new Vector3(1, 0, 1),
            pos + new Vector3(0, 0, 1)
        };

        verts.AddRange(cubeVerts);

        int[,] faces =
        {
            {0, 1, 2, 3 }, // Front
            {5, 4, 7, 6 }, // Back
            {3, 2, 5, 4 }, // Top
            {1, 0, 7, 6 }, // Bottom
            {4, 5, 6, 7 }, // Right
            {0, 3, 4, 7 } // Left
        };

        for(int i = 0; i < 6; i++)
        {
            tris.Add(vertCount + faces[i, 0]);
            tris.Add(vertCount + faces[i, 1]);
            tris.Add(vertCount + faces[i, 2]);
            tris.Add(vertCount + faces[i, 2]);
            tris.Add(vertCount + faces[i, 3]);
            tris.Add(vertCount + faces[i, 0]);
        }
    }
}
