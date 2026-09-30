using UnityEngine;

public class MarioBlock : MonoBehaviour
{
    public float bounceHeight = 0.5f;
    public float bounceSpeed = 2.0f;

    private Vector3 startPosition;
    private Vector3 topPosition;

    private bool bouncingUp = false;
    private bool bouncingDown = false;

    void Start()
    {
        startPosition = transform.position;
        topPosition = startPosition + Vector3.up * bounceHeight;
    }

    void Update()
    {
        if (bouncingUp)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                topPosition,
                bounceSpeed * Time.deltaTime
            );

            if (transform.position == topPosition)
            {
                bouncingUp = false;
                bouncingDown = true;
            }
        }

        if (bouncingDown)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                startPosition,
                bounceSpeed * Time.deltaTime
            );

            if (transform.position == startPosition)
            {
                bouncingDown = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Hit from underneath
            Debug.Log(contact.normal.y);
            if (contact.normal.y > 0.5f)
            {
                if (!bouncingUp && !bouncingDown) // So that it doesn't proc during animation
                {
                    bouncingUp = true;
                }

                break;
            }
        }
    }
}