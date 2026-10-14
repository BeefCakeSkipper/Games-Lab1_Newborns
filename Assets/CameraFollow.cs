using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float horizontalLimit = 1.0f;

    private Transform mario;

    void Start()
    {
        if (PlayerMovement.instance != null)
        {
            mario = PlayerMovement.instance.transform;
        }
    }

    void LateUpdate()
    {
        // Reconnect if Mario's reference is missing
        if (mario == null)
        {
            if (PlayerMovement.instance == null)
                return;

            mario = PlayerMovement.instance.transform;
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