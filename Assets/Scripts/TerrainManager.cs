using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TerrainManager
{
    public const int CHUNK_SIZE = 32;
    public const int RENDER_DISTANCE = 23 * CHUNK_SIZE; 
    public const float QUAD_SIZE = 1.0f;

    static Terrain terrain;

    public static void Init()
    {
        _GenerateStartingChunks();
    }

    private static void _GenerateStartingChunks()
    {
        terrain = new Terrain(new GameObject("Terrain"));
    }

    public static void UpdateTerrain()
    {
        //See if we need new chunks and generate them
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        var playerPos = new Vector2(playerGO.transform.position.x, playerGO.transform.position.z);
        terrain.GenerateMissingChunks(playerPos);

        //See if any of the chunks need to be updated
        foreach (KeyValuePair<Vector3, Chunk> kvp in terrain.chunkDict)
        {
            var chunk = kvp.Value;
            if (chunk.needsUpdate)
            {
                //do stuff
                chunk.needsUpdate = false;
            }
        }
    }
}