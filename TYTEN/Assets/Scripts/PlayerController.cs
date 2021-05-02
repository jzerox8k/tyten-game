using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DirectionEnumerator;

namespace DirectionEnumerator
{
    public enum Direction
    {
        ANY = -1,
        NONE = 0,
        NORTH = 1,
        EAST = 2,
        SOUTH = 4,
        WEST = 8,
    };
}

public class PlayerController : MonoBehaviour
{
    public float run_speed = 1.0f;

    bool changeDirection = true;
    bool stalled;

    private Direction run_direction;
    private Direction valid_direction;

    private Vector2 run_vector;

    public Rigidbody2D rb;
    public new Collider2D collider;
    private Collider2D room_center;
    private Collider2D wall;

    // Start is called before the first frame update
    void Start()
    {
        run_speed = 1.0f;
        run_direction = Direction.NONE;
        changeDirection = true;
        stalled = true;
        collider = GetComponent<Collider2D>();
        tag = "Player";
    }

    // Update is called once per frame
    void Update()
    {
        // Every frame, check if we are currently in a location that allows us to change our direction
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

        // 
        if (stalled && (valid_direction != Direction.ANY))
        {
            if (run_direction != valid_direction)
            {
                run_direction = Direction.NONE;
            }
            else
            {
                valid_direction = Direction.ANY;
            }
        }
    }

    private void FixedUpdate()
    {
        // only change the rigidbody movement vector ~at the middle of a room, like snake
        if ((room_center.transform.position - collider.transform.position).magnitude < 0.01f)
        {
            // assume that we will no longer be stalled...
            stalled = false;
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
                    // ... unless the new run direction is NONE, in which case we stop moving...
                    run_vector = Vector2.zero;

                    // ... reallow for movement change ...
                    changeDirection = true;

                    // ... and stall
                    stalled = true;
                    break;
            }
        }

        // keep moving in the direction of the current movement vector
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
            // detect if we are at a wall exactly once.
            changeDirection = false;

            // we will only be able to rebound off of the wall in the same direction we came in
            // set the next valid direction to be the opposite of our current direction
            if (other.name == "N_Hotspot")
            {
                valid_direction = Direction.SOUTH;
            }
            else if (other.name == "S_Hotspot")
            {
                valid_direction = Direction.NORTH;
            }
            else if (other.name == "W_Hotspot")
            {
                valid_direction = Direction.EAST;
            }
            else if (other.name == "E_Hotspot")
            {
                valid_direction = Direction.WEST;
            }

            // the next running direction will be none, since we stop at the wall.
            run_direction = Direction.NONE;

            wall = other;

            Debug.Log("We hit a wall! Coords: " + other.transform.position.ToString());
        }
    }
}
