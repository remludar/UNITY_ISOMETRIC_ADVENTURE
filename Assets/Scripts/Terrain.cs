using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Terrain : MonoBehaviour
{
    List<Chunk> chunks = new List<Chunk>();

    void Start()
    {
        _NextChunkInSpiral();
    }

    void Update()
    {
        InputManager.ProcessInput();
    }

    private Vector2 _NextChunkInSpiral()
    {
        var taskExecutor = gameObject.AddComponent<TaskExecutor>();

        int x = 0, z = 0;
        int dx = 0, dz = -1;

        int chunkCount = (TerrainManager.WORLD_SIZE / TerrainManager.CHUNK_SIZE);
        for (int i = 0; i < chunkCount * chunkCount; i++)
        {

            int p1 = -chunkCount / 2;
            int p2 = chunkCount / 2;
            

            if ( ((-chunkCount / 2.0f < x) && (x <= chunkCount / 2.0f)) && 
                 ((-chunkCount / 2.0f < z) && (z <= chunkCount / 2.0f)) )
            {

                var position = new Vector3(x * TerrainManager.CHUNK_SIZE, 0, z * TerrainManager.CHUNK_SIZE);
                var go = new GameObject(position.ToString());
                go.transform.position = position;
                go.transform.SetParent(gameObject.transform);


                taskExecutor.ScheduleTask(new Task(delegate
                {
                    chunks.Add(new Chunk(go));
                }));

            }
            if ( (x == z) ||
                ((x < 0) && (x == -z)) || 
                ((x > 0) && (x == 1 - z)) )
            {
                int tmpDX = dx;
                dx = -dz;
                dz = tmpDX;
            }
            x = x + dx;
            z = z + dz;

        }

            return new Vector2();
    }

}