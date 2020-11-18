using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{

    public enum AttackMode
    {
        Ranged,
        Melee
    };

    public AttackMode attackMode = AttackMode.Melee;
    
    public float MOVEMENT_SPEED = 1;
    public float JUMP_FORCE = 200;
    Rigidbody2D _rigidbody;
    public Transform attackPos;

    public float range = 1f;

    private LayerMask enemyMask;

    private Camera _camera;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        enemyMask = LayerMask.GetMask("Enemy");
        _camera = Camera.main;
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

        if (Input.GetKeyDown(KeyCode.K) && attackMode == AttackMode.Melee)
        {
            AttackMelee();
        }
        if (Input.GetMouseButtonDown(0) && attackMode == AttackMode.Ranged)
        {
            AttackRanged();
        }
        
        if (Input.GetMouseButton(1))
        {
            Time.timeScale = 0.5f;
        }
        else
        {
            Time.timeScale = 1f;
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
                hit.rigidbody.AddForce((-hit.normal + Vector2.up) * 100f);                
            }

        }
    }
    
    void AttackRanged()
    {
        Vector3 dir = _camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        Debug.DrawLine(transform.position, _camera.ScreenToWorldPoint(Input.mousePosition));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, Mathf.Infinity, enemyMask);
        _rigidbody.AddForce((-dir) * 100f);              

        Debug.DrawRay(transform.position, dir.normalized, Color.black, 1f);
        if (hit.collider != null)
        {
            hit.rigidbody.AddForce((-hit.normal + Vector2.up) * 100f);              
        }
        
    }


    private void OnDrawGizmos2D()
    {
        Gizmos.DrawSphere(attackPos.position, range);
    }
}
