using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    private Rigidbody2D marioBody;

    public float upSpeed = 2;
    public GameObject enemies;
    public TextMeshProUGUI scoreText;
    public CameraFollow cameraFollow;
    public BossLogic boss;
    public int score = 0;
    public BossTrigger bossTrigger;
    public GameObject gameOverScreen;
    public TextMeshProUGUI resultText;
    private bool onGroundState = true;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    private Vector3 startPosition; // taking it from the scene start
    public bool isOnGround => onGroundState;
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate = 30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        startPosition = transform.localPosition;
        // scoreValue = GetComponent<JumpOnGoomba>();

    }

    // Update is called once per frame
    void Update()
    {
        // always check if mario is on the ground so if u fall on the goomba it still gets stomped
        onGroundState = onGroundCheck();

        if (Input.GetKeyDown("a") && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
        }

        if (Input.GetKeyDown("d") && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
        }

        if (Input.GetKeyDown("space") && onGroundState)
        {
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
        }

    }
    public float maxSpeed = 20;
    // FixedUpdate is called 50 times a second
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(moveHorizontal) > 0)
        {
            Vector2 movement = new Vector2(moveHorizontal, 0);
            // check if it doesn't go beyond maxSpeed
            if (marioBody.linearVelocity.x < maxSpeed)
                marioBody.AddForce(movement * speed);
        }

        // stop
        if (Input.GetKeyUp("a") || Input.GetKeyUp("d"))
        {
            // stop
            marioBody.linearVelocity = new Vector2(0, marioBody.linearVelocity.y);
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // grounding is handled by onGroundCheck() now, not by collisions
        if (col.gameObject.CompareTag("Enemy")
            && transform.position.y - col.transform.position.y < 0.4f)
        {
            Debug.Log("Collided with goomba!");
            ShowEndScreen("GAME OVER");
        }
        if (col.gameObject.CompareTag("Boss"))
        {
            ContactPoint2D contact = col.GetContact(0);
            if (contact.normal.y < 0.7f)
            {
                Debug.Log("Collided with Tralalero Tralala! Game Over!");
                ShowEndScreen("GAME OVER");
            }
        }
    }

    private bool onGroundCheck()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, maxDistance, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    }

    // shows the end screen
    public void ShowEndScreen(string message)
    {
        resultText.text = message;
        gameOverScreen.SetActive(true);
        bossTrigger.bossHealth.SetActive(false);
        Time.timeScale = 0.0f;
    }

    public void RestartButtonCallback(int input)
    {
        Debug.Log("Restart!");
        gameOverScreen.SetActive(false);
        // reset everything
        ResetGame();
        cameraFollow.enabled = true;

        // resume time
        Time.timeScale = 1.0f;
    }

    private void ResetGame()
    {
        // reset position
        marioBody.transform.localPosition = startPosition;
        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;
        // reset score
        score = 0;
        scoreText.text = "Score: 0";
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.GetComponent<EnemyMovement>().Respawn();
        }
        boss.Respawn();
        bossTrigger.ResetTrigger();

    }
    public void AddScore()
    {
        score++;
        scoreText.text = "Score: " + score.ToString();
        Debug.Log(score);
    }



}