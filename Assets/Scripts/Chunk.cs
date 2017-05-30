using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Chunk
{
    string name;

    public bool needsUpdate;
    public bool isLoaded;

    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    GameObject gameObject;
    
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCol;
    Dictionary<Vector2, float> heightMap = new Dictionary<Vector2, float>();
    int type;
    float[,] noise;

    public Chunk(GameObject gameObject)
    {
        name = gameObject.transform.position.ToString();
        type = 1;
        isLoaded = false;
        this.gameObject = gameObject;
        noise = new float[TerrainManager.CHUNK_SIZE + 1, TerrainManager.CHUNK_SIZE + 1];
        isLoaded = true;

        meshCol = gameObject.AddComponent<MeshCollider>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load<Material>("Materials/Default");
    }

    public void Generate()
    {
        _GenerateHeightMap();
        _GenerateMesh();
    }

    public void Reload(GameObject go)
    {
        gameObject = go;
        meshCol = gameObject.AddComponent<MeshCollider>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load<Material>("Materials/Default");

        //Render Mesh
        Mesh mesh = meshFilter.mesh;
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        meshCol.sharedMesh = mesh;
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    public void Unload()
    {
        GameObject.Destroy(gameObject);
    }

    private void _GenerateHeightMap()
    {
        //Create Heightmap
        var generator = new SimplexNoiseGenerator("test");

        int startX = (int)gameObject.transform.position.x;
        int startZ = (int)gameObject.transform.position.z;
        
        for (int x = startX, localX = 0; x < startX + TerrainManager.CHUNK_SIZE + 1; x++, localX++)
        {
            for (int z = startZ, localZ = 0; z < startZ + TerrainManager.CHUNK_SIZE + 1; z++, localZ++)
            {
                noise[localX,localZ] = 0.0f;

                //Rolling Hills
                if (type == 1)
                {
                    noise[localX, localZ] = generator.coherentNoise(x, 0, z, 2, 100, 50, 2, 0.1f);
                }

                //Mountains
                else if (type == 2)
                {
                    noise[localX, localZ] = generator.coherentNoise(x, 0, z, 1, 100, 50f, 2, 0.9f);
                    noise[localX, localZ] += generator.coherentNoise(x, 0, z, 2, 75, 50f, 2, 0.9f);
                    noise[localX, localZ] += generator.coherentNoise(x, 0, z, 2, 10, 10f, 2, 0.9f);
                }
                else if (type == 3)
                {
                    noise[localX, localZ] = generator.coherentNoise(x, 0, z, 1, 25, 0.5f, 2, 0.9f);
                }
                else
                {
                    //Mostly Flat
                    noise[localX, localZ] = generator.coherentNoise(x, 0, z);
                }


                heightMap.Add(new Vector2(x, z), noise[localX, localZ]);

            }
        }
    }
    private void _GenerateMesh()
    {
        int numTris = 0;
        bool flipped = false;
        //Create verts and tris
        for (int x = 0; x < TerrainManager.CHUNK_SIZE; x++)
        {
            for (int z = 0; z < TerrainManager.CHUNK_SIZE; z++)
            {
                int worldX = (int)gameObject.transform.position.x + x;
                int worldZ = (int)gameObject.transform.position.z + z;

                float[] heights = new float[4];
                if (!heightMap.TryGetValue(new Vector2(worldX + 0, worldZ + 0), out heights[0])) throw new Exception("Shits fucked");
                if (!heightMap.TryGetValue(new Vector2(worldX + 0, worldZ + 1), out heights[1])) throw new Exception("Shits fucked");
                if (!heightMap.TryGetValue(new Vector2(worldX + 1, worldZ + 1), out heights[2])) throw new Exception("Shits fucked");
                if (!heightMap.TryGetValue(new Vector2(worldX + 1, worldZ + 0), out heights[3])) throw new Exception("Shits fucked");

                verts.Add(new Vector3(x + 0, heights[0], z + 0));
                verts.Add(new Vector3(x + 0, heights[1], z + 1));
                verts.Add(new Vector3(x + 1, heights[2], z + 1));
                verts.Add(new Vector3(x + 1, heights[3], z + 0));

                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));
                uvs.Add(new Vector2(1, 0));

                if (flipped)
                {
                    tris.Add(numTris + 0);
                    tris.Add(numTris + 1);
                    tris.Add(numTris + 2);
                    tris.Add(numTris + 2);
                    tris.Add(numTris + 3);
                    tris.Add(numTris + 0);
                }
                else
                {
                    tris.Add(numTris + 0);
                    tris.Add(numTris + 1);
                    tris.Add(numTris + 3);
                    tris.Add(numTris + 3);
                    tris.Add(numTris + 1);
                    tris.Add(numTris + 2);
                }
                numTris += 4;
                flipped = !flipped;

            }
            if (TerrainManager.CHUNK_SIZE % 2 == 0)
            {
                flipped = !flipped;
            }
        }
        //Render Mesh
        Mesh mesh = meshFilter.mesh;
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        meshCol.sharedMesh = mesh;
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        isLoaded = true;
        
    }
}