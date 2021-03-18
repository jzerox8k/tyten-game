using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float run_speed = 1.0f;

    bool stall = false;
    bool changeDirection = true;

    enum Direction
    {
        NORTH,
        SOUTH,
        EAST,
        WEST,
        NONE
    };

    private Direction run_direction;

    private Vector2 run_vector;

    public Rigidbody2D rb;
    public Collider2D collider;
    public Collider2D room_center;
    public Collider2D wall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (changeDirection)
        {
            if (Input.GetKeyDown("w"))
            {
                run_direction = Direction.NORTH;
            }
            else if (Input.GetKeyDown("s"))
            {
                run_direction = Direction.SOUTH;
            }
            else if (Input.GetKeyDown("a"))
            {
                run_direction = Direction.WEST;
            }
            else if (Input.GetKeyDown("d"))
            {
                run_direction = Direction.EAST;
            }
        }
    }

    private void FixedUpdate()
    {
        if ((room_center.transform.position - collider.transform.position).magnitude < 0.01f)
        {
            switch (run_direction)
            {
                case (Direction.NORTH):
                    run_vector = Vector2.up;
                    break;

                case (Direction.SOUTH):
                    run_vector = Vector2.down;
                    break;

                case (Direction.WEST):
                    run_vector = Vector2.left;
                    break;

                case (Direction.EAST):
                    run_vector = Vector2.right;
                    break;

                case (Direction.NONE):
                    run_vector = Vector2.zero;
                    changeDirection = true;
                    break;
            }
        }

        rb.MovePosition(rb.position + run_vector * run_speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (changeDirection && (other.CompareTag("Room Wall") || other.CompareTag("Room Center")))
        {
            room_center = other;
        }
        
        if (other.CompareTag("Room Wall"))
        {
            // detect if we are at a wall
            changeDirection = false;
            run_direction = Direction.NONE;

            wall = other;

            Debug.Log("We hit a wall! Coords: " + other.transform.position.ToString());
        }
    }
}
