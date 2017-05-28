using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Terrain
{
    public Dictionary<Vector3, Chunk> chunkDict = new Dictionary<Vector3, Chunk>();

    GameObject gameObject;
    Dictionary<Vector3, bool> chunkExistDict = new Dictionary<Vector3, bool>();
    List<Vector3> chunksToUnload = new List<Vector3>();
    TaskExecutor taskExecutor;

    public Terrain(GameObject go)
    {
        gameObject = go;
        gameObject.transform.position = new Vector3(0, 0, 0);
        taskExecutor = gameObject.AddComponent<TaskExecutor>();
        _NextChunkInSpiral(Vector2.zero);
    }

    public void GenerateMissingChunks(Vector3 playerPos)
    {
        _NextChunkInSpiral(playerPos);
        _UnloadDistantChunks(playerPos);
    }

    private void _NextChunkInSpiral(Vector3 playerPos)
    {
        int playerX = (int)playerPos.x / TerrainManager.CHUNK_SIZE;
        int playerZ = (int)playerPos.z / TerrainManager.CHUNK_SIZE; 

        int x = 0;
        int z = 0;
        int dx = 0;
        int dz = -1;

        int chunkCount = (TerrainManager.RENDER_DISTANCE / TerrainManager.CHUNK_SIZE);

        for (int i = 0; i < chunkCount * chunkCount; i++)
        {
            //if chunk doesn't exist, create it
            var position = new Vector3((playerX + x) * TerrainManager.CHUNK_SIZE, 0, (playerZ + z) * TerrainManager.CHUNK_SIZE);
            bool chunkExists;
            chunkExists = chunkExistDict.TryGetValue(position, out chunkExists);

            if (!chunkExists)
            {
                chunkExistDict.Add(position, true);

                var go = new GameObject(position.ToString());
                go.transform.position = position;
                go.transform.SetParent(gameObject.transform);

                taskExecutor.ScheduleTask(new Task(delegate
                {
                    chunkDict.Add(position, new Chunk(go));
                }));
            }
            
            if ( (x  == z) || 
                ((x < 0) && (x == -z )) ||
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
    private void _UnloadDistantChunks(Vector3 playerPos)
    {
        //Debug.Log(chunkDict.Count);

        //Get all chunks from existsDict
        foreach (KeyValuePair<Vector3, bool> kvp in chunkExistDict)
        {
            var location = kvp.Key;

            //tmp
            if (location == new Vector3(192, 0, 192) && chunkDict.Count == 146)
            {
                int blarb = 0;
            }

            int chunkCount = TerrainManager.RENDER_DISTANCE / TerrainManager.CHUNK_SIZE;
            //int threshold = 6;// chunkCount - (chunkCount - ((int)chunkCount / 2));
            int maxXZ = chunkCount - (chunkCount - ((int)chunkCount / 2));
            int threshold = (int)new Vector3(maxXZ, 0, maxXZ).magnitude;

            int playerChunkPosX = (int)playerPos.x / TerrainManager.CHUNK_SIZE;
            int playerChunkPosZ = (int)playerPos.z / TerrainManager.CHUNK_SIZE;
            var playerChunkPos = new Vector3(playerChunkPosX, 0, playerChunkPosZ);

            int locationChunkPosX = (int)location.x / TerrainManager.CHUNK_SIZE;
            int locationChunkPosZ = (int)location.z / TerrainManager.CHUNK_SIZE;
            var locationChunkPos = new Vector3(locationChunkPosX, 0, locationChunkPosZ);

            var differenceVector = (playerChunkPos - locationChunkPos);
            float magnitude = differenceVector.magnitude;
            int distance = (int)Mathf.Abs(magnitude);

            if (distance > threshold)
            {
                Chunk chunkToUnload;
                if (chunkDict.TryGetValue(location, out chunkToUnload))
                {
                    chunksToUnload.Add(location);
                }
            }
        }

        foreach (Vector3 locationToUnload in chunksToUnload)
        {
            Chunk chunkToUnload; 
            chunkDict.TryGetValue(locationToUnload, out chunkToUnload);

            chunkToUnload.DeleteChunk();
            chunkDict.Remove(locationToUnload);
            chunkExistDict.Remove(locationToUnload);
        }

        chunksToUnload.Clear();
    }
}