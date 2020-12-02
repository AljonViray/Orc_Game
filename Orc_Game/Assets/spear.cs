using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class spear : MonoBehaviour
{
    public enum SpearState
    {
        Holding,
        Thrown,
        Landed
    };

    public SpearState spearState = SpearState.Landed;
    public CharacterMove character;
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

    private void Update()
    {
        /*if (spearState == SpearState.Landed && Input.GetKeyDown(KeyCode.E))
        {
            if (Physics2D.OverlapCircle(transform.position, 1f, LayerMask.GetMask("Player")))
            {
                rb.constraints = RigidbodyConstraints2D.None;
                character._spear = this;
                character.rb_spear = rb;
                spearState = SpearState.Holding;
                box.enabled = false;
                character.SwitchSpears();
            }
        }*/
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") && spearState == SpearState.Thrown)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            spearState = SpearState.Thrown;
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
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
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
