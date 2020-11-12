using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public List<Unit> units;
    public Vector3 target;
    public float speed;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = transform.position;
        foreach (Transform child in transform)
        {
            units.Add(child.gameObject.GetComponent<Unit>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(target, transform.position) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            target = other.transform.position;
        }
    }
    /*private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("trigger stay");

        if (other.CompareTag("Enemy"))
        {
            target = other.transform.position;
        }
    }*/
}
