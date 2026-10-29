using UnityEngine;

public class StarmanPowerup : BasePowerup
{
    public float speed = 3f;
    public float bounceSpeed = 3f; // how high the star bounces off the ground

    // setup this object's type
    protected override void Start()
    {
        base.Start(); // call base class Start()
        this.type = PowerupType.StarMan;
    }

    void FixedUpdate()
    {
        // keep moving sideways at a constant speed once it's out of the box
        if (spawned)
        {
            rigidBody.linearVelocity = new Vector2(speed * (goRight ? 1 : -1), rigidBody.linearVelocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!spawned)
            return;

        if (col.gameObject.CompareTag("Player"))
        {
            ApplyPowerup(col.gameObject.GetComponent<MarioStateController>());

            gameObject.SetActive(false);
            return;
        }

        foreach (ContactPoint2D contact in col.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, bounceSpeed);
                break;
            }
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                goRight = !goRight;
                break;
            }
        }
    }

    // called by an animation event at the end of the star's spawn animation
    public override void SpawnPowerup()
    {
        spawned = true;
        // stop the animator from holding the star's position so physics can take over
        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.enabled = false;
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true; // turned off while it's inside the box
        goRight = true; // move to the right
    }

    // put the star back inside the block, ready to spawn again
    public override void ResetPowerup()
    {
        gameObject.SetActive(true);
        spawned = false;
        goRight = true;

        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
        transform.localPosition = Vector3.zero;

        Animator animator = GetComponent<Animator>();
        animator.enabled = true;
        animator.ResetTrigger("spawned");
        animator.Rebind();
    }

    public override void ApplyPowerup(MonoBehaviour i)
    {
        MarioStateController mario = i as MarioStateController;
        if (mario != null)
        {
            mario.SetPowerup(powerupType);
        }
    }
}
