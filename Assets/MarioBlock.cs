using UnityEngine;

public class MarioBlock : MonoBehaviour
{
    public float bounceHeight = 0.5f;
    public float bounceSpeed = 2.0f;

    private Vector3 startPosition;
    private Vector3 topPosition;

    private bool bouncingUp = false;
    private bool bouncingDown = false;
    public Animator blockAnimator;
    public AudioSource blockAudio;
    public AudioClip coinSound;
    private bool used = false;
    public bool reusable = false;
    public bool hasCoin = true;


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
            if (contact.normal.y > 0.5f)
            {
                if (!used && !bouncingUp && !bouncingDown) // So that it doesn't proc during animation
                {
                    bouncingUp = true;

                    if (hasCoin)
                    {
                        blockAnimator.SetTrigger("Spawn");
                        blockAudio.PlayOneShot(coinSound);
                        hasCoin = false;
                    }
                    if (!reusable)
                    {
                        used = true;
                    }

                    
                }

                break;
            }
        }
    }

    public void ResetBlock()
    {
        used = false;
        bouncingUp = false;
        bouncingDown = false;
        transform.position = startPosition;
        if (blockAnimator)
        {
            blockAnimator.ResetTrigger("Spawn");
            blockAnimator.Rebind();  
        }

    }

}