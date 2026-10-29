
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMushroomPowerup : BasePowerup
{
    public float speed = 3f;

    // setup this object's type
    // instantiate variables
    protected override void Start()
    {
        base.Start(); // call base class Start()
        this.type = PowerupType.MagicMushroom;
    }

    void FixedUpdate()
    {
        // keep sliding at a constant speed once it's out of the box, gravity still pulls it down
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
            // give mario the powerup
            ApplyPowerup(col.gameObject.GetComponent<MarioStateController>());

            // hide instead of destroying so it can be reset on restart
            gameObject.SetActive(false);
        }
        else
        {
            // flip travel direction when hitting a wall/pipe from the side (not when landing on the ground)
            foreach (ContactPoint2D contact in col.contacts)
            {
                if (Mathf.Abs(contact.normal.x) > 0.5f)
                {
                    goRight = !goRight;
                    break;
                }
            }
        }
    }

    // interface implementation
    // called by an animation event at the end of the mushroom's spawn animation
    public override void SpawnPowerup()
    {
        spawned = true;
        // stop the animator from holding the mushroom's position so physics can take over
        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.enabled = false;
        rigidBody.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true; // turned off while it's inside the box
        goRight = true; // move to the right
    }


    // put the mushroom back inside the block
    public override void ResetPowerup()
    {
        gameObject.SetActive(true);
        spawned = false;
        goRight = true;

        // stop it and hand control back to the animator
        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
        transform.localPosition = Vector3.zero;

        // back to the idle animation
        Animator animator = GetComponent<Animator>();
        animator.enabled = true;
        animator.ResetTrigger("spawned");
        animator.Rebind();
    }

    // interface implementation
    public override void ApplyPowerup(MonoBehaviour i)
    {
        MarioStateController mario = i as MarioStateController;
        if (mario != null)
        {
            mario.SetPowerup(powerupType);
        }
    }
}
