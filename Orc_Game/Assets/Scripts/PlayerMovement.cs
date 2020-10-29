using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    public Tilemap ground;
    public float speed = 10;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 origin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Does the ray intersect any objects excluding the player layer
        /*
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up);
        */
        Vector3Int gridPos = ground.WorldToCell(origin);
        gridPos.z = 0;
        Debug.Log("tile at: " + gridPos + " Has Tile: " + ground.HasTile(gridPos));
        if (ground.HasTile(gridPos))
        {
            transform.position = origin;
        }

        /*if (true)
        {
            Debug.DrawRay(Camera.main.transform.position, Vector2.one, Color.yellow);
            Debug.Log("Did Hit: " + hit.collider.name);
            Debug.Log("point: " + hit.point);
            Vector3Int target = ground.WorldToCell(hit.point);
            Debug.Log("cell point: " + target);
            /*
            transform.position = ground.CellToWorld(target);
            #1#
            transform.position = target;

        }*/
        

        /*if (Input.GetKeyDown(KeyCode.A))
        {
            rb.MovePosition(Vector2.Lerp(rb.transform.localPosition, rb.transform.localPosition * Vector2.left, speed * Time.deltaTime));
            //rb.velocity = Vector2.left * speed * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            rb.velocity = Vector2.right * speed * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            rb.velocity = Vector2.up * speed * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            rb.velocity = Vector2.down * speed * Time.deltaTime;
        }*/
    }
}
