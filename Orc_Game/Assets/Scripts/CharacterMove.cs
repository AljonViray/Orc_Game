using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float MOVEMENT_SPEED = 1;
    public float JUMP_FORCE = 200;
    Rigidbody2D _rigidbody;
    public Transform attackPos;

    public float range = 1f;

    private LayerMask enemyMask;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        enemyMask = LayerMask.GetMask("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        // left right movement
        var movement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(movement, 0, 0) * Time.deltaTime * MOVEMENT_SPEED;
        if (movement > 0)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if(movement < 0)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        
        
        if(Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(_rigidbody.velocity.y) < 0.001f)
        {
            _rigidbody.AddForce(Vector2.up * JUMP_FORCE);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            AttackMelee();
        }

        if (_rigidbody.velocity.y < 1)
        {
            _rigidbody.gravityScale = 4;
        }
        else
        {
            _rigidbody.gravityScale = 2;
        }
    }

    void AttackMelee()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPos.position, range, enemyMask);
        foreach (var coll in enemiesHit)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, range, enemyMask);
            if (hit.collider != null)
            {
                Debug.Log(hit.normal);
                hit.rigidbody.AddForce((-hit.normal + Vector2.up) * 100f);                
            }

            
            // coll.gameObject.GetComponent<Rigidbody2D>();

        }
    }

    private void OnDrawGizmos()
    {
        
        // Gizmos.DrawSphere(attackPos.position, range);
    
    }
}
