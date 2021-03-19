using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public bool North, South, West, East; // true = open, false = closed
    string NH = "N_Hotspot", SH = "S_Hotspot", WH = "W_Hotspot", EH = "E_Hotspot"; // Collider2D names in the FloorTile prefab
    string Wall = "Room Wall", Untagged = "Untagged"; // Collider2D tags

    int state = 0;
    public GameObject tile_empty;
    public Transform tile_sprite;
    public SpriteRenderer tile_spriteRenderer;
    List<EdgeCollider2D> edges;
    public Sprite[] sprites;

    private static int[] tile_map = new int[] { 0, 1, 1, 2,
                                                1, 3, 2, 4,
                                                1, 2, 3, 4,
                                                2, 4, 4, 5 };

    private static int[] tile_rot = new int[] { 0, 0, 1, 0,
                                                2, 0, 1, 0,
                                                3, 3, 1, 3,
                                                2, 2, 1, 0 };

    enum Direction
    {
        NORTH = 1,
        EAST = 2,
        SOUTH = 4,
        WEST = 8,        
    };

    // Start is called before the first frame update
    void Start()
    {
        state = 0;

        tile_sprite = tile_empty.transform.Find("Tile_Sprite");
        tile_spriteRenderer = tile_sprite.GetComponent<SpriteRenderer>();

        if (North) {
            state += (int)Direction.NORTH;
            tile_empty.transform.Find(NH).tag = Untagged;
        }
        else {
            tile_empty.transform.Find(NH).tag = Wall;
        }

        if (South)
        {
            state += (int) Direction.SOUTH;
            tile_empty.transform.Find(SH).tag = Untagged;
        }
        else
        {
            tile_empty.transform.Find(SH).tag = Wall;
        }

        if (West)
        {
            state += (int)Direction.WEST;
            tile_empty.transform.Find(WH).tag = Untagged;
        }
        else
        {
            tile_empty.transform.Find(WH).tag = Wall;
        }

        if (East)
        {
            state += (int)Direction.EAST;
            tile_empty.transform.Find(EH).tag = Untagged;
        }
        else
        {
            tile_empty.transform.Find(EH).tag = Wall;
        }


        tile_sprite.eulerAngles = Vector3.zero;
        tile_spriteRenderer.sprite = sprites[tile_map[state]];
        tile_sprite.Rotate(new Vector3(0, 0, -90 * tile_rot[state]));
    }

    private void OnValidate()
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

}
