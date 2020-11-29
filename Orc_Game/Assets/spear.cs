using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spear : MonoBehaviour
{
    public CharacterMove character;

    public Rigidbody2D rb;

    public ParticleSystem impactSystem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && character.rangedState == CharacterMove.RangedState.Thrown)
        {
            character.rangedState = CharacterMove.RangedState.Holding;
            character.SwitchSpears();
        }

        if (other.CompareTag("Ground") && character.rangedState == CharacterMove.RangedState.Thrown)
        {
            rb.velocity = Vector2.zero;
            impactSystem.Play();

        }
        else if (other.CompareTag("Enemy"))
        {
            //TODO: enemies take damage
            rb.velocity = Vector2.zero;
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        // so it doesnt get stuck if thrown while inside a wall
        if (other.CompareTag("Ground") && character.rangedState == CharacterMove.RangedState.Thrown)
        {
            rb.velocity = Vector2.zero;

        }

    }

}
