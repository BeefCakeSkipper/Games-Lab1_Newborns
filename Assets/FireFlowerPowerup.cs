using UnityEngine;

public class FireFlowerPowerup : BasePowerup
{
    protected override void Start()
    {
        base.Start();
        this.type = PowerupType.FireFlower;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!spawned)
            return;

        if (col.gameObject.CompareTag("Player"))
        {
            ApplyPowerup(col.gameObject.GetComponent<MarioStateController>());
            gameObject.SetActive(false);
        }
    }

    // the flower doesn't move, it just sits on top of the block once spawned
    public override void SpawnPowerup()
    {
        spawned = true;
        Animator animator = GetComponent<Animator>();
        if (animator != null)
            animator.enabled = false;
        GetComponent<Collider2D>().enabled = true;
    }

    public override void ResetPowerup()
    {
        gameObject.SetActive(true);
        spawned = false;

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
