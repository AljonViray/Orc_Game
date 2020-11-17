using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float MOVEMENT_SPEED = 1;
    public float JUMP_FORCE = 200;
    Rigidbody2D _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // left right movement
        var movement = Input.GetAxis("Horizontal");
        transform.position += new Vector3(movement, 0, 0) * Time.deltaTime * MOVEMENT_SPEED;

        if(Input.GetKeyDown(KeyCode.UpArrow) && Mathf.Abs(_rigidbody.velocity.y) < 0.001f)
        {
            //_rigidbody.AddForce(new Vector2(0, JUMP_FORCE), ForceMode2D.Impulse);
            GetComponent<Rigidbody2D>().AddForce(Vector2.up * JUMP_FORCE);
        }
    }
}
