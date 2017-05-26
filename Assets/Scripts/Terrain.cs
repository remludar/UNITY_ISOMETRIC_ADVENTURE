using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Terrain : MonoBehaviour
{
    public static Dictionary<Vector2, float> heightMap = new Dictionary<Vector2, float>();
    List<Chunk> chunks = new List<Chunk>();

    void Start()
    {
        _GenerateHeightMap();
        int startX = -TerrainManager.CHUNK_SIZE;
        int startZ = -TerrainManager.CHUNK_SIZE;
        for (int x = startX; x < TerrainManager.WORLD_SIZE - TerrainManager.CHUNK_SIZE; x+= TerrainManager.CHUNK_SIZE)
        {
            for (int z = startZ; z < TerrainManager.WORLD_SIZE - TerrainManager.CHUNK_SIZE; z+= TerrainManager.CHUNK_SIZE)
            {
                var position = new Vector3(x, 0, z);
                var go = new GameObject(position.ToString());
                go.transform.position = position;
                go.transform.SetParent(gameObject.transform);
                chunks.Add(new Chunk(go));
            }
        }
    }

    void Update()
    {
        InputManager.ProcessInput();
    }

    private void _GenerateHeightMap()
    {
        //Create Heightmap
        var generator = new SimplexNoiseGenerator();

        int startX = -TerrainManager.CHUNK_SIZE;
        int startZ = -TerrainManager.CHUNK_SIZE;
        for (int x = startX; x < TerrainManager.WORLD_SIZE - TerrainManager.CHUNK_SIZE + 1; x++)
        {
            for (int z = startZ; z < TerrainManager.WORLD_SIZE - TerrainManager.CHUNK_SIZE + 1; z++)
            {
                float noise = 0.0f;
                int type = 1; 
                //Mountains
                if (type == 0)
                {
                    noise = generator.coherentNoise(x, 0, z, 1, 100, 50f, 2, 0.9f);
                    noise += generator.coherentNoise(x, 0, z, 2, 75, 50f, 2, 0.9f);
                    noise += generator.coherentNoise(x, 0, z, 2, 10, 10f, 2, 0.9f);
                }
                //Rolling Hills
                else if (type == 1)
                {
                    noise = generator.coherentNoise(x, 0, z, 2, 100, 50, 2, 0.9f);
                }
                heightMap.Add(new Vector2(x, z), noise);

            }
        }
       
    }
}