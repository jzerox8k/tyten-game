using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float run_speed = 0.5f;

    bool stall = false;

    enum Direction
    {
        NORTH,
        SOUTH,
        EAST,
        WEST
    };

    private Direction run_direction;

    private Vector2 run_vector;

    public Rigidbody2D rb;
    public Collider2D collider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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

    private void FixedUpdate()
    {
        List<Collider2D> cldr_list = new List<Collider2D>();
        ContactFilter2D cntct_fltr = new ContactFilter2D();
        cntct_fltr.NoFilter();
        collider.OverlapCollider(cntct_fltr, cldr_list);

        foreach (Collider2D cldr in cldr_list)
        {
            Debug.Log(cldr);
        }

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
        }

        rb.MovePosition(rb.position + run_vector * run_speed * Time.fixedDeltaTime);
    }
}
