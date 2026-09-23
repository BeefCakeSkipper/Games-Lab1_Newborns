using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JumpOnBoss : MonoBehaviour
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
        // mario stomps the Boss: airborne, and landing clearly on top of it.
        // usinng ground state from playermovement
        if (col.gameObject.CompareTag("Boss") && !playerMovement.isOnGround)
        {
            ContactPoint2D contact = col.GetContact(0);

            // Only count as a stomp if Mario lands clearly on top
            if (contact.normal.y >= 0.7f)
            {
                Debug.Log("Boss has been stomped");
                playerMovement.AddScore();
                col.gameObject.GetComponent<BossLogic>().TakeDamage();
                marioBody.AddForce(Vector2.up * bounceSpeed, ForceMode2D.Impulse);
            }
        }
    }
}
