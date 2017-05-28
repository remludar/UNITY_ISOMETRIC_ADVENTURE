using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    public float gravity;

    CharacterController cc;
    Camera playerCamera;

    bool spawnSet = false;

    void Awake()
    {
        moveSpeed = 60.0f;
        gravity = 0.0f;
        cc = gameObject.AddComponent<CharacterController>();
        cc.slopeLimit = 90;
        playerCamera = gameObject.GetComponentInChildren<Camera>();
        transform.position = new Vector3(TerrainManager.CHUNK_SIZE / 2, 200.0f, TerrainManager.CHUNK_SIZE / 2);

    }

    void Start()
    {
       

        
    }

    void Update()
    {
        if (!spawnSet) _SetSpawn();


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

    private void _SetSpawn()
    {
        {
            //see if there's anything below, if yes, enable gravity and go 10 units above, if no wait
            bool safeToSpawn = false;
            RaycastHit hit;
            var ray = new Ray(transform.position, Vector3.down);
            safeToSpawn = Physics.Raycast(new Vector3(0, 30, 0), Vector3.down, out hit);
            if (safeToSpawn)
            {
                float playerSpawnY = hit.collider.transform.position.y + 10;
                transform.position = new Vector3(transform.position.x, playerSpawnY, transform.position.z);
                gravity = -5.0f;
                spawnSet = true;
            }
        }
    }
}