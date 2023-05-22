using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaiCounter : MonoBehaviour
{
    private List<Tile> handTile = null;
    private List<Tile> tileDeck = null;
    private List<string> scoring = null;
    private int[] characterArray = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0};
    private int[] dotArray = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0};
    private int[] bambooArray = new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0};
    private int[] dragonArray = new int[] {0, 0, 0}; //中 發 白
    private int[] windArray = new int[] {0, 0, 0, 0}; //東 南 西 北
    private int[] seasonArray = new int[] {0, 0, 0, 0}; //春 夏 秋 冬
    private int[] flowerArray = new int[] {0, 0, 0, 0}; //梅 蘭 竹 菊
    private int deckKongCnt = 0;
    private int deckPonCnt = 0;
    private int deckStraightCnt = 0;
    private int tai = 0;
    private bool isDealer = false;
    private bool isFirstTile = false;
    private bool isLastTile = false;

    public List<string> Scoring
    {
        get { return scoring; }
    }

    public int Tai
    {
        get { return tai; }
    }

    public void TaiCount(List<Tile> handTile = null, List<Tile> tileDeck = null, int deckKongCnt = 0, int deckPonCnt = 0, int deckStraightCnt = 0,
    bool isDealer = false, bool isFirstTile = false, bool isLastTile = false)
    {
        this.handTile = handTile;
        this.tileDeck = tileDeck;
        this.deckKongCnt = deckKongCnt;
        this.deckPonCnt = deckPonCnt;
        this.deckStraightCnt = deckStraightCnt;
        this.isDealer = isDealer;
        this.isFirstTile = isFirstTile;
        this.isLastTile = isLastTile;

        TransHandsToArray();

        return;
    }

    private void TransHandsToArray()
    {
        foreach (var tile in handTile)
        {
            if (tile.tile_type == TileType.Character)
            {
                characterArray[tile.tile_number - 1] += 1;
            }
            else if (tile.tile_type == TileType.Bamboo)
            {
                bambooArray[tile.tile_number - 1] += 1;
            }
            else if (tile.tile_type == TileType.Dot)
            {
                dotArray[tile.tile_number - 1] += 1;
            }
            else if (tile.tile_type == TileType.Dragon)
            {
                dragonArray[tile.tile_number - 1] += 1;
            }
            else if (tile.tile_type == TileType.Wind)
            {
                windArray[tile.tile_number - 1] += 1;
            }
            else
            {
                Debug.Log("Error: " + tile.tile_type);
            }
        }
    }

}
