using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaiCounter : MonoBehaviour
{
    private List<Tile> handTileList = new List<Tile>();
    private List<Tile> tileDeckList = new List<Tile>();
    private List<string> scoringList = new List<string>();
    private int deckGangCnt = 0;
    private int deckPonCnt = 0;
    private int deckStraightCnt = 0;
    private int faceWind = 0; //門風 1東2南3西4北
    private int courtWind = 0; //場風 1東2南3西4北
    private int hideGangCnt = 0;
    private int dealerWinStreak = 0;
    private bool isDealer = false;
    private bool isFirstTile = false;
    private bool isLastTile = false;
    private bool isSelfDraw = false;
    private bool isAfterGang = false;
    private bool isOnly = false;
    private Tile winningTile = null;
    private int tai = 0;

    public void Start()
    {
    }

    public List<string> ScoringList
    {
        get { return scoringList; }
    }

    public int Tai
    {
        get { return tai; }
    }

    public void TaiCount(List<Tile> handTileList, List<Tile> tileDeckList, int deckGangCnt = 0, int deckPonCnt = 0, int deckStraightCnt = 0,
    int faceWind = 0, int courtWind = 0, int hideGangCnt = 0, int dealerWinStreak = 0, bool isDealer = false, bool isFirstTile = false,
    bool isLastTile = false, bool isSelfDraw = false, bool isAfterGang = false, bool isOnly = false, Tile winningTile = null)
    {
        int[] tileCountArray = new int[50]; // 1~9：萬、11~19：筒、21~29：條、31~37：東南西北中發白、41~48：春夏秋冬梅蘭竹菊

        this.handTileList = handTileList;
        this.tileDeckList = tileDeckList;
        this.deckGangCnt = deckGangCnt;
        this.deckPonCnt = deckPonCnt;
        this.deckStraightCnt = deckStraightCnt;
        this.faceWind = faceWind;
        this.courtWind = courtWind;
        this.hideGangCnt = hideGangCnt;
        this.dealerWinStreak = dealerWinStreak;
        this.isDealer = isDealer;
        this.isFirstTile = isFirstTile;
        this.isLastTile = isLastTile;
        this.isSelfDraw = isSelfDraw;
        this.isAfterGang = isAfterGang;
        this.isOnly = isOnly;
        this.winningTile = winningTile;
        
        TransToArray(tileCountArray, handTileList);

        //recursion
        FindHighestTai((int[])tileCountArray.Clone(), false, 0, 0);

        return;
    }

    private void TransToArray(int[] tileCountArray, List<Tile> tileList)
    {
        foreach (var tile in tileList)
        {
            if (tile.tile_type == TileType.Character)
            {
                tileCountArray[0 + tile.tile_number] += 1; // 1~9 萬
            }
            else if (tile.tile_type == TileType.Dot)
            {
                tileCountArray[10 + tile.tile_number] += 1; // 11~19 筒
            }
            else if (tile.tile_type == TileType.Bamboo)
            {
                tileCountArray[20 + tile.tile_number] += 1; // 21~29 條
            }
            else if (tile.tile_type == TileType.Wind)
            {
                tileCountArray[30 + tile.tile_number] += 1; // 31~34 東南西北
            }
            else if (tile.tile_type == TileType.Dragon)
            {
                tileCountArray[34 + tile.tile_number] += 1; // 35~37 中發白
            }
            else if (tile.tile_type == TileType.Season)
            {
                tileCountArray[40 + tile.tile_number] += 1; // 41~44 春夏秋冬
            }
            else if (tile.tile_type == TileType.Flower)
            {
                tileCountArray[44 + tile.tile_number] += 1; // 45~48 梅蘭竹菊
            }
            else
            {
                Debug.Log("Error tile type: " + tile.tile_type);
            }
        }
    }

    private void FindHighestTai(int[] nowTileArray, bool havePair = false, int ponCnt = 0, int straightCnt = 0)
    {
        if(nowTileArray.Sum() == 0)
        {
            int tmpTai = CalculateTai(ponCnt, straightCnt);
            if (tmpTai > this.tai)
            {
                this.tai = tmpTai;
                this.scoringList = CalculateScoring(ponCnt, straightCnt);
            }
            return;
        }

        for(int i = 1; i <= 37; i++)
        {
            if(nowTileArray[i] == 0)
                continue;
            
            // have pair
            if (havePair)
            {
                // check straight
                if(nowTileArray[i] > 0 && nowTileArray[i - 1] > 0 && nowTileArray[i + 1] > 0)
                {
                    nowTileArray[i - 1] -= 1;
                    nowTileArray[i] -= 1;
                    nowTileArray[i + 1] -= 1;
                    FindHighestTai((int[])nowTileArray.Clone(), true, ponCnt, straightCnt + 1);
                    nowTileArray[i - 1] += 1;
                    nowTileArray[i] += 1;
                    nowTileArray[i + 1] += 1;
                }
                // check pon
                if(nowTileArray[i] >= 3)
                {
                    nowTileArray[i] -= 3;
                    FindHighestTai((int[])nowTileArray.Clone(), true, ponCnt + 1, straightCnt);
                    nowTileArray[i] += 3;
                }
            }
            // no pair yet
            else
            {
                if(nowTileArray[i] >= 2)
                {
                    nowTileArray[i] -= 2;
                    FindHighestTai((int[])nowTileArray.Clone(), true, ponCnt, straightCnt);
                    nowTileArray[i] += 2;
                }
            }
        }
        return;
    }

    private int CalculateTai(int handPonCnt = 0, int handStraightCnt = 0)
    {
        int calTai = 0;
        return calTai;
    }

    private List<string> CalculateScoring(int handPonCnt = 0, int handStraightCnt = 0)
    {
        List<string> calScoring = new List<string>();
        return calScoring;
    }

    private bool Dealer() //莊家
    {
        return isDealer;
    }

    private bool SelfDraw() //自摸
    {
        return isSelfDraw;
    }

    private bool MenQing() //門清
    {
        if(deckGangCnt + deckPonCnt + deckStraightCnt == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool Zhong(int[] tileCountArray) //中
    {
        if (tileCountArray[45] >= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool Fa(int[] tileCountArray) //發
    {
        if (tileCountArray[46] >= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool Bai(int[] tileCountArray) //白
    {
        if (tileCountArray[47] >= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool Dong(int[] tileCountArray) //東
    {
        if (tileCountArray[31] >= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool Nan(int[] tileCountArray)//南
    {
        if (tileCountArray[32] >= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool Xi(int[] tileCountArray)//西
    {
        if (tileCountArray[33] >= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool Bei(int[] tileCountArray)//北
    {
        if (tileCountArray[34] >= 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool HuaGang(int[] tileCountArray, bool isSeason)//花槓
    {
        if (isSeason && tileCountArray[41] + tileCountArray[42] + tileCountArray[43] + tileCountArray[44] == 4)
        {
            return true;
        }
        else if ((!isSeason) && tileCountArray[45] + tileCountArray[46] + tileCountArray[47] + tileCountArray[48] == 4)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool HuaTai(int[] tileCountArray, int pos)//花台
    {
        if (tileCountArray[pos] == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool DuTing()//獨聽
    {
        return isOnly;
    }

    private bool GangShangKaiHua()//槓上開花
    {
        return isAfterGang;
    }

    private bool HaiDiLaoYue()//海底撈月
    {
        return isLastTile && isSelfDraw;
    }

    private bool HeDiLaoYu()//河底撈魚
    {
        return isLastTile && (!isSelfDraw);
    }

    private bool QuanQiuRen()//全求人
    {
        if(deckGangCnt + deckPonCnt + deckStraightCnt == 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool PingHu(int[] tileCountArray, int handPonCnt)//平胡
    {
        //檢查無字無花
        int check = 0;
        for (int i = 31; i <= 48; i++)
        {
            check += tileCountArray[i];
        }

        if (deckGangCnt + deckPonCnt + hideGangCnt + handPonCnt + check == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool SanAnKe(int handPonCnt)//三暗刻
    {
        if (handPonCnt + hideGangCnt == 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool PengPengHu(int handPonCnt)//碰碰胡
    {
        if(handPonCnt + hideGangCnt + deckPonCnt + deckGangCnt == 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //混一色

    //小三元

    private bool SiAnKe(int handPonCnt)//四暗刻
    {
        if (handPonCnt + hideGangCnt == 4)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool WuAnKe(int handPonCnt)//五暗刻
    {
        if (handPonCnt + hideGangCnt == 5)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //清一色

    //小四喜

    //大三元

    private bool QiQiangYi()//七搶一
    {
        if ((!isSelfDraw) && (winningTile.tile_type == TileType.Season || winningTile.tile_type == TileType.Flower))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool BaXianGuoHai(int[] tileCountArray)//八仙過海
    {
        int cnt = 0;
        for(int i = 41; i <= 48; i++)
        {
            cnt += tileCountArray[i];
        }

        if(cnt == 8)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //字一色

    //大四喜

    //地胡

    //天胡
}
