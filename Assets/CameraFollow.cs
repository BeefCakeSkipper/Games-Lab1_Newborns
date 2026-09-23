using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform mario;
    public float horizontalLimit = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void LateUpdate()
    {
        float distanceFromCamera = mario.position.x - transform.position.x;


        if (distanceFromCamera > horizontalLimit)
        {
            transform.position = new Vector3(mario.position.x - horizontalLimit, transform.position.y, transform.position.z);
        }
        else if (distanceFromCamera < -horizontalLimit)
        {
            transform.position = new Vector3(mario.position.x + horizontalLimit, transform.position.y, transform.position.z);
        }
    }
}
