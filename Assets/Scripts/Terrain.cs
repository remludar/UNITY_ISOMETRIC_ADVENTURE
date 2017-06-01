 using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Terrain
{
    public const int CHUNK_SIZE = 32;
    public const int WORLD_DIMENSION = 11;

    Dictionary<Vector3, Chunk> chunkDict = new Dictionary<Vector3, Chunk>();

    public static List<Chunk> chunksToRender = new List<Chunk>();
    public static object chunksToListLock = new object();

    GameObject gameObject;
    Player player;

    public void Generate()
    {
        gameObject = new GameObject("Terrain");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        int zeroIndexHalfEdge = ((WORLD_DIMENSION - 3) / 2) + 1;
        for (int x = -zeroIndexHalfEdge; x < zeroIndexHalfEdge + 1; x++)
        {
            for (int z = -zeroIndexHalfEdge; z < zeroIndexHalfEdge + 1; z++)
            {
                var chunkPosition = new Vector3(x * CHUNK_SIZE, 0, z * CHUNK_SIZE);
                var go = new GameObject(chunkPosition.ToString());
                go.transform.position = chunkPosition;
                go.transform.parent = gameObject.transform;
                var chunk = new Chunk(go);
                chunkDict.Add(chunkPosition, chunk);
            }
        }
    }

    

    public void Update()
    {
        _GenerateMissingChunks(player.GetPosition());
        _RenderChunks();
        _RemoveDistantChunks(player.GetPosition());
        
    }

    private void _GenerateMissingChunks(Vector3 playerPos)
    {
        int playerX = (int)playerPos.x / Terrain.CHUNK_SIZE;
        int playerZ = (int)playerPos.z / Terrain.CHUNK_SIZE;

        if (playerPos.x < 0)
        {
            playerX = (int)(playerPos.x - Terrain.CHUNK_SIZE) / Terrain.CHUNK_SIZE;
        }

        if (playerPos.z < 0)
        {
            playerZ = (int)(playerPos.z - Terrain.CHUNK_SIZE) / Terrain.CHUNK_SIZE;
        }

        int edgeLength = Terrain.WORLD_DIMENSION;
        int zeroIndexHalfEdge = ((edgeLength - 3) / 2) + 1;

        int x = zeroIndexHalfEdge;
        int z = zeroIndexHalfEdge;
        int dx = -1;
        int dz = 0;

        int corners = 4;
        int edges = edgeLength - 2;
        int chunkCount = (edges * 4) + corners;

        for (int i = 0; i < chunkCount; i++)
        {
            //Prepare to store next chunk if it already exists
            Chunk thisChunk;
            var topRightPosition = new Vector3((playerX + x) * Terrain.CHUNK_SIZE, 0, (playerZ + z) * Terrain.CHUNK_SIZE);

            //if chunk doesn't exist, create it
            if (!chunkDict.TryGetValue(topRightPosition, out thisChunk))
            {
                var go = new GameObject(topRightPosition.ToString());
                go.transform.position = topRightPosition;
                go.transform.SetParent(gameObject.transform);

                thisChunk = new Chunk(go);
                chunkDict.Add(topRightPosition, thisChunk);
            }

            //Shift variables to next chunk
            if ((x == -zeroIndexHalfEdge && z == zeroIndexHalfEdge) ||
                (x == -zeroIndexHalfEdge && z == -zeroIndexHalfEdge) ||
                (x == zeroIndexHalfEdge && z == -zeroIndexHalfEdge))
            {
                int tmpDX = dx;
                dx = -dz;
                dz = tmpDX;
            }

            x = x + dx;
            z = z + dz;

        }
    }

    private void _RenderChunks()
    {
        lock (chunksToListLock)
        {
            int numberOfElementsToCopy = chunksToRender.Count;
            Chunk[] chunksToRenderArray = new Chunk[numberOfElementsToCopy];

            for (int i = 0; i < numberOfElementsToCopy; i++)
            {
                if (chunksToRender[i] == null)
                {
                    Debug.Log("Somehow there's a null chunk in the chunksToRenderList");
                    return;
                }
                chunksToRenderArray[i] = chunksToRender[i];
            }

            for (int i = 0; i < numberOfElementsToCopy; i++)
            {
                chunksToRenderArray[i].Render();
                chunksToRender.Remove(chunksToRenderArray[i]);
            }
        }
        
    }

    private void _RemoveDistantChunks(Vector3 playerPos)
    {
        int playerChunkPosX = (int)playerPos.x / CHUNK_SIZE;
        if (playerPos.x < 0) playerChunkPosX -= 1;
        int playerChunkPosZ = (int)playerPos.z / CHUNK_SIZE;
        if (playerPos.z < 0) playerChunkPosZ -= 1;

        int maxDistance = ((WORLD_DIMENSION - 3) / 2) + 1; ;

        List<Vector3> chunkPositionsToRemove = new List<Vector3>();

        foreach (Vector3 position in chunkDict.Keys.Where(key => playerChunkPosX - (int)key.x / CHUNK_SIZE > maxDistance ||
                                                                 (int)key.x / CHUNK_SIZE - playerChunkPosX > maxDistance ||
                                                                 playerChunkPosZ - (int)key.z / CHUNK_SIZE > maxDistance ||
                                                                 (int)key.z / CHUNK_SIZE - playerChunkPosZ > maxDistance).ToList())
        {
            Chunk thisChunk;
            chunkDict.TryGetValue(position, out thisChunk);
            thisChunk.Unload();

            chunkPositionsToRemove.Add(position);
        }

        foreach (Vector3 position in chunkPositionsToRemove)
        {
            chunkDict.Remove(position);
        }
    }
}