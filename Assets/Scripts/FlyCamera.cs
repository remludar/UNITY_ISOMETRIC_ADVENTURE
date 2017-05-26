using UnityEngine;
using System.Collections;

public class FlyCamera : MonoBehaviour
{
    public float moveSpeed;
    public float lookAtSpeed;
    float yRot;
    float xRot;
    
    
    Transform transform;

    void Start()
    {
        moveSpeed = 0.5f;
        lookAtSpeed = 1.0f;
        yRot = 40.0f;
        xRot = 0.0f;
        transform = gameObject.transform;
    }

    void Update()
    {
        if (GameManager.instance.gameState == GameManager.GameState.FLY_CAM &&
            InputManager.isRightMouse)
        {
            var mouseDelta = InputManager.mouseDelta;
            yRot += mouseDelta.x;
            xRot += -mouseDelta.y;

            if (InputManager.isW)
            {
                transform.Translate(Vector3.forward * moveSpeed);
            }
            if (InputManager.isA)
            {
                transform.Translate(Vector3.left * moveSpeed);
            }
            if (InputManager.isS)
            {
                transform.Translate(Vector3.back * moveSpeed);
            }
            if (InputManager.isD)
            {
                transform.Translate(Vector3.right * moveSpeed);
            }
            if (InputManager.isQ)
            {
                transform.Translate(Vector3.down * moveSpeed);
            }
            if (InputManager.isE)
            {
                transform.Translate(Vector3.up * moveSpeed);
            }

        }
        transform.rotation = Quaternion.Euler(new Vector3(xRot, yRot, 0.0f) * lookAtSpeed);



       
       
    }
}