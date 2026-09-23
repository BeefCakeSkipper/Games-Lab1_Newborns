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
    private bool onGroundState = true;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    private Vector3 startPosition; // taking it from the scene start
    private JumpOnGoomba scoreValue;
    public bool isOnGround => onGroundState;
    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate =  30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        startPosition = transform.localPosition;
        scoreValue = GetComponent<JumpOnGoomba>();

    }

    // Update is called once per frame
    void Update()
    {
    
      // always check if mario is on the ground so if u fall on the goomba it still gets stomped
      onGroundState = onGroundCheck();

      if (Input.GetKeyDown("a") && faceRightState){
          faceRightState = false;
          marioSprite.flipX = true;
      }

      if (Input.GetKeyDown("d") && !faceRightState){
          faceRightState = true;
          marioSprite.flipX = false;
      }

      if (Input.GetKeyDown("space") && onGroundState){
          marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
      }

    }
    public float maxSpeed = 20;
    // FixedUpdate is called 50 times a second
    void  FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(moveHorizontal) > 0){
            Vector2 movement = new Vector2(moveHorizontal, 0);
            // check if it doesn't go beyond maxSpeed
            if (marioBody.linearVelocity.x < maxSpeed)
                    marioBody.AddForce(movement * speed);
        }

        // stop
        if (Input.GetKeyUp("a") || Input.GetKeyUp("d")){
            // stop
            marioBody.linearVelocity = new Vector2(0, marioBody.linearVelocity.y);
        }

  } 

  void OnCollisionEnter2D(Collision2D col)
  {

      if (col.gameObject.CompareTag("Enemy")
          && transform.position.y - col.transform.position.y < 0.4f)
      {
          Debug.Log("Collided with goomba!");
          Time.timeScale = 0.0f;
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

    public void RestartButtonCallback(int input)
    {
        Debug.Log("Restart!");
        // reset everything
        ResetGame();
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
        scoreValue.score = 0;
        scoreText.text = "Score: 0";
        // reset Goomba
        foreach (Transform eachChild in enemies.transform)
        {
            eachChild.GetComponent<EnemyMovement>().Respawn();
        }

    }

  

}