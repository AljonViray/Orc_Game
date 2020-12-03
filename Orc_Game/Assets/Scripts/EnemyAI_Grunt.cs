using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI_Grunt : MonoBehaviour
{
    Rigidbody2D rb;
    Transform player;
    BoxCollider2D hitbox;
    BoxCollider2D visionArea;
    CircleCollider2D attackArea;
    Transform wallDetector;
    Transform groundDetector;

    public float normalMoveSpeed;
    public float chasingMoveSpeed;
    public float jumpForce;
    public bool sawPlayer;
    public bool grounded;
    public float turnTime;
    private float time;

    public enum Mode {Standing, StandAndTurn, Patrolling, Chasing};
    public Mode mode;
    public enum Direction {Left, Right};
    public Direction direction;


    // Unity Functions
    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("PlayerCharacter").transform;
        hitbox = GetComponent<BoxCollider2D>();
        visionArea = transform.GetChild(0).GetComponent<BoxCollider2D>();
        attackArea = transform.GetChild(1).GetComponent<CircleCollider2D>();
        wallDetector = transform.GetChild(2);
        groundDetector = transform.GetChild(3);
        sawPlayer = false;
        grounded = true;

        // All enemies start by standing, then adjust in Update()
        Stand();
    }


    void FixedUpdate() 
    {
        time += Time.deltaTime;
        
        // Test surroundings
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetector.position, Vector2.down, 0.1f);
        RaycastHit2D wallInfo = Physics2D.Raycast(wallDetector.position, Vector2.left, 0.1f);
        if (direction == Direction.Right) {
            wallInfo = Physics2D.Raycast(wallDetector.position, Vector2.right, 0.1f);
        }

        // Check if grounded to allow jumping
        if (groundInfo.collider == true && groundInfo.collider.CompareTag("Ground")) {
            grounded = true;
        }
        else {
            grounded = false;
        }

        // Regardless of inital mode, follow and attack player forever if they are seen
        if (sawPlayer) {
            // Do a little hop to show alert
            if (mode != Mode.Chasing) {
                rb.AddForce(Vector2.up * jumpForce/2);
                mode = Mode.Chasing;
            }
            else {
                Chasing(groundInfo, wallInfo);
            }
        }

        // If mode is Standing, do nothing here.
        // Stand in place, switching directions every X seconds
        if (mode == Mode.StandAndTurn) {
            StandAndTurn();
        }
        // Patrol, starting with a given direction, and change directions when seeing a wall/pit
        else if (mode == Mode.Patrolling) {
            Patrol(groundInfo, wallInfo);
        }
    }

    // Detect player in enemy's vision area
    void OnTriggerEnter2D (Collider2D col) 
    {
        // PlayerCharacter was given tag "Player" in editor
        if (col.CompareTag("Player")) 
        {
            // Activate follow mode if it sees the player, even if player leaves vision
            print("Player spotted!");
            sawPlayer = true;
        }
    }



    // Private Functions
    private void Stand() 
    {
        if (direction == Direction.Left) {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }    
    }


    private void StandAndTurn() 
    {
        // Alternate between left and right directions at constant rate
        if (time >= turnTime) {
            if (direction == Direction.Left) {
                transform.eulerAngles = new Vector3(0, 180, 0);
                direction = Direction.Right;
            }
            else {
                transform.eulerAngles = new Vector3(0, 0, 0);
                direction = Direction.Left;
            }
            time = 0;  
        }
    }


    private void Patrol(RaycastHit2D groundInfo, RaycastHit2D wallInfo) 
    {
        // Move in a given direction
        transform.Translate(Vector2.left * normalMoveSpeed * Time.deltaTime);

        // If a pit or a wall is detected ahead, turn around
        if (groundInfo.collider == false || wallInfo.collider == true) {
            if (direction == Direction.Left) {
                transform.eulerAngles = new Vector3(0, 180, 0);
                direction = Direction.Right;
            }
            else {
                transform.eulerAngles = new Vector3(0, 0, 0);
                direction = Direction.Left;
            }
        }
    }


    private void Chasing(RaycastHit2D groundInfo, RaycastHit2D wallInfo) 
    {
        transform.Translate(Vector2.left * chasingMoveSpeed * Time.deltaTime);

        // If a wall is detected ahead, try to jump (can cause enemies to fall into pits but it's funny)
        if (direction == Direction.Left) {
            if (wallInfo.collider == true && grounded) {
                rb.AddForce(Vector2.up * jumpForce);
            }
        }
        else {
            if (wallInfo.collider == true && grounded) {
                rb.AddForce(Vector2.up * jumpForce);
            }
        }
    }
}
