using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    Rigidbody2D rb;
    BoxCollider2D hitbox;
    BoxCollider2D visionArea;
    CircleCollider2D attackArea;
    public Transform player;
    public float moveSpeed = 100;
    public float attackPower = 300;
    public float jumpForce = 300;


    // Unity Functions
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Enemies have the following hitboxes:
        // Normal hitbox attached to Enemy itself
        hitbox = GetComponent<BoxCollider2D>();
        // First child object must be VisionArea
        visionArea = transform.GetChild(0).GetComponent<BoxCollider2D>();
        // Second child object must be AttackArea
        attackArea = transform.GetChild(1).GetComponent<CircleCollider2D>();

        player = GameObject.Find("PlayerCharacter").transform;
    }

    // Detect player in enemy's vision area
    void OnTriggerEnter2D (Collider2D col) 
    {
        // PlayerCharacter was given tag "Player" in editor
        if (col.CompareTag("Player")) 
        {
            // Activate follow mode if it sees the player, even if player leaves vision
            print("Player spotted!");

        }
    }


    // Private Functions
    private void FollowPlayer() {
        while (true) {
            print(player.position);
            Vector3 forwardAxis = new Vector3 (0, 0, -1);
            transform.LookAt (player.position, forwardAxis);
            Debug.DrawLine (transform.position, player.position);
            transform.eulerAngles = new Vector3 (0, 0, -transform.eulerAngles.z);
            transform.position -= transform.TransformDirection(Vector2.up) * moveSpeed;        
        }
    }
}
