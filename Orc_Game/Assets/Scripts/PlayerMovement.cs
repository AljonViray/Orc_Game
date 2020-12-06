﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public enum AttackMode
    {
        Ranged,
        Melee
    };
    public AttackMode attackMode = AttackMode.Melee;
    public float MOVEMENT_SPEED = 1;
    public float JUMP_FORCE = 200;
    public float recoil_strength = 100f;
    public float max_velocity;
    [HideInInspector] public Rigidbody2D _rigidbody;
    public Transform attackPos;
    public float range = 1f;
    private LayerMask enemyMask;
    private Camera _camera;
    private Animator animator;
    

    public Spear _spear;
    public GameObject unequiped_spear;
    [HideInInspector]public Rigidbody2D rb_spear;
    public float throw_force = 1000f;
    public float meleeDamage = 10f;

    public Transform groundCheck;
    public float groundCheckRadius;

    private PlayerHealth _playerHealth;
    

    public AudioClip jumpAudio;
    public AudioClip landAudio;
    public AudioClip throwAudio;
    private AudioSource _audioSource;
    /*private bool landed = true;
    private bool grounded;*/
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        enemyMask = LayerMask.GetMask("Enemy");
        _camera = Camera.main;
        animator = GetComponent<Animator>();
        rb_spear = _spear.rb;
        _spear.spearState = Spear.SpearState.Holding;
        _spear.box.enabled = false;
        // initialize which Spear is active initially
        if (attackMode == AttackMode.Melee)
        {
            unequiped_spear.gameObject.SetActive(true);
            _spear.gameObject.SetActive(false);
        }
        else
        {
            unequiped_spear.gameObject.SetActive(false);
            _spear.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -100f)
        {
            SceneManager.LoadScene("Platform2d");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Platform2d");
        }
        HandleInput();
        
        HandleAnimation();
        
        if (_rigidbody.velocity.y < 1)
        {
            _rigidbody.gravityScale = 4;
        }
        else
        {
            _rigidbody.gravityScale = 2;
        }
    }
    
    private void HandleInput()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            PlayAudio(jumpAudio);
            _rigidbody.AddForce(Vector2.up * JUMP_FORCE);
        }
        if (_spear.spearState == Spear.SpearState.Holding)
        {
            _spear.transform.position = transform.position;
        }
        if (attackMode == AttackMode.Ranged && _spear.spearState == Spear.SpearState.Holding)
        {
            HandleRanged();
        }
        
        // switch from melee to ranged
        if (Input.GetKeyDown(KeyCode.E) && _spear.spearState == Spear.SpearState.Holding)
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
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D temp = Physics2D.OverlapCircle(transform.position, 1f, LayerMask.GetMask("Spear"));
            if (temp && temp.gameObject.CompareTag("Spear"))
            {
                rb_spear = temp.attachedRigidbody;
                rb_spear.constraints = RigidbodyConstraints2D.None;
                _spear = temp.gameObject.GetComponent<Spear>();
                _spear.spearState = Spear.SpearState.Holding;
                _spear.box.enabled = false;
                SwitchSpears();
            }
        }
    }

    public void EquipSpear(Spear s)
    {
        rb_spear = s.rb;
        rb_spear.constraints = RigidbodyConstraints2D.None;
        _spear = s;
        _spear.spearState = Spear.SpearState.Holding;
        _spear.box.enabled = false;
        SwitchSpears();
    }
    private void HandleAnimation()
    {
        if (animator.GetBool("isAttacking"))
        {
            animator.SetBool("isAttacking", false);
        }
        
        if (Input.GetMouseButtonDown(0) && attackMode == AttackMode.Melee)
        {
            animator.SetBool("isAttacking", true);
            AttackMelee();
        }
        if (Input.GetMouseButtonDown(0) && attackMode == AttackMode.Ranged)
        {
            PlayAudio(throwAudio);
            AttackRanged();
        }
    }
    private void FixedUpdate()
    {
        // left right movement
        float movement = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(_rigidbody.velocity.x) < max_velocity)
        {
            _rigidbody.AddForce( new Vector2(movement * MOVEMENT_SPEED, 0f) * Time.deltaTime);
        }
        if (Mathf.Abs(_rigidbody.velocity.y) > max_velocity)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, max_velocity * _rigidbody.velocity.normalized.y);
        }
        if (attackMode == AttackMode.Melee)
        {
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
        }

    }

    bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, LayerMask.GetMask("Ground", "Enemy", "Spear"));
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
        
        _spear.transform.position = transform.position;
        Vector3 dir = _camera.ScreenToWorldPoint(Input.mousePosition);
        dir.z = 0;
        float AngleRad = Mathf.Atan2 (dir.y - _spear.transform.position.y, dir.x - _spear.transform.position.x);
        float angle = (180 / Mathf.PI) * AngleRad;
        if (Mathf.Abs(angle) < 90f)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if(Mathf.Abs(angle) > 90f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        _spear.transform.eulerAngles = new Vector3(0f, 0f, angle);
        
    }

    void AttackMelee()
    {
        Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPos.position, range, enemyMask);
        foreach (var coll in enemiesHit)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, range, enemyMask);
            if (hit.collider != null)
            {
                hit.rigidbody.AddForce((-hit.normal + Vector2.up) * recoil_strength);        
                EnemyHealth eh = hit.transform.GetComponent<EnemyHealth>();
                eh.TakeDamage(meleeDamage);
                eh.bloodSplatter.Play();
            }
            // coll.gameObject.GetComponent<Rigidbody2D>();
        }
    }
    
    void AttackRanged()
    {
        Vector3 dir = _camera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, dir.normalized, Mathf.Infinity, enemyMask);
        _rigidbody.AddForce((-dir) * recoil_strength);              
        
        rb_spear.velocity = Vector2.zero;
        rb_spear.AddForce(_spear.transform.right * throw_force);
        _spear.spearState = Spear.SpearState.Thrown;
        attackMode = AttackMode.Melee;
        Time.timeScale = 1f;

    }

    public void SwitchSpears()
    {
        _spear.gameObject.SetActive(!_spear.gameObject.activeSelf);
        unequiped_spear.gameObject.SetActive(!unequiped_spear.gameObject.activeSelf);
    }

    private void PlayAudio(AudioClip clip)
    {
        _audioSource.clip = clip;
        _audioSource.Play();
    }
    private void OnDrawGizmos()
    {
        /*
        Gizmos.DrawSphere(transform.position, range);
    */
    }
}
