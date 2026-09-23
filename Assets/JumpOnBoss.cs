using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JumpOnBoss : MonoBehaviour
{
    public Transform enemyLocation;
    // public TextMeshProUGUI scoreText;
    private bool onGroundState;

    // [System.NonSerialized]
    // public int score = 0; // we don't want this to show up in the inspector

    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;
    public float bounceSpeed = 5.0f;   // lil bump after stomp
    private Rigidbody2D marioBody;
    private PlayerMovement playerMovement;

    // Start is called before the first frame update
    void Start()
    {
        marioBody = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        // mario jumps -- input must be polled here, not in FixedUpdate
        if (Input.GetKeyDown("space") && onGroundCheck())
        {
            onGroundState = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground")) onGroundState = true;

        // mario stomps the Boss: airborne from a jump, haven't scored yet this
        // jump, and mario is above the goomba (so side hits don't count)
        // Mario collides with Boss
        if (col.gameObject.CompareTag("Boss"))
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


    private bool onGroundCheck()
    {
        if (Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, maxDistance, layerMask))
        {
            Debug.Log("on ground");
            return true;
        }
        else
        {
            Debug.Log("not on ground");
            return false;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position - transform.up * maxDistance, boxSize);
    }
}
