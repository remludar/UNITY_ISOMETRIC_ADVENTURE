using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Terrain
{
    GameObject gameObject;
    Dictionary<Vector3, bool> chunkExistDict = new Dictionary<Vector3, bool>();
    public Dictionary<Vector3, Chunk> chunkDict = new Dictionary<Vector3, Chunk>();
    TaskExecutor taskExecutor;

    public Terrain(GameObject go)
    {
        gameObject = go;
        gameObject.transform.position = new Vector3(0, 0, 0);
        taskExecutor = gameObject.AddComponent<TaskExecutor>();
        _NextChunkInSpiral(Vector2.zero);
    }

    public void GenerateMissingChunks(Vector2 playerPos)
    {
        _NextChunkInSpiral(playerPos);
    }

    private void _NextChunkInSpiral(Vector2 playerPos)
    {
        

        int playerX = (int)playerPos.x / TerrainManager.CHUNK_SIZE;
        int playerZ = (int)playerPos.y / TerrainManager.CHUNK_SIZE; 

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

    //private Vector2 _NextChunkInSpiral(Vector2 playerPos)
    //{
    //    var taskExecutor = gameObject.AddComponent<TaskExecutor>();

    //    int startingX = (int)playerPos.x / TerrainManager.CHUNK_SIZE;

    //    int x = startingX;
    //    int z = (int)playerPos.y/ TerrainManager.CHUNK_SIZE;
    //    int dx = startingX, dz = -1;


    //    int chunkCount = (TerrainManager.RENDER_DISTANCE / TerrainManager.CHUNK_SIZE);
    //    float xFloor = chunkCount / 2.0f - x;
    //    float xCeiling = chunkCount / 2.0f + x;
    //    float zFloor= chunkCount / 2.0f - z;
    //    float zCeiling= chunkCount / 2.0f + z;

    //    for (int i = 0; i < chunkCount * chunkCount; i++)
    //    {

    //        //float p1 = (chunkCount) / 2.0f;
    //        //float p2 = (chunkCount) / 2.0f;

            
            
    //        if ( ((-xFloor < x) && (x <= xCeiling)) && 
    //             ((-zFloor < z) && (z <= zCeiling)) )
    //        {

    //            //if chunk doesn't exist, create it
    //            var position = new Vector3(x * TerrainManager.CHUNK_SIZE, 0, z * TerrainManager.CHUNK_SIZE);
    //            bool chunkExists;
    //            chunkExists = chunkExistDict.TryGetValue(position, out chunkExists);


    //            if (!chunkExists) 
    //            {
    //                chunkExistDict.Add(position, true);
                    
    //                var go = new GameObject(position.ToString());
    //                go.transform.position = position;
    //                go.transform.SetParent(gameObject.transform);

    //                taskExecutor.ScheduleTask(new Task(delegate
    //                {
    //                    chunkDict.Add(position, new Chunk(go));
    //                }));
    //            }
    //        }
    //        if ( (x == z) ||
    //            ((x < 0) && (x == -z)) || 
    //            ((x > 0) && (x == 1 - z)) )
    //        {
    //            int tmpDX = dx;
    //            dx = -dz;
    //            dz = tmpDX;
    //        }
    //        x = x + dx;
    //        z = z + dz;

    //    }

    //        return new Vector2();
    //}

}