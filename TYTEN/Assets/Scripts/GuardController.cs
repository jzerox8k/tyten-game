using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DirectionEnumerator;

public class GuardController : MonoBehaviour
{
    public float run_speed = 1.0f;

    public float stall_time = 3.0f;

    public float rotation_speed = 1.0f;
    public float scanner_distance = Mathf.Infinity;

    public bool changeDirection = true;
    public bool shut_allowed = true;
    bool stalled;

    Dictionary<string, Direction> direction_map = new Dictionary<string, Direction>()
    {
        {"N_Hotspot", Direction.NORTH},
        {"E_Hotspot", Direction.EAST},
        {"S_Hotspot", Direction.SOUTH},
        {"W_Hotspot", Direction.WEST}
    };

    private Direction run_direction;
    private List<Direction> valid_directions;

    private Vector2 run_vector;

    public Rigidbody2D rb;
    public new Collider2D collider;
    private Collider2D room_center;
    private Collider2D wall;
    public GameObject scanner;

    // Start is called before the first frame update
    void Start()
    {
        stall_time = 3.0f;
        run_direction = Direction.NONE;
        changeDirection = true;
        stalled = true;
        collider = GetComponent<Collider2D>();

        List<Collider2D> collider_init = new List<Collider2D>();
        Physics2D.OverlapCollider(collider, new ContactFilter2D(), collider_init);
        room_center = collider_init.Find(x => x.tag == "Room Center");

        StartCoroutine("DoCheck");

    }

    IEnumerator DoCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(stall_time);
            changeDirection = true; // allow for next move to occur. 
            shut_allowed = true;
            Debug.Log("Movement is allowed again.");
            Debug.Log("Door Locks are enabled again.");
        }
    }

    List<Direction> FindValidDirections()
    {
        List<Direction> vds = new List<Direction>();
        if (room_center != null)
        {
            //Debug.Log("Found Room! : " + Time.time);
            foreach (Transform irb in room_center.transform.parent)
            {
                Direction dir;
                Collider2D irb_cd2d;
                if (irb.TryGetComponent(out irb_cd2d))
                {
                    //Debug.Log("Trying Direction: " + irb_cd2d.name + "...");
                    if (direction_map.TryGetValue(irb_cd2d.name, out dir))
                    {
                        if (irb_cd2d.tag == "Hallway")
                        {
                            vds.Add(direction_map[irb_cd2d.name]);
                        }
                    }
                }
            }
        }
        //Debug.Log("Found " + vds.Count + " Valid Directions! : " + Time.time);
        return vds;
    }

    // Update is called once per frame
    void Update()
    {
        // Every frame, check if we are currently in a location that allows us to change our direction
        if (changeDirection)
        {
            valid_directions = FindValidDirections();
            run_direction = valid_directions[Random.Range(0, valid_directions.Count)];
            Debug.Log("Chosen Random direction is now " + run_direction.ToString());
            changeDirection = false; // cannot change direction again until next direction change event occurs at next room center
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

                    // ... and stall
                    stalled = true;

                    // look for the player
                    if (ScanForPlayer() && shut_allowed)
                    {
                        // close a random door
                        valid_directions = FindValidDirections();
                        Direction closing_door = valid_directions[Random.Range(0, valid_directions.Count)];
                        GameObject tile = room_center.transform.parent.gameObject;
                        Debug.Log("Parent Tile : " + tile.name);
                        tile.GetComponent<TileManager>().ToggleWall(closing_door);
                        shut_allowed = false; // wait for cooldown
                    }
                    break;
            }
        }

        // keep moving in the direction of the current movement vector
        rb.MovePosition(rb.position + run_vector * run_speed * Time.fixedDeltaTime);
    }

    private bool ScanForPlayer()
    {
        // toggle if found player
        bool found = false;

        // rotate the scanner a little
        scanner.transform.Rotate(new Vector3(0, 0, rotation_speed * Time.fixedDeltaTime));

        Vector2 rc_dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * scanner.transform.eulerAngles.z),
                                     Mathf.Sin(Mathf.Deg2Rad * scanner.transform.eulerAngles.z));

        //Debug.Log("Casting Ray from " + rb.transform.position.ToString() + " along direction " + rc_dir + "...");

        // raycast along scanner
        RaycastHit2D[] hits = Physics2D.RaycastAll(rb.transform.position, rc_dir, scanner_distance);

        Transform reticle = scanner.transform.Find("TargetReticle");
        Transform beam = scanner.transform.Find("TargetBeam").Find("BeamSprite");

        float wall_distance = scanner_distance;
        Vector3 wall_position = rb.position + rc_dir.normalized * scanner_distance;

        // look for the player
        //Debug.Log("Found " + hits.Length + " Colliders.");
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.transform.tag == "Player")
            {
                Debug.Log("FOUND PLAYER! WEE WOO WEE WOO!");
                found = true;
                break;
            }
            else if (hit.transform.tag == "Room Wall")
            {
                wall_distance = hit.distance;
                wall_position = hit.point;
                //Debug.Log("Just a wall...");
                break;
            }
        }

        // update the beam stretch and reticle sprite position
        reticle.position = wall_position;
        beam.localScale = new Vector3(wall_distance, beam.localScale.y);

        return found;
    }

    private void CloseDoor()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Room Center"))
        {
            room_center = other;

            Debug.Log("Enemy is at room center! Coords: " + other.transform.position.ToString());

            run_direction = Direction.NONE; // stop moving!

            // ENEMIES can only move from center to center 
            // Valid directions depend on the state of the parent tile of the collider we run 

            // the next running direction will be none, since we stop at the wall.
        }
    }
}
