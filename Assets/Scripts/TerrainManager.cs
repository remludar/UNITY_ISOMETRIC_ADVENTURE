using UnityEngine;
using System.Collections;

public static class TerrainManager
{
    public const int CHUNK_SIZE = 50;
    public const int WORLD_SIZE = 3 * CHUNK_SIZE;
    public const float QUAD_SIZE = 1.0f;

    static Terrain terrain;

    public static void Init()
    {
        _GenerateStartingChunks();
    }

    private static void _GenerateStartingChunks()
    {
        GameObject terrainGO = new GameObject("Terrain");
        terrainGO.transform.position = new Vector3(0, 0, 0);
        terrainGO.AddComponent<Terrain>();


    }
}