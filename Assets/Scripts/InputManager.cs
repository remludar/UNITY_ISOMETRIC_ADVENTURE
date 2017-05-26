using UnityEngine;
using System.Collections;

public static class InputManager
{
    public static bool isW;
    public static bool isA;
    public static bool isS;
    public static bool isD;
    public static bool isE;
    public static bool isQ;
    public static bool isRightMouse;

    public static Vector2 oldMousePosition;
    public static Vector2 newMousePosition;
    public static Vector2 mouseDelta;

    public static void ProcessInput()
    {
        isW = Input.GetKey(KeyCode.W);
        isA = Input.GetKey(KeyCode.A);
        isS = Input.GetKey(KeyCode.S);
        isD = Input.GetKey(KeyCode.D);
        isE = Input.GetKey(KeyCode.E);
        isQ = Input.GetKey(KeyCode.Q);
        isRightMouse = Input.GetMouseButton(1);

        oldMousePosition = newMousePosition;
        newMousePosition = Input.mousePosition;
        mouseDelta = newMousePosition - oldMousePosition;


        if (isRightMouse)
            GameManager.instance.gameState = GameManager.GameState.FLY_CAM;
        else
            GameManager.instance.gameState = GameManager.GameState.PLAYER_CAM;
    }
}