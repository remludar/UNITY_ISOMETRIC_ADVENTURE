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
    public static bool is1;
    public static bool is2;
    public static bool isRightMouse;
    public static bool isEsc;
    public static bool isSpace;

    public static Vector2 mouseDelta;
    public static Vector2 mouseScrollDelta;
    public static float mouseX;

    public static void ProcessInput()
    {
        isW = Input.GetKey(KeyCode.W);
        isA = Input.GetKey(KeyCode.A);
        isS = Input.GetKey(KeyCode.S);
        isD = Input.GetKey(KeyCode.D);
        isE = Input.GetKey(KeyCode.E);
        isQ = Input.GetKey(KeyCode.Q);
        is1 = Input.GetKey(KeyCode.Alpha1);
        is2 = Input.GetKey(KeyCode.Alpha2);
        isEsc = Input.GetKey(KeyCode.Escape);
        isSpace = Input.GetKeyDown(KeyCode.Space);
        isRightMouse = Input.GetMouseButton(1);



        mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        mouseScrollDelta = Input.mouseScrollDelta;
        mouseX = Input.GetAxis("Mouse X");


      
    }
}