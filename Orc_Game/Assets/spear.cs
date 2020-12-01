using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class spear : MonoBehaviour
{
    public CharacterMove character;
    public float spearDamage;
    public Rigidbody2D rb;
    public BoxCollider2D box;
    public ParticleSystem impactSystem;
    public FixedJoint2D joint2D;
    
    private void Start()
    {
        box.enabled = false;
    }

    private void Update()
    {
        if (character.rangedState == CharacterMove.RangedState.Landed && Input.GetKeyDown(KeyCode.E))
        {
            if (Physics2D.OverlapCircle(transform.position, 1f, LayerMask.GetMask("Player")))
            {
                rb.constraints = RigidbodyConstraints2D.None;
                character.rangedState = CharacterMove.RangedState.Holding;
                box.enabled = false;
                character.SwitchSpears();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground") && character.rangedState == CharacterMove.RangedState.Thrown)
        {
            rb.velocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            character.rangedState = CharacterMove.RangedState.Landed;
            box.enabled = true;
            impactSystem.Play();

        }
        else if (other.CompareTag("Enemy") && character.rangedState == CharacterMove.RangedState.Thrown)
        {
            //TODO: enemies take damage
            other.GetComponent<EnemyHealth>().TakeDamage(spearDamage);
            other.attachedRigidbody.AddForce(rb.velocity * 10f);

        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        // so it doesnt get stuck if thrown while inside a wall
        if (other.CompareTag("Ground") && character.rangedState == CharacterMove.RangedState.Thrown)
        {
            character.rangedState = CharacterMove.RangedState.Landed;
            rb.velocity = Vector2.zero;
        }
        else if (other.CompareTag("Enemy") && character.rangedState == CharacterMove.RangedState.Thrown)
        {
            //TODO: enemies take damage
            other.GetComponent<EnemyHealth>().TakeDamage(spearDamage);
            other.attachedRigidbody.AddForce(rb.velocity * 10f);

        }

    }

}
