using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Terrain
{
    public Dictionary<Vector3, Chunk> chunkDict = new Dictionary<Vector3, Chunk>();
    public Dictionary<Vector3, Chunk> loadedChunks = new Dictionary<Vector3, Chunk>();

    GameObject gameObject;
    TaskExecutor taskExecutor;

    public Terrain(GameObject go)
    {
        gameObject = go;
        gameObject.transform.position = new Vector3(0, 0, 0);
        taskExecutor = gameObject.AddComponent<TaskExecutor>();
        GenerateStartingChunks();
    }

    public void GenerateStartingChunks()
    {
        int playerX = 0;
        int playerZ = 0;

        int x = 0;
        int z = 0;
        int dx = 0;
        int dz = -1;

        int chunkCount =  (TerrainManager.RENDER_DISTANCE / TerrainManager.CHUNK_SIZE);

        for (int i = 0; i < chunkCount * chunkCount; i++)
        {
            //if chunk doesn't exist, create it
            Chunk thisChunk;
            var position = new Vector3((playerX + x) * TerrainManager.CHUNK_SIZE, 0, (playerZ + z) * TerrainManager.CHUNK_SIZE);

            if (!chunkDict.TryGetValue(position, out thisChunk))
            {
                var go = new GameObject(position.ToString());
                go.transform.position = position;
                go.transform.SetParent(gameObject.transform);

                thisChunk = new Chunk(go);
                chunkDict.Add(position, thisChunk);
                loadedChunks.Add(position, thisChunk);

                taskExecutor.ScheduleTask(new Task(delegate
                {
                    thisChunk.Generate();
                }));
            }

            if ((x == z) ||
                ((x < 0) && (x == -z)) ||
                ((x > 0) && (x == 1 - z)))
            {
                int tmpDX = dx;
                dx = -dz;
                dz = tmpDX;
            }
            x = x + dx;
            z = z + dz;

        }
    }

    public void Update(Vector3 playerPos)
    {
        _GenerateMissingChunks(playerPos);
        _UnloadDistantChunks(playerPos);

        //Debug
        if (InputManager.isSpace)
        {
            Debug.Log("Chunks: " + loadedChunks.Count + "/" + chunkDict.Count);
        }
        //Debug

    }

    private void _GenerateMissingChunks(Vector3 playerPos)
    {
        int playerX = (int)playerPos.x / TerrainManager.CHUNK_SIZE;
        int playerZ = (int)playerPos.z / TerrainManager.CHUNK_SIZE;

        if (playerPos.x < 0)
        {
            playerX = (int)(playerPos.x - TerrainManager.CHUNK_SIZE) / TerrainManager.CHUNK_SIZE;
        }

        if (playerPos.z < 0)
        {
            playerZ = (int)(playerPos.z - TerrainManager.CHUNK_SIZE) / TerrainManager.CHUNK_SIZE;
        }

        int edgeLength = (TerrainManager.RENDER_DISTANCE / TerrainManager.CHUNK_SIZE);
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
            var position = new Vector3((playerX + x) * TerrainManager.CHUNK_SIZE, 0, (playerZ + z) * TerrainManager.CHUNK_SIZE);

            //if chunk doesn't exist, create it
            if (!chunkDict.TryGetValue(position, out thisChunk))
            {
                var go = new GameObject(position.ToString());
                go.transform.position = position;
                go.transform.SetParent(gameObject.transform);

                thisChunk = new Chunk(go);
                chunkDict.Add(position, thisChunk);
                loadedChunks.Add(position, thisChunk);

                taskExecutor.ScheduleTask(new Task(delegate
                {
                    thisChunk.Generate();
                }));
            }
            else
            {
                if (!thisChunk.isLoaded)
                {
                    var go = new GameObject(position.ToString());
                    go.transform.position = position;
                    go.transform.SetParent(gameObject.transform);
                    loadedChunks.Add(position, thisChunk);
                    thisChunk.isLoaded = true;

                    taskExecutor.ScheduleTask(new Task(delegate
                    {
                        thisChunk.Reload(go);
                    }));
                }
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

    #region old and innefficient _GenerateMissingChunks
    //private void _GenerateMissingChunks(Vector3 playerPos)
    //{
    //    int playerX = (int)playerPos.x / TerrainManager.CHUNK_SIZE;
    //    int playerZ = (int)playerPos.z / TerrainManager.CHUNK_SIZE;

    //    if (playerPos.x < 0)
    //    {
    //        playerX = (int)(playerPos.x - TerrainManager.CHUNK_SIZE) / TerrainManager.CHUNK_SIZE;
    //    }

    //    if (playerPos.z < 0)
    //    {
    //        playerZ = (int)(playerPos.z - TerrainManager.CHUNK_SIZE) / TerrainManager.CHUNK_SIZE;
    //    }

    //    int x = 0;
    //    int z = 0;
    //    int dx = 0;
    //    int dz = -1;

    //    int chunkCount = (TerrainManager.RENDER_DISTANCE / TerrainManager.CHUNK_SIZE);

    //    for (int i = 0; i < chunkCount * chunkCount; i++) //TODO: optimize this to only spiral around the outter edge of the matrix
    //    {
    //        //Prepare to store next chunk if it already exists
    //        Chunk thisChunk;
    //        var position = new Vector3((playerX + x) * TerrainManager.CHUNK_SIZE, 0, (playerZ + z) * TerrainManager.CHUNK_SIZE);

    //        //if chunk doesn't exist, create it
    //        if (!chunkDict.TryGetValue(position, out thisChunk))
    //        {
    //            var go = new GameObject(position.ToString());
    //            go.transform.position = position;
    //            go.transform.SetParent(gameObject.transform);

    //            thisChunk = new Chunk(go);
    //            chunkDict.Add(position, thisChunk);
    //            loadedChunks.Add(position, thisChunk);

    //            taskExecutor.ScheduleTask(new Task(delegate
    //            {
    //                thisChunk.Generate();
    //            }));
    //        }
    //        else
    //        {
    //            if (!thisChunk.isLoaded)
    //            {
    //                var go = new GameObject(position.ToString());
    //                go.transform.position = position;
    //                go.transform.SetParent(gameObject.transform);
    //                loadedChunks.Add(position, thisChunk);
    //                thisChunk.isLoaded = true;

    //                taskExecutor.ScheduleTask(new Task(delegate
    //                {
    //                    thisChunk.Reload(go);
    //                }));
    //            }
    //        }

    //        //Shift variables to next chunk
    //        if ((x == z) ||
    //            ((x < 0) && (x == -z)) ||
    //            ((x > 0) && (x == 1 - z)))
    //        {
    //            int tmpDX = dx;
    //            dx = -dz;
    //            dz = tmpDX;
    //        }

    //        x = x + dx;
    //        z = z + dz;

    //    }
    //}
    #endregion

    private void _UnloadDistantChunks(Vector3 playerPos)
    {
        //Get all chunks from existsDict
        #region foreach version
        //foreach (KeyValuePair<Vector3, Chunk> kvp in chunkDict)
        //{
        //    #region old code
        //    //var location = kvp.Key;

        //    //int chunkCount = TerrainManager.RENDER_DISTANCE / TerrainManager.CHUNK_SIZE;
        //    //int maxXZ = chunkCount - (chunkCount - ((int)chunkCount / 2));
        //    //int threshold = (int)new Vector3(maxXZ, 0, maxXZ).magnitude;

        //    //int playerChunkPosX = (int)playerPos.x / TerrainManager.CHUNK_SIZE;
        //    //if (playerPos.x < 0) playerChunkPosX -= 1;
        //    //int playerChunkPosZ = (int)playerPos.z / TerrainManager.CHUNK_SIZE;
        //    //if (playerPos.z < 0) playerChunkPosZ -= 1;
        //    //var playerChunkPos = new Vector3(playerChunkPosX, 0, playerChunkPosZ);

        //    //int locationChunkPosX = (int)location.x / TerrainManager.CHUNK_SIZE;
        //    //int locationChunkPosZ = (int)location.z / TerrainManager.CHUNK_SIZE;
        //    //var locationChunkPos = new Vector3(locationChunkPosX, 0, locationChunkPosZ);

        //    //var differenceVector = (playerChunkPos - locationChunkPos);
        //    //float magnitude = differenceVector.magnitude;
        //    //int distance = (int)Mathf.Abs(magnitude);

        //    //if (distance > threshold)
        //    //{
        //    //    kvp.Value.isLoaded = false;
        //    //    kvp.Value.Unload();
        //    //}
        //    #endregion

        //    var location = kvp.Key;

        //    int playerChunkPosX = (int)playerPos.x / TerrainManager.CHUNK_SIZE;
        //    if (playerPos.x < 0) playerChunkPosX -= 1;
        //    int playerChunkPosZ = (int)playerPos.z / TerrainManager.CHUNK_SIZE;
        //    if (playerPos.z < 0) playerChunkPosZ -= 1;
        //    var playerChunkPos = new Vector3(playerChunkPosX, 0, playerChunkPosZ);

        //    int maxDistance = (TerrainManager.RENDER_DISTANCE / TerrainManager.CHUNK_SIZE) / 2;

        //    bool tooFarNorth = ((int)location.z / TerrainManager.CHUNK_SIZE) - playerChunkPosZ > maxDistance;
        //    bool tooFarSouth = playerChunkPosZ - ((int)location.z / TerrainManager.CHUNK_SIZE) > maxDistance;
        //    bool tooFarEast = ((int)location.x / TerrainManager.CHUNK_SIZE) - playerChunkPosX > maxDistance;
        //    bool tooFarWest = playerChunkPosX - ((int)location.x / TerrainManager.CHUNK_SIZE) > maxDistance;

        //    bool tooFar = tooFarNorth || tooFarSouth || tooFarEast || tooFarWest;

        //    if (tooFar)
        //    {
        //        kvp.Value.isLoaded = false;
        //        kvp.Value.Unload();
        //    }
        //}
        #endregion

        int playerChunkPosX = (int)playerPos.x / TerrainManager.CHUNK_SIZE;
        if (playerPos.x < 0) playerChunkPosX -= 1;
        int playerChunkPosZ = (int)playerPos.z / TerrainManager.CHUNK_SIZE;
        if (playerPos.z < 0) playerChunkPosZ -= 1;

        int maxDistance = (TerrainManager.RENDER_DISTANCE / TerrainManager.CHUNK_SIZE) ;

        List<Vector3> chunkPositionsToRemove = new List<Vector3>();


        foreach (Vector3 position in loadedChunks.Keys.Where(key => playerChunkPosX - (int)key.x / TerrainManager.CHUNK_SIZE > maxDistance ||
                                                                 (int)key.x / TerrainManager.CHUNK_SIZE - playerChunkPosX > maxDistance ||
                                                                 playerChunkPosZ - (int)key.z / TerrainManager.CHUNK_SIZE > maxDistance ||
                                                                 (int)key.z / TerrainManager.CHUNK_SIZE - playerChunkPosZ > maxDistance).ToList())
        { 
            Chunk thisChunk;
            loadedChunks.TryGetValue(position, out thisChunk);
            thisChunk.isLoaded = false;
            thisChunk.Unload();

            chunkPositionsToRemove.Add(position);
        }

        foreach (Vector3 position in chunkPositionsToRemove)
        {
            loadedChunks.Remove(position);
        }

    }
}