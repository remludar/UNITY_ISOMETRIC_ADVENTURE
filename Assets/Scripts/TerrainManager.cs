using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TerrainManager
{
    public const int CHUNK_SIZE = 32;
    public const int RENDER_DISTANCE = 17 * CHUNK_SIZE; 
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
        var playerGO = GameObject.FindGameObjectWithTag("Player");
        terrain.Update(playerGO.transform.position);

        

    }
}