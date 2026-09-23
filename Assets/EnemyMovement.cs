using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    private float originalX;
    private float maxOffset = 5.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;
    private Vector2 velocity;

    private Rigidbody2D enemyBody;
    private SpriteRenderer goombaSprite;
    private Vector3 startPosition; // taking it from the scene start

    private Collider2D enemyCollider;
    private Sprite normalSprite;
    private bool squashed = false;

    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        goombaSprite = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
        normalSprite = goombaSprite.sprite;
        startPosition = transform.localPosition;
        originalX = transform.position.x;
        ComputeVelocity();
    }
    void ComputeVelocity()
    {
        velocity = new Vector2((moveRight) * maxOffset / enemyPatroltime, 0);
    }
    void Movegoomba()
    {
        enemyBody.linearVelocity = new Vector2(velocity.x, enemyBody.linearVelocity.y);
    }

    void FixedUpdate()
    {
        if (squashed) return;

        if (moveRight > 0)
        {
            goombaSprite.flipX = false;
        }
        else
        {
            goombaSprite.flipX = true;
        }

        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {// move goomba
            Movegoomba();
        }
        else
        {
            // change direction
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (squashed) return;
        // change direction on collision
        moveRight *= -1;
        ComputeVelocity();
        Movegoomba();

    }
    public Sprite squashedSprite;
    public float despawnDelay = 0.5f;

    public void Squash()
    {
        squashed = true;
        goombaSprite.sprite = squashedSprite;
        enemyBody.bodyType = RigidbodyType2D.Static;
        enemyCollider.enabled = false;
        // hide when squashed
        Invoke(nameof(Hide), despawnDelay);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Respawn()
    {
        CancelInvoke();
        squashed = false;
        gameObject.SetActive(true);
        transform.localPosition = startPosition;
        goombaSprite.sprite = normalSprite;
        enemyCollider.enabled = true;
        enemyBody.bodyType = RigidbodyType2D.Dynamic;
        // restart the patrol
        moveRight = -1;
        originalX = transform.position.x;
        ComputeVelocity();
    }

}