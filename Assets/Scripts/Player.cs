using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float gravity;

    CharacterController cc;
    Camera playerCamera;

    void Awake()
    {
        moveSpeed = 30.0f;
        gravity = -5.0f;
        cc = gameObject.AddComponent<CharacterController>();
        cc.slopeLimit = 90;
        playerCamera = gameObject.GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (GameManager.instance.gameState == GameManager.GameState.PLAYER_CAM)
        {
            playerCamera.enabled = true;
            _Move();
        }
        else
        {
            playerCamera.enabled = false;
        }
    }

    private void _Move()
    {
        var moveDirection = new Vector3();
        var forward = playerCamera.transform.forward;
        forward.y = 0;
        var right = playerCamera.transform.right;
        forward.Normalize();
        right.Normalize();

        if (InputManager.isW)
        {
            moveDirection += forward;
        }
        if (InputManager.isA)
        {
            moveDirection -= right;
        }
        if (InputManager.isS)
        {
            moveDirection -= forward;
        }
        if (InputManager.isD)
        {
            moveDirection += right;
        }

        moveDirection.y += gravity;
        cc.Move(moveDirection * moveSpeed * Time.deltaTime);

    }
}