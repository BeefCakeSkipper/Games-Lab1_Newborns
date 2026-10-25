
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossLogic : MonoBehaviour
{
    [Header("ScriptableObject References")]
    [SerializeField] private GameConstants gameConstants;

    [Header("Boss UI")]
    [SerializeField] private GameObject bossHealthUI;
    // Boss movement
    private float originalX;
    private float maxOffset;
    private float enemyPatroltime;
    private int moveRight = -1;
    private Vector2 velocity;

    // Components
    private Rigidbody2D enemyBody;
    private SpriteRenderer bossSprite;
    private Vector3 startPosition;
    private Collider2D enemyCollider;
    private Sprite normalSprite;
    private Animator bossAnimator;

    [Header("Boss Health")]
    public int maxHealth = 3;
    private int currentHealth;
    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;
    public GameObject[] hearts;

    [Header("Boss References")]
    public PlayerMovement player;
    public AudioSource bossMusic;
    public GameObject castle;

    [Header("Boss Movement Settings")]
    public float skidDuration = 0.9f;

    // Boss state
    private bool isDead = false;
    public bool IsDead => isDead;
    private bool skidding = false;
    private bool isInitialized = false;

    private void Awake()
    {
        InitializeBoss();
    }

    private void InitializeBoss()
    {
        if (isInitialized)
            return;

        enemyBody = GetComponent<Rigidbody2D>();
        bossSprite = GetComponent<SpriteRenderer>();
        enemyCollider = GetComponent<Collider2D>();
        bossAnimator = GetComponent<Animator>();

        normalSprite = bossSprite.sprite;
        startPosition = transform.localPosition;
        originalX = transform.position.x;

        if (gameConstants == null)
        {
            Debug.LogError("BossLogic: GameConstants is not assigned!", this);
            return;
        }

        maxOffset = gameConstants.bossMaxOffset;
        enemyPatroltime = gameConstants.bossPatrolTime;

        if (enemyPatroltime <= 0f)
        {
            Debug.LogError("BossLogic: Boss patrol time must be greater than zero!", this);
            return;
        }

        ComputeVelocity();

        currentHealth = maxHealth;
        UpdateHealth();

        isInitialized = true;
    }

    private void ComputeVelocity()
    {
        velocity = new Vector2(
            moveRight * maxOffset / enemyPatroltime,
            0f
        );
    }

    private void MoveBoss()
    {
        enemyBody.linearVelocity = new Vector2(
            velocity.x,
            enemyBody.linearVelocity.y
        );
    }

    private void FixedUpdate()
    {
        if (!isInitialized || isDead || skidding)
            return;

        bossSprite.flipX = moveRight < 0;

        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {
            MoveBoss();
        }
        else
        {
            ChangeDirection();
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!isInitialized || isDead)
            return;

        if (col.gameObject.CompareTag("BossWall") && !skidding)
        {
            skidding = true;

            enemyBody.linearVelocity = Vector2.zero;

            bossAnimator.SetTrigger("HitWall");

            Invoke(nameof(ChangeDirection), skidDuration);
        }
    }

    public void Respawn()
    {
        InitializeBoss();

        if (!isInitialized)
            return;

        CancelInvoke();

        // Reset boss state.
        isDead = false;
        skidding = false;

        // Reset physics.
        enemyBody.linearVelocity = Vector2.zero;
        enemyBody.angularVelocity = 0f;
        enemyBody.bodyType = RigidbodyType2D.Dynamic;
        enemyBody.simulated = true;

        // Restore original position.
        transform.localPosition = startPosition;

        // Restore sprite.
        bossSprite.sprite = normalSprite;
        bossSprite.color = Color.white;
        bossSprite.flipX = true;

        // Restore collision.
        enemyCollider.enabled = true;

        // Reset patrol.
        moveRight = -1;
        originalX = transform.position.x;
        ComputeVelocity();

        // Reset health.
        currentHealth = maxHealth;
        UpdateHealth();

        // Reset animator.
        if (bossAnimator != null)
        {
            bossAnimator.Rebind();
            bossAnimator.Update(0f);
        }

        // Hide castle until boss is defeated.
        if (castle != null)
            castle.SetActive(false);

        // Boss remains hidden until triggered.
        gameObject.SetActive(false);
    }

    public void TakeDamage()
    {
        if (!isInitialized || isDead)
            return;

        currentHealth = Mathf.Max(currentHealth - 1, 0);

        UpdateHealth();

        // Darken boss when damaged.
        bossSprite.color = new Color(
            0.5f,
            0.5f,
            0.5f,
            1f
        );

        CancelInvoke(nameof(ResetSprite));

        if (currentHealth <= 0)
        {
            if (bossMusic != null)
                bossMusic.Stop();

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
            if (hearts[i] == null)
            {
                Debug.LogWarning($"[BossLogic] Heart {i} is not assigned!");
                continue;
            }

            Image heartImage = hearts[i].GetComponent<Image>();

            if (heartImage == null)
            {
                Debug.LogWarning(
                    $"[BossLogic] {hearts[i].name} has no Image component!"
                );
                continue;
            }

            heartImage.sprite = i < currentHealth
                ? fullHeartSprite
                : emptyHeartSprite;

            heartImage.color = Color.white;
            heartImage.enabled = true;
        }

    }


    public void Die()
    {
        if (isDead)
            return;

        if (bossHealthUI != null)
            bossHealthUI.SetActive(false);
        isDead = true;
        skidding = false;

        CancelInvoke();

        enemyBody.linearVelocity = Vector2.zero;
        enemyCollider.enabled = false;

        if (bossMusic != null)
            bossMusic.Stop();

        // Reveal castle when boss is defeated.
        if (castle != null)
            castle.SetActive(true);

        gameObject.SetActive(false);
    }

    private void ChangeDirection()
    {
        if (isDead)
            return;

        moveRight *= -1;

        ComputeVelocity();
        MoveBoss();

        skidding = false;
    }

    private void ResetSprite()
    {
        if (bossSprite != null)
            bossSprite.color = Color.white;
    }
}
