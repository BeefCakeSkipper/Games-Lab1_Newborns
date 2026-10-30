using UnityEngine;

public class Fireball : MonoBehaviour
{
    public GameConstants gameConstants;
    public IntGameEvent onIncrementScore;
    public float speed = 12f;
    public float bounceSpeed = 6f;
    public float lifetime = 3f;

    private Rigidbody2D body;
    private int direction = 1;

    public void Launch(int dir)
    {
        direction = dir;
        GetComponent<SpriteRenderer>().flipX = dir < 0;
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        body.linearVelocity = new Vector2(speed * direction, body.linearVelocity.y);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Enemy"))
        {
            EnemyMovement enemy = col.gameObject.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                enemy.Squash();
                if (onIncrementScore != null && gameConstants != null)
                    onIncrementScore.Raise(gameConstants.fireballKillScore);
            }
            Destroy(gameObject);
            return;
        }

        foreach (ContactPoint2D contact in col.contacts)
        {
            // bounce along the floor, burn out against walls
            if (contact.normal.y > 0.5f)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, bounceSpeed);
                return;
            }
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                Destroy(gameObject);
                return;
            }
        }
    }
}
