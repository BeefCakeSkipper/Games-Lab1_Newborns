using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JumpOnGoomba : MonoBehaviour
{
    public Transform enemyLocation;
    // public TextMeshProUGUI scoreText;

    // [System.NonSerialized]
    // public int score = 0; // we don't want this to show up in the inspector

    public float bounceSpeed = 5.0f;   // lil bump after stomp
    private Rigidbody2D marioBody;
    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        marioBody = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // mario stomps the goomba: airborne from a jump, haven't scored yet this
        // jump, and mario is above the goomba (so side hits don't count)
        if (col.gameObject.CompareTag("Enemy") && !playerMovement.isOnGround
            && transform.position.y > col.transform.position.y + 0.4f)
        {
            // score++;
            // scoreText.text = "Score: " + score.ToString();
            playerMovement.AddScore();
            col.gameObject.GetComponent<EnemyMovement>().Squash();
            marioBody.AddForce(Vector2.up * bounceSpeed, ForceMode2D.Impulse);
        }
    }


}
