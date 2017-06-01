using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Chunk
{
    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    GameObject gameObject;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCol;

    float[,] heightMap;
    Vector3 chunkPosition;
    Thread _thread;
    bool isThreadFinished = false;

    public Chunk(GameObject go)
    {
        gameObject = go;
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load<Material>("Materials/Default");
        meshCol = gameObject.AddComponent<MeshCollider>();

        heightMap = new float[Terrain.CHUNK_SIZE + 1, Terrain.CHUNK_SIZE + 1];
        chunkPosition = gameObject.transform.position;

        _thread = new Thread(_ThreadedWork);
        _thread.Start();

    }

    private void _GenerateHeightMap()
    {
        var generator = new SimplexNoiseGenerator("test");

        for (int x = 0; x < heightMap.GetLength(0); x++)
        {
            for (int z = 0; z < heightMap.GetLength(1); z++)
            {
                heightMap[x, z] = generator.coherentNoise(x + chunkPosition.x, 0, z + chunkPosition.z, 2, 100, 50, 2, 0.1f); 
            }
        }
    }

    private void _GenerateGeometry()
    {
        int numTris = 0;
        bool flipped = false;

        for (int x = 0; x < Terrain.CHUNK_SIZE; x++)
        {
            for (int z = 0; z < Terrain.CHUNK_SIZE; z++)
            {

                verts.Add(new Vector3(x + 0, heightMap[x + 0, z + 0], z + 0));
                verts.Add(new Vector3(x + 0, heightMap[x + 0, z + 1], z + 1));
                verts.Add(new Vector3(x + 1, heightMap[x + 1, z + 1], z + 1));
                verts.Add(new Vector3(x + 1, heightMap[x + 1, z + 0], z + 0));

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
            if (Terrain.CHUNK_SIZE % 2 == 0)
            {
                flipped = !flipped;
            }
        }
    }

    private void _ThreadedWork()
    {
        _GenerateHeightMap();
        _GenerateGeometry();
        isThreadFinished = true;
        lock (Terrain.chunksToListLock)
        {
            Terrain.chunksToRender.Add(this);
        }
    }

    public void Render()
    {
        Mesh mesh = meshFilter.mesh;
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        meshCol.sharedMesh = mesh;
    }

    public void Unload()
    {
        GameObject.Destroy(this.gameObject);
    }

    public bool IsThreadFinished()
    {
        return isThreadFinished;
    }

    //temp

    public Vector3 GetPosition()
    {
        return chunkPosition;
    }
    //temp
}