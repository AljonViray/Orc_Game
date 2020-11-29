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
    public enum RangedState
    {
        Holding,
        Thrown
    };

    [HideInInspector] public RangedState rangedState = RangedState.Holding;
    
    
    public AttackMode attackMode = AttackMode.Melee;
    public float MOVEMENT_SPEED = 1;
    public float JUMP_FORCE = 200;
    public float recoil_strength = 100f;
    public float max_velocity;
    Rigidbody2D _rigidbody;
    public Transform attackPos;
    public float range = 1f;
    private LayerMask enemyMask;
    private Camera _camera;
    private Animator animator;
    

    public GameObject spear;
    public GameObject unequiped_spear;
    private Rigidbody2D rb_spear;
    public float throw_force = 1000f;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        enemyMask = LayerMask.GetMask("Enemy");
        _camera = Camera.main;
        animator = GetComponent<Animator>();
        rb_spear = spear.GetComponent<Rigidbody2D>();
        
        // initialize which spear is active initially
        if (attackMode == AttackMode.Melee)
        {
            unequiped_spear.gameObject.SetActive(true);
            spear.gameObject.SetActive(false);
        }
        else
        {
            unequiped_spear.gameObject.SetActive(false);
            spear.gameObject.SetActive(true);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (rangedState == RangedState.Holding)
        {
            spear.transform.position = transform.position;
        }
        if (attackMode == AttackMode.Ranged && rangedState == RangedState.Holding)
        {
            HandleRanged();
        }
        
        // switch from melee to ranged
        if (Input.GetKeyDown(KeyCode.E) && rangedState == RangedState.Holding)
        {
            SwitchSpears();
            if (attackMode == AttackMode.Ranged)
            {
                attackMode = AttackMode.Melee;
            }
            else
            {
                attackMode = AttackMode.Ranged;
            }
        }
        if (animator.GetBool("isAttacking"))
        {
            animator.SetBool("isAttacking", false);
        }
        // left right movement
        var movement = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(_rigidbody.velocity.x) < max_velocity)
        {
            _rigidbody.AddForce( new Vector2(movement * MOVEMENT_SPEED, 0f) * Time.deltaTime);
        }

        if (movement > 0)
        {
            animator.SetBool("isWalking", true);
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if(movement < 0)
        {
            animator.SetBool("isWalking", true);
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        else 
        {
            animator.SetBool("isWalking", false);
        }
        
        
        if(Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(_rigidbody.velocity.y) < 0.001f)
        {
            _rigidbody.AddForce(Vector2.up * JUMP_FORCE);
        }

        if (Input.GetMouseButtonDown(0) && attackMode == AttackMode.Melee)
        {
            animator.SetBool("isAttacking", true);
            AttackMelee();
        }
        if (Input.GetMouseButtonDown(0) && attackMode == AttackMode.Ranged)
        {
            AttackRanged();
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

    void HandleRanged()
    {
        if (Input.GetMouseButton(1))
        {
            attackMode = AttackMode.Ranged;
            Time.timeScale = 0.5f;
        }
        else
        {
            Time.timeScale = 1f;
        }
        
        spear.transform.position = transform.position;
        Vector3 dir = _camera.ScreenToWorldPoint(Input.mousePosition);
        dir.z = 0;
        float AngleRad = Mathf.Atan2 (dir.y - spear.transform.position.y, dir.x - spear.transform.position.x);
        float angle = (180 / Mathf.PI) * AngleRad;
        spear.transform.eulerAngles = new Vector3(0f, 0f, angle);
        
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
            // coll.gameObject.GetComponent<Rigidbody2D>();
        }
    }
    
    void AttackRanged()
    {
        Vector3 dir = _camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, Mathf.Infinity, enemyMask);
        _rigidbody.AddForce((-dir) * recoil_strength);              

        /*
        if (hit.collider != null)
        {
            hit.rigidbody.AddForce((-hit.normal + Vector2.up) * 100f);              
        }*/
        
        rb_spear.AddForce(spear.transform.right * throw_force);
        rangedState = RangedState.Thrown;
        attackMode = AttackMode.Melee;
        Time.timeScale = 1f;

    }

    public void SwitchSpears()
    {
        spear.gameObject.SetActive(!spear.gameObject.activeSelf);
        unequiped_spear.gameObject.SetActive(!unequiped_spear.gameObject.activeSelf);
    }
    
}
