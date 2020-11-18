using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwordman : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    public Transform attackPos;
    public float range = 3f;
    public float power = 300f;
    public float JUMP_FORCE = 300;

    private LayerMask playerMask;
    private int timer;
    private int movement;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = 2;
        playerMask = LayerMask.GetMask("Player");
        timer = 0;
        movement = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] playerHit = Physics2D.OverlapCircleAll(attackPos.position, range, playerMask);  
        if (playerHit.Length > 0 && timer <= 0) {
            Attack();
            timer = 600;
            Debug.Log("Enemy attack");
        }

        if (Random.value > .7) {
            timer -= 3;
        }
        else {
            timer -= 1;
        }

        if (Mathf.Abs(_rigidbody.velocity.y) < 0.001f && Random.value < .1) {
            _rigidbody.AddForce(Vector2.up * JUMP_FORCE);
        }

        if (Mathf.Abs(_rigidbody.velocity.y) < 0.001f && Random.value < .01) {
            float rand = Random.value;
            if (rand < .33) {
                movement = -1;
            }
            else if (rand > .66) {
                movement = 1;
            }
            else {
                movement = 0;
            }
        }

        transform.position += new Vector3(movement, 0, 0) * Time.deltaTime;
    }

    void Attack() {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.right, range * 2, playerMask);
        if (hit.collider != null)
        {
            hit.rigidbody.AddForce((-hit.normal + Vector2.up) * power);               
        }
    }

    private void OnDrawGizmos()
    {
        
        // Gizmos.DrawSphere(attackPos.position, range);
    
    }
}
