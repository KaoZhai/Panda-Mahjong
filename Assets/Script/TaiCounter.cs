using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaiCounter : MonoBehaviour
{
    private List<Tile> handTileList = null;
    private List<Tile> tileDeckList = null;
    private List<string> scoringList = null;
    private int[] tileCountArray = new int[50]; // 1~9：萬、11~19：筒、21~29：條、31~37：東南西北中發白、41~48：春夏秋冬梅蘭竹菊
    private int deckKongCnt = 0;
    private int deckPonCnt = 0;
    private int deckStraightCnt = 0;
    private bool isDealer = false;
    private bool isFirstTile = false;
    private bool isLastTile = false;
    private int tai = 0;

    public List<string> ScoringList
    {
        get { return scoringList; }
    }

    public int Tai
    {
        get { return tai; }
    }

    public void TaiCount(List<Tile> handTileList = null, List<Tile> tileDeckList = null, int deckKongCnt = 0, int deckPonCnt = 0, int deckStraightCnt = 0,
    bool isDealer = false, bool isFirstTile = false, bool isLastTile = false)
    {
        this.handTileList = handTileList;
        this.tileDeckList = tileDeckList;
        this.deckKongCnt = deckKongCnt;
        this.deckPonCnt = deckPonCnt;
        this.deckStraightCnt = deckStraightCnt;
        this.isDealer = isDealer;
        this.isFirstTile = isFirstTile;
        this.isLastTile = isLastTile;

        TransHandsToArray();

        //recursion
        FindHighestTai(handTileList);

        return;
    }

    private void TransHandsToArray()
    {
        foreach (var tile in handTileList)
        {
            if (tile.tile_type == TileType.Character)
            {
                tileCountArray[0 + tile.tile_number] += 1;
            }
            else if (tile.tile_type == TileType.Dot)
            {
                tileCountArray[10 + tile.tile_number] += 1;
            }
            else if (tile.tile_type == TileType.Bamboo)
            {
                tileCountArray[20 + tile.tile_number] += 1;
            }
            else if (tile.tile_type == TileType.Wind)
            {
                tileCountArray[30 + tile.tile_number] += 1;
            }
            else if (tile.tile_type == TileType.Dragon)
            {
                tileCountArray[34 + tile.tile_number] += 1;
            }
            else
            {
                Debug.Log("Error: " + tile.tile_type);
            }
        }
    }
    
    private int FindHighestTai(List<Tile> curTileList = null)
    {
        if(curTileList.Count == 0)
        {
            int tmpTai = CalculateTai();
            if (tmpTai > tai)
            {
                tai = tmpTai;
                scoringList = CalculateScoring();
                return tai;
            }
            else
            {
                return 0;
            }
        }

        foreach (var tile in curTileList)
        {
            // have pair
            if (pairCnt == 1)
            {

            }
            // no pair yet
            else
            {
                int tile_number = tile.tile_number;
                if (tile.tile_type == TileType.Character)
                {
                    if (characterArray[tile_number - 1] >= 2)
                    {
                        characterArray[tile_number - 1] -= 2;
                        for (int i = 0; i < 2; i++)
                        {
                            var tileToRemove = curTileList.SingleOrDefault(r => (r.tile_number == tile_number && r.tile_type == TileType.Character));
                            curTileList.Remove(tileToRemove);
                        }
                        pairCnt = 1;
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (tile.tile_type == TileType.Bamboo)
                {
                    if (bambooArray[tile_number - 1] >= 2)
                    {

                    }
                    else
                    {
                        continue;
                    }
                }
                else if (tile.tile_type == TileType.Dot)
                {
                    if (dotArray[tile_number - 1] >= 2)
                    {

                    }
                    else
                    {
                        continue;
                    }
                }
                else if (tile.tile_type == TileType.Dragon)
                {
                    if (dragonArray[tile_number - 1] >= 2)
                    {

                    }
                    else
                    {
                        continue;
                    }
                }
                else if (tile.tile_type == TileType.Wind)
                {
                    if (windArray[tile_number - 1] >= 2)
                    {

                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    Debug.Log("Error: " + tile.tile_type);
                    return 0;
                }
            }
        }
        return 0;
    }

    private int CalculateTai()
    {
        return 0;
    }

    private List<string> CalculateScoring()
    {
        return null;
    }

}
