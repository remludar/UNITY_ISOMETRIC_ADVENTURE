using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    Terrain terrain;

    void Start()
    {
        terrain = new Terrain();
        terrain.Generate();
    }

    void Update()
    {
        InputManager.ProcessInput();
        terrain.Update();
        
    }
	
}
