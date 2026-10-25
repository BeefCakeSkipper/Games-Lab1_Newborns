using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private GameConstants constants;
    private float originalX;
    private float maxOffset;        // set from GameConstants
    private float enemyPatroltime;  // set from GameConstants
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

        // patrol values come from the game constants
        // GameConstants constants = GameManager.instance.gameConstants;
        maxOffset = constants.goombaMaxOffset;
        enemyPatroltime = constants.goombaPatrolTime;
        ComputeVelocity();
    }



    void ComputeVelocity()
    {
        if (enemyPatroltime <= 0f)
        {
            Debug.LogError(
                $"Invalid goombaPatrolTime: {enemyPatroltime} on {gameObject.name}",
                this
            );

            velocity = Vector2.zero;
            return;
        }

        velocity = new Vector2(moveRight * maxOffset / enemyPatroltime, 0);
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

        float distanceFromStart = enemyBody.position.x - originalX;

        if ((moveRight > 0 && distanceFromStart >= maxOffset) ||
            (moveRight < 0 && distanceFromStart <= -maxOffset))
        {
            moveRight *= -1;
            ComputeVelocity();
        }

        Movegoomba();
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
    public void GameRestart()
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