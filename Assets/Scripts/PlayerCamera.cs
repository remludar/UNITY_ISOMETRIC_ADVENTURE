using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    public float rotationSpeed;
    public float zoomSpeed;
    
    float yRot;

    Transform transform;

    void Start()
    {
        rotationSpeed = 300.0f;
        zoomSpeed = 20f;
        yRot = 0.0f;
        transform = gameObject.transform;
    }

    void Update()
    {
        if (GameManager.instance.gameState == GameManager.GameState.PLAYER_CAM)
        {
            //Rotate Camera
            if (InputManager.isRightMouse)
            {
                yRot = InputManager.mouseDelta.x;
            }
            transform.Translate(new Vector3(-yRot, 0, 0) * rotationSpeed * Time.deltaTime);
            yRot = 0;

            //Zoom Camera
            var scrollDelta = InputManager.mouseScrollDelta.y;

            //Limit scrolling floor/ceiling
            var zoomVector = transform.parent.position - transform.position;
            if (zoomVector.magnitude > 10 && scrollDelta > 0) 
                transform.position += zoomVector * scrollDelta * zoomSpeed * Time.deltaTime;
            if (zoomVector.magnitude < 100 && scrollDelta < 0)
                transform.position += zoomVector * scrollDelta * zoomSpeed * Time.deltaTime;


            
           

        }
        
    }

    void LateUpdate()
    {
        transform.LookAt(gameObject.transform.parent); 
    }
}