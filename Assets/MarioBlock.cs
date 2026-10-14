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
    private bool startHasCoin;
    public BasePowerup powerup;       // leave empty for coin blocks
    public Animator powerupAnimator;  // the powerup's animator
    public AudioClip powerupAppearsSound;


    void Start()
    {
        startHasCoin = hasCoin;
        startPosition = transform.position;
        topPosition = startPosition + Vector3.up * bounceHeight;

        // reset this block (and its powerup) when the game restarts
        GameManager.instance.gameRestart.AddListener(ResetBlock);
    }

    void OnDestroy()
    {
        // GameManager survives scene changes, so stop listening when this block is gone
        if (GameManager.instance != null)
            GameManager.instance.gameRestart.RemoveListener(ResetBlock);
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
        // only mario can hit the block so the star doesnt activate it
        if (!collision.gameObject.CompareTag("Player"))
            return;

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
                        if (blockAnimator != null && blockAnimator.runtimeAnimatorController != null)
                            blockAnimator.SetTrigger("Spawn");
                        if (blockAudio != null && coinSound != null)
                            blockAudio.PlayOneShot(coinSound);
                        hasCoin = false;
                    }
                    if (powerup != null && !powerup.hasSpawned)
                    {
                        // block plays its "used" animation
                        if (blockAnimator != null && blockAnimator.runtimeAnimatorController != null)
                            blockAnimator.SetTrigger("Spawn");
                        // powerup rises out of the block
                        if (powerupAnimator != null)
                            powerupAnimator.SetTrigger("spawned");
                        if (blockAudio != null && powerupAppearsSound != null)
                            blockAudio.PlayOneShot(powerupAppearsSound);
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
        hasCoin = startHasCoin;
        bouncingUp = false;
        bouncingDown = false;
        transform.position = startPosition;
        if (blockAnimator != null)
        {
            blockAnimator.ResetTrigger("Spawn");
            blockAnimator.Rebind();
        }
        // put the powerup back inside the block
        if (powerup != null)
        {
            powerup.ResetPowerup();
        }

    }

}