using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Terrain : MonoBehaviour
{
    public static Dictionary<Vector2, float> heightMap = new Dictionary<Vector2, float>();

    List<Chunk> chunks = new List<Chunk>();

    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCol;

    FlyCamera flyCam;

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
        #region old
        //int startX = -TerrainManager.CHUNK_SIZE;
        //int startZ = -TerrainManager.CHUNK_SIZE;
        //for (int x = startX; x < TerrainManager.WORLD_SIZE - TerrainManager.CHUNK_SIZE + 1; x++)
        //{
        //    for (int z = startZ; z < TerrainManager.WORLD_SIZE - TerrainManager.CHUNK_SIZE + 1; z++)
        //    {
        //        #region example
        //        //float noise = _PerlinNoise(new Vector2(x, z), 2.0f, 10.0f);
        //        //noise += _PerlinNoise(new Vector2(x, z), 5.0f, 40.0f);
        //        //noise += _PerlinNoise(new Vector2(x, z), 15.0f, 10.0f);
        //        //noise += _PerlinNoise(new Vector2(x, z), 50.0f, 5.0f);
        //        //heightMap.Add(new Vector2(x, z), noise);
        //        #endregion

        //        float noise = _PerlinNoise(new Vector2(x, z), 10.0f, 1.0f);
        //        heightMap.Add(new Vector2(x, z), noise);
        //    }
        //}
        #endregion
        var generator = new SimplexNoiseGenerator();
        

        int startX = -TerrainManager.CHUNK_SIZE;
        int startZ = -TerrainManager.CHUNK_SIZE;
        for (int x = startX; x < TerrainManager.WORLD_SIZE - TerrainManager.CHUNK_SIZE + 1; x++)
        {
            for (int z = startZ; z < TerrainManager.WORLD_SIZE - TerrainManager.CHUNK_SIZE + 1; z++)
            {
                float noise = generator.coherentNoise(x, 0, z, 1, 100, 50f, 2, 0.9f);
                noise += generator.coherentNoise(x, 0, z, 2, 75, 50f, 2, 0.9f);
                noise += generator.coherentNoise(x, 0, z, 2, 10, 10f, 2, 0.9f);
                heightMap.Add(new Vector2(x, z), noise);
            }
        }
       
    }

    private float _PerlinNoise(Vector2 point, float freq, float scale)
    {
        float ret = Mathf.PerlinNoise( (float)point.x  / 125.0f * freq, (float)point.y / 125 * freq) * scale;
        return ret;
    }
}