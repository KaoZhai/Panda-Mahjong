using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Flower, 
    Season,
    Wind,
    Dragon, 
    Character,
    Bamboo,
    Dot
}


public class Tile : MonoBehaviour
{
    // {tile_type}_{tile_number}_{1..4}
    public string id;
    public TileType tile_type;
    public int tile_number;
    public int cardFace_index;


}
