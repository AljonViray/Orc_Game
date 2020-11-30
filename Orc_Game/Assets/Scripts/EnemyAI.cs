using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    Rigidbody2D rb;
    Transform player;
    BoxCollider2D hitbox;
    BoxCollider2D visionArea;
    CircleCollider2D attackArea;
    Transform turnDetector;
    public float moveSpeed = 0.01f;
    public float attackPower = 100;
    public float jumpForce = 10;
    public bool sawPlayer = false;
    public enum Mode {Standing, StandAndTurn, Patrolling};
    public Mode startingMode;
    public enum Direction {Left, Right};
    public Direction direction;


    // Unity Functions
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("PlayerCharacter").transform;
        hitbox = GetComponent<BoxCollider2D>();
        visionArea = transform.GetChild(0).GetComponent<BoxCollider2D>();
        attackArea = transform.GetChild(1).GetComponent<CircleCollider2D>();
        turnDetector = transform.GetChild(2);

        // Change script's direction based on initial position of the enemy AI, default to Left
        if (direction == Direction.Left) {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else {
            transform.eulerAngles = new Vector3(0, -180, 0);
        }
    }
    
    void Update() {
        Patrol();

        // Check if startingMode is Standing or Patrolling
        if (startingMode == Mode.Standing) {
            // Stand in place, facing a given direction
            if (direction == Direction.Left) {
                //
            }
        }
        else if (startingMode == Mode.StandAndTurn) {
            // Stand in place, switching directions every X seconds
        }
        else if (startingMode == Mode.Standing) {
            // Patrol, starting with a given direction, and change directions when seeing a wall/pit
        }

        // Regardless of starting mode, follow and attack player if they are seen
        if (sawPlayer) {
        }
    }

    // Detect player in enemy's vision area
    void OnTriggerEnter2D (Collider2D col) 
    {
        // PlayerCharacter was given tag "Player" in editor
        if (col.CompareTag("Player")) 
        {
            // Activate follow mode if it sees the player, even if player leaves vision
            /*
            print("Player spotted!");
            */
            sawPlayer = true;
        }
    }


    // Private Functions
    private void Patrol() {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        RaycastHit2D groundInfo = Physics2D.Raycast(turnDetector.position, Vector2.down, 1);

        // If a pit is detected ahead, turn around
        /*
        print(groundInfo.collider);
        */
        if (groundInfo.collider == false) {
            if (direction == Direction.Left) {
                transform.eulerAngles = new Vector3(0, -180, 0);
                direction = Direction.Right;
            }
            else {
                transform.eulerAngles = new Vector3(0, 0, 0);
                direction = Direction.Left;
            }
        }

        // If a wall is detected ahead, turn around
        if (direction == Direction.Left) {
            RaycastHit2D wallInfo = Physics2D.Raycast(turnDetector.position, Vector2.left, 0.1f);
            /*
            print(wallInfo.collider);
            */
            if (wallInfo.collider == true) {
                transform.eulerAngles = new Vector3(0, -180, 0);
                direction = Direction.Right;
            }
        }
        else {
            RaycastHit2D wallInfo = Physics2D.Raycast(turnDetector.position, Vector2.right, 0.1f);
            /*
            print(wallInfo.collider);
            */
            if (wallInfo.collider == true) {
                transform.eulerAngles = new Vector3(0, 0, 0);
                direction = Direction.Left;
            }
        }
    }
}
