using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    public enum SpearState
    {
        Holding,
        Thrown,
        Landed
    };

    public SpearState spearState = SpearState.Landed;
    public PlayerMovement character;
    public float spearDamage;
    public Rigidbody2D rb;
    public BoxCollider2D box;
    public ParticleSystem impactSystem;
    
    private void Start()
    {
        if (spearState == SpearState.Landed)
        {
            box.enabled = true;
        }
        else
        {
            box.enabled = false;
        }
    }

    private void LateUpdate()
    {
        /*if (spearState == SpearState.Thrown)
        {
            float angle = Mathf.Atan2(rb.velocity.y - transform.position.y, rb.velocity.x - transform.position.x);
            angle = (180 / Mathf.PI) * angle;
            transform.eulerAngles = new Vector3(0f, 0f, angle);
            print(angle);
        }*/
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") && spearState == SpearState.Thrown)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.velocity = Vector2.zero;
            spearState = SpearState.Landed;
            box.enabled = true;
            impactSystem.Play();

        }
        else if (other.CompareTag("Enemy") && spearState == SpearState.Thrown)
        {
            //TODO: enemies take damage
            other.GetComponent<EnemyHealth>().TakeDamage(spearDamage);
            other.attachedRigidbody.AddForce(rb.velocity * 10f);

        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        // so it doesnt get stuck if thrown while inside a wall
        if (other.CompareTag("Ground") && spearState == SpearState.Thrown)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.velocity = Vector2.zero;
            spearState = SpearState.Landed;
            box.enabled = true;
            impactSystem.Play();
        }
        else if (other.CompareTag("Enemy") && spearState == SpearState.Thrown)
        {
            //TODO: enemies take damage
            other.GetComponent<EnemyHealth>().TakeDamage(spearDamage);
            other.attachedRigidbody.AddForce(rb.velocity * 10f);

        }

    }

}
