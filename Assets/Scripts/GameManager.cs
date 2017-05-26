using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public enum GameState { FLY_CAM, PLAYER_CAM };

    public GameState gameState;

    void Awake()
    {
        instance = this;
        gameState = GameState.PLAYER_CAM;
    }

    void Start()
    {
        TerrainManager.Init();
    }

} 