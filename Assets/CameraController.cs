using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    public float horizontalLimit = 1.0f;

    private Transform mario;
    private Vector3 startPosition; // Add this line

    void Start()
    {
        startPosition = transform.position;

        player = FindFirstObjectByType<PlayerMovement>();

        if (player != null)
        {
            mario = player.transform;
        }
        else
        {
            Debug.LogError("CameraController: Mario not found in the scene!");
        }
    }

    public void GameRestart()
    {
        transform.position = startPosition;
    }

    public void TestEvent()
    {
        Debug.Log("Camera event triggered!");
    }
    void LateUpdate()
    {
        // Reconnect if Mario's reference is missing
        if (mario == null)
        {
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();

            if (player == null)
                return;

            mario = player.transform;
        }

        float distanceFromCamera = mario.position.x - transform.position.x;

        if (distanceFromCamera > horizontalLimit)
        {
            transform.position = new Vector3(
                mario.position.x - horizontalLimit,
                transform.position.y,
                transform.position.z
            );
        }
        else if (distanceFromCamera < -horizontalLimit)
        {
            transform.position = new Vector3(
                mario.position.x + horizontalLimit,
                transform.position.y,
                transform.position.z
            );
        }
    }
}