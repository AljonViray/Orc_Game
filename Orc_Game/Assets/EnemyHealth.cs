using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public ParticleSystem bloodSplatter;
    public ParticleSystem deathGush;
    
    private float death_time = 5f;
    private bool dead = false;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    private EnemyAI enemyScript;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        enemyScript = GetComponent<EnemyAI>();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (dead)
        {
            if (death_time <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                death_time -= Time.deltaTime;
            }
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        
        if (currentHealth <= 0)
        {
            dead = true;
            enemyScript.enabled = false;
            Die();
        }
    }

    private void Die()
    {
        deathGush.Play();
        boxCollider.enabled = false;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.AddForce(Vector2.up * 500f);
        rb.AddTorque(180f);
    }

}
