using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Chunk
{
    public bool needsUpdate;

    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    GameObject gameObject;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCol;

    public Chunk(GameObject gameObject)
    {
        needsUpdate = false;

        this.gameObject = gameObject;
        meshCol = gameObject.AddComponent<MeshCollider>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load<Material>("Materials/Default");

        _GenerateMesh();
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
                if (!Terrain.heightMap.TryGetValue(new Vector2(worldX + 0, worldZ + 0), out heights[0])) throw new Exception("Shits fucked");
                if (!Terrain.heightMap.TryGetValue(new Vector2(worldX + 0, worldZ + 1), out heights[1])) throw new Exception("Shits fucked");
                if (!Terrain.heightMap.TryGetValue(new Vector2(worldX + 1, worldZ + 1), out heights[2])) throw new Exception("Shits fucked");
                if (!Terrain.heightMap.TryGetValue(new Vector2(worldX + 1, worldZ + 0), out heights[3])) throw new Exception("Shits fucked");

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
    }


}