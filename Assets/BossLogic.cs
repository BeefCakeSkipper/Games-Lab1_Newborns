using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossLogic : MonoBehaviour
{

    private float originalX;
    private float maxOffset = 8.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;
    private Vector2 velocity;


    private Rigidbody2D enemyBody;
    private SpriteRenderer bossSprite;
    private Vector3 startPosition; // taking it from the scene start
    private Collider2D enemyCollider;

    private Sprite normalSprite;

    private Animator bossAnimator;
    public int maxHealth = 3;
    private int currentHealth;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;
    // public GameObject bossHealth;
    public GameObject[] hearts;
    public PlayerMovement player;
    private bool isDead = false;
    public float skidDuration = 0.9f;
    private bool skidding = false;
    void Start()
    {
        // bossHealth.SetActive(true);
        enemyBody = GetComponent<Rigidbody2D>();
        bossSprite = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
        bossAnimator = GetComponent<Animator>();
        normalSprite = bossSprite.sprite;
        startPosition = transform.localPosition;
        originalX = transform.position.x;
        ComputeVelocity();

        currentHealth = maxHealth;
        UpdateHealth();

    }
    void ComputeVelocity()
    {
        velocity = new Vector2((moveRight) * maxOffset / enemyPatroltime, 0);
    }
    void MoveBoss()
    {
        enemyBody.linearVelocity = new Vector2(velocity.x, enemyBody.linearVelocity.y);
    }

    void FixedUpdate()
    {
        if (moveRight > 0)
        {
            bossSprite.flipX = false;
        }
        else
        {
            bossSprite.flipX = true;
        }

        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {// move Boss
            MoveBoss();
        }
        else
        {
            changeDirection();
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("BossWall") && !skidding)
        {
            skidding = true;
            enemyBody.linearVelocity = Vector2.zero;
            // change direction on collision
            bossAnimator.SetTrigger("HitWall");
            Invoke(nameof(changeDirection), skidDuration);

        }
    }



    public void Respawn()
    {
        CancelInvoke();
        gameObject.SetActive(false);
        isDead = false;
        transform.localPosition = startPosition;
        bossSprite.sprite = normalSprite;
        bossSprite.color = Color.white;
        enemyCollider.enabled = true;
        enemyBody.bodyType = RigidbodyType2D.Dynamic;
        // restart the patrol
        moveRight = -1;
        originalX = transform.position.x;
        enemyBody.linearVelocity = Vector2.zero;
        ComputeVelocity();
        currentHealth = maxHealth;
        UpdateHealth();

    }

    public void TakeDamage()
    {
        if (isDead) return;
        currentHealth = Mathf.Max(currentHealth - 1, 0);
        UpdateHealth();
        bossSprite.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        CancelInvoke(nameof(ResetSprite));
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            Invoke(nameof(ResetSprite), 0.2f);
        }
    }

    public void UpdateHealth()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            SpriteRenderer heartSprite = hearts[i].GetComponent<SpriteRenderer>();
            if (i < currentHealth)
            {
                heartSprite.sprite = fullHeartSprite;
            }
            else
            {
                heartSprite.sprite = emptyHeartSprite;
            }
        }
    }
    public void Die()
    {
        isDead = true;
        enemyBody.linearVelocity = Vector2.zero;
        enemyCollider.enabled = false;
        gameObject.SetActive(false);
        player.ShowEndScreen("You Win!");

    }
    private void changeDirection()
    {
        // change direction
        moveRight *= -1;
        ComputeVelocity();
        MoveBoss();
        skidding = false;
    }
    private void ResetSprite()
    {
        bossSprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

}