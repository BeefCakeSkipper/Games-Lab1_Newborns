using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JumpOnGoomba : MonoBehaviour
{
    public Transform enemyLocation;
    public TextMeshProUGUI scoreText;
    private bool onGroundState;

    [System.NonSerialized]
    public int score = 0; // we don't want this to show up in the inspector

    public Vector3 boxSize;
    public float maxDistance;
    public LayerMask layerMask;
    public float bounceSpeed = 5.0f;   // lil bump after stomp
    private Rigidbody2D marioBody;

    // Start is called before the first frame update
    void Start()
    {
        marioBody = GetComponent<Rigidbody2D>();
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

        // mario stomps the goomba: airborne from a jump, haven't scored yet this
        // jump, and mario is above the goomba (so side hits don't count)
        if (col.gameObject.CompareTag("Enemy") && !onGroundState
            && transform.position.y > col.transform.position.y + 0.4f)
        {
            score++;
            scoreText.text = "Score: " + score.ToString();
            col.gameObject.GetComponent<EnemyMovement>().Squash();
            marioBody.AddForce(Vector2.up * bounceSpeed, ForceMode2D.Impulse);
            Debug.Log(score);
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
