using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum Team
    {
        Ally,
        Enemy
    }

    public Team team;

    public int health = 10;
    public int speed = 10;
    private Rigidbody2D rb;
    private Squad squad;
    private void Start()
    {
        squad = GetComponentInParent<Squad>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Enemy"))
        {
            Debug.Log("Collided");
        }
    }
    
    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            squad.target = other.transform.position;
        }
    }*/
}
