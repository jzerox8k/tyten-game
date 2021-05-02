using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DirectionEnumerator;

public class TileManager : MonoBehaviour
{
    public bool North, South, West, East; // true = open, false = closed
    string NH = "N_Hotspot", SH = "S_Hotspot", WH = "W_Hotspot", EH = "E_Hotspot"; // Collider2D names in the FloorTile prefab
    string Wall = "Room Wall", Hallway = "Hallway"; // Collider2D tags

    int state = 0;
    public GameObject tile_empty;
    public Transform tile_sprite;
    public SpriteRenderer tile_spriteRenderer;
    List<EdgeCollider2D> edges;
    public Sprite[] sprites = new Sprite[] { };
    Dictionary<Direction, Transform> hotspots = new Dictionary<Direction, Transform>();

    private static int[] tile_map = new int[] { 0, 1, 1, 2,
                                                1, 3, 2, 4,
                                                1, 2, 3, 4,
                                                2, 4, 4, 5 };

    private static int[] tile_rot = new int[] { 0, 0, 1, 0,
                                                2, 0, 1, 0,
                                                3, 3, 1, 3,
                                                2, 2, 1, 0 };

    // Start is called before the first frame update
    void Start()
    {
        state = 0;

        tile_sprite = tile_empty.transform.Find("Tile_Sprite");
        tile_spriteRenderer = tile_sprite.GetComponent<SpriteRenderer>();

        hotspots.Add(Direction.NORTH, tile_empty.transform.Find(NH));
        hotspots.Add(Direction.SOUTH, tile_empty.transform.Find(SH));
        hotspots.Add(Direction.WEST, tile_empty.transform.Find(WH));
        hotspots.Add(Direction.EAST, tile_empty.transform.Find(EH));


        if (North) {
            state += (int)Direction.NORTH;
            hotspots[Direction.NORTH].tag = Hallway;
        }
        else
        {
            hotspots[Direction.NORTH].tag = Wall;
        }

        if (South)
        {
            state += (int)Direction.SOUTH;
            hotspots[Direction.SOUTH].tag = Hallway;
        }
        else
        {
            hotspots[Direction.SOUTH].tag = Wall;
        }

        if (West)
        {
            state += (int)Direction.WEST;
            hotspots[Direction.WEST].tag = Hallway;
        }
        else
        {
            hotspots[Direction.WEST].tag = Wall;
        }

        if (East)
        {
            state += (int)Direction.EAST;
            hotspots[Direction.EAST].tag = Hallway;
        }
        else
        {
            hotspots[Direction.EAST].tag = Wall;
        }


        tile_sprite.eulerAngles = Vector3.zero;
        tile_spriteRenderer.sprite = sprites[tile_map[state]];
        tile_sprite.Rotate(new Vector3(0, 0, -90 * tile_rot[state]));
    }

    private void OnValidate()
    {
        ValidateTile();
    }

    private void ValidateTile()
    {
        state = 0;

        if (North) { state += (int)Direction.NORTH; }
        if (South) { state += (int)Direction.SOUTH; }
        if (West) { state += (int)Direction.WEST; }
        if (East) { state += (int)Direction.EAST; }

        tile_sprite.eulerAngles = Vector3.zero;
        tile_spriteRenderer.sprite = sprites[tile_map[state]];
        tile_sprite.Rotate(new Vector3(0, 0, -90 * tile_rot[state]));
    }



    private (Vector2, Direction) CloseWall(Direction d)
    {
        Vector2 neighbor = new Vector2();
        Direction neighbor_wall = Direction.NONE;

        switch (d)
        {
            case (Direction.NORTH):
                North = !North;
                neighbor = Vector2.up;
                neighbor_wall = Direction.SOUTH;
                break;
            case (Direction.SOUTH):
                South = !South;
                neighbor = Vector2.down;
                neighbor_wall = Direction.NORTH;
                break;
            case (Direction.WEST):
                West = !West;
                neighbor = Vector2.left;
                neighbor_wall = Direction.EAST;
                break;
            case (Direction.EAST):
                East = !East;
                neighbor = Vector2.right;
                neighbor_wall = Direction.WEST;
                break;
        }

        hotspots[d].tag = (hotspots[d].tag == Wall) ? Hallway : Wall;

        ValidateTile();

        return (neighbor, neighbor_wall);
    }

    public void ToggleWall(Direction d)
    {
        (Vector2, Direction) neighbor_info = CloseWall(d);

        RaycastHit2D[] hits = Physics2D.RaycastAll(hotspots[d].position, neighbor_info.Item1);

        if (hits.Length >= 2)
        {
            Transform neighbor_tile = hits[1].transform.parent;
            Debug.Log("Neighbor tile : " + neighbor_tile.position.ToString());

            neighbor_tile.GetComponent<TileManager>().CloseWall(neighbor_info.Item2);
        }

    }

}
