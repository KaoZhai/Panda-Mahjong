using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Play
{
    public class TaiCalculator : MonoBehaviour
    {
        private List<GameObject> handTilesList = new List<GameObject>();
        private List<GameObject> showTilesList = new List<GameObject>();
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
        private GameObject winningTile = null;
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

        public TaiCalculator(
            List<GameObject> handTilesList,
            List<GameObject> showTilesList,
            int deckGangCnt = 0,
            int deckPonCnt = 0,
            int deckStraightCnt = 0,
            int faceWind = 0,
            int courtWind = 0,
            int hideGangCnt = 0,
            int dealerWinStreak = 0,
            bool isDealer = false,
            bool isFirstTile = false,
            bool isLastTile = false,
            bool isSelfDraw = false,
            bool isAfterGang = false,
            bool isOnly = false,
            GameObject winningTile = null)
        {
            int[] tileCountArray = new int[50]; // 1~9：萬、11~19：筒、21~29：條、31~37：東南西北中發白、41~48：春夏秋冬梅蘭竹菊

            this.handTilesList = handTilesList;
            this.showTilesList = showTilesList;
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

            if (QiQiangYi() || BaXianGuoHai())
            {
                this.tai = CalculateTai(0, 0);
                this.scoringList = CalculateScoring(0, 0);
            }
            else
            {
                TransToArray(tileCountArray, handTilesList);
                //recursion
                FindHighestTai((int[])tileCountArray.Clone(), false, 0, 0);
            }
        }

        private void TransToArray(int[] tileCountArray, List<GameObject> tileList)
        {
            foreach (var tile in tileList)
            {
                TileType tileType = tile.GetComponent<Tile>().TileType;

                switch (tileType)
                {
                    case TileType.Character:
                        tileCountArray[0 + tile.GetComponent<Tile>().TileNumber] += 1; // 1~9 萬
                        break;
                    case TileType.Dot:
                        tileCountArray[10 + tile.GetComponent<Tile>().TileNumber] += 1; // 11~19 筒
                        break;
                    case TileType.Bamboo:
                        tileCountArray[20 + tile.GetComponent<Tile>().TileNumber] += 1; // 21~29 條
                        break;
                    case TileType.Wind:
                        tileCountArray[30 + tile.GetComponent<Tile>().TileNumber] += 1; // 31~34 東南西北
                        break;
                    case TileType.Dragon:
                        tileCountArray[34 + tile.GetComponent<Tile>().TileNumber] += 1; // 35~37 中發白
                        break;
                    case TileType.Season:
                        tileCountArray[40 + tile.GetComponent<Tile>().TileNumber] += 1; // 41~44 春夏秋冬
                        break;
                    case TileType.Flower:
                        tileCountArray[44 + tile.GetComponent<Tile>().TileNumber] += 1; // 45~48 梅蘭竹菊
                        break;
                    default:
                        Debug.Log("Error tile type: " + tileType);
                        break;   
                }
            }
        }

        private void FindHighestTai(int[] nowTileArray, bool havePair, int ponCnt, int straightCnt)
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
            int[] tileCountArray = new int[50];

            //將所有手牌和擺牌轉為 array
            TransToArray(tileCountArray, handTilesList);
            TransToArray(tileCountArray, showTilesList);

            //地胡不計門清、自摸、不求人
            //天胡不計門清、自摸、不求人、獨聽、槓上開花
            //七搶一、八仙不計正花、花槓
            //花槓不計正花
            //小、大三元不再計三元刻
            //小、大四喜不再計圈風台、門風台
            if (Dealer())
            {
                calTai += 1;
                if (dealerWinStreak > 0)
                {
                    calTai += dealerWinStreak * 2;
                }
                
                if (TianHu())
                {
                    calTai += 24;
                }
            }
            if (DiHu())
            {
                calTai += 16;
            }

            //風
            if (DaSiXi(tileCountArray))
            {
                calTai += 16;
            }
            else if (XiaoSiXi(tileCountArray))
            {
                calTai += 8;
            }
            else
            {
                if (faceWind == 1 && Dong(tileCountArray))
                {
                    calTai += 1;
                }
                else if (faceWind == 2 && Nan(tileCountArray))
                {
                    calTai += 1;
                }
                else if (faceWind == 3 && Xi(tileCountArray))
                {
                    calTai += 1;
                }
                else if (faceWind == 4 && Bei(tileCountArray))
                {
                    calTai += 1;
                }

                else if (courtWind == 1 && Dong(tileCountArray))
                {
                    calTai += 1;
                }
                else if (courtWind == 2 && Nan(tileCountArray))
                {
                    calTai += 1;
                }
                else if (courtWind == 3 && Xi(tileCountArray))
                {
                    calTai += 1;
                }
                else if (courtWind == 4 && Bei(tileCountArray))
                {
                    calTai += 1;
                }
            }

            //三元
            if (DaSanYuan(tileCountArray))
            {
                calTai += 8;
            }
            else if (XiaoSanYuan(tileCountArray))
            {
                calTai += 4;
            }
            else
            {
                if (Zhong(tileCountArray))
                {
                    calTai += 1;
                }
                if (Fa(tileCountArray))
                {
                    calTai += 1;
                }
                if (Bai(tileCountArray))
                {
                    calTai += 1;
                }
            }

            //花
            if (BaXianGuoHai())
            {
                calTai += 8;
            }
            else if (QiQiangYi())
            {
                calTai += 8;
            }
            else
            {
                if (HuaGang(tileCountArray, true)) //春夏秋冬
                {
                    calTai += 2;
                }
                else if(HuaTai(tileCountArray, 40 + faceWind))
                {
                    calTai += 1;
                }

                if (HuaGang(tileCountArray, false)) //梅蘭竹菊
                {
                    calTai += 2;
                }
                else if(HuaTai(tileCountArray, 44 + faceWind))
                {
                    calTai += 1;
                }
            }

            //門清、自摸、不求人
            if (!(TianHu() || DiHu()))
            {
                if (MenQing() && SelfDraw())
                {
                    calTai += 3;
                }
                else if(MenQing())
                {
                    calTai += 1;
                }
                else if(SelfDraw())
                {
                    calTai += 1;
                }
            }

            //獨聽、槓上開花
            if (!TianHu())
            {
                if (DuTing())
                {
                    calTai += 1;
                }
                if (GangShangKaiHua())
                {
                    calTai += 1;
                }
            }

            if (HaiDiLaoYue())
            {
                calTai += 1;
            }

            if (HeDiLaoYu())
            {
                calTai += 1;
            }

            if (QuanQiuRen())
            {
                calTai += 2;
            }

            if (PingHu(tileCountArray, handPonCnt))
            {
                calTai += 2;
            }

            if (PengPengHu(handPonCnt))
            {
                calTai += 4;
            }

            if (SanAnKe(handPonCnt))
            {
                calTai += 2;
            }
            else if (SiAnKe(handPonCnt))
            {
                calTai += 5;
            }
            else if (WuAnKe(handPonCnt))
            {
                calTai += 8;
            }

            if (ZiYiSe(tileCountArray))
            {
                calTai += 16;
            }
            else if (QingYiSe(tileCountArray))
            {
                calTai += 8;
            }
            else if (HunYiSe(tileCountArray))
            {
                calTai += 4;
            }

            return calTai;
        }

        private List<string> CalculateScoring(int handPonCnt = 0, int handStraightCnt = 0)
        {
            List<string> calScoring = new List<string>();
            int[] tileCountArray = new int[50];

            //將所有手牌和擺牌轉為 array
            TransToArray(tileCountArray, handTilesList);
            TransToArray(tileCountArray, showTilesList);

            //地胡不計門清、自摸、不求人
            //天胡不計門清、自摸、不求人、獨聽、槓上開花
            //七搶一、八仙不計正花、花槓
            //花槓不計正花
            //小、大三元不再計三元刻
            //小、大四喜不再計圈風台、門風台
            if (Dealer())
            {
                calScoring.Add("莊家");
                if (dealerWinStreak > 0)
                {
                    calScoring.Add("連"+dealerWinStreak.ToString()+"拉"+dealerWinStreak.ToString());
                }
                
                if (TianHu())
                {
                    calScoring.Add("天胡");
                }
            }
            if (DiHu())
            {
                calScoring.Add("地胡");
            }

            //風
            if (DaSiXi(tileCountArray))
            {
                calScoring.Add("大四喜");
            }
            else if (XiaoSiXi(tileCountArray))
            {
                calScoring.Add("小四喜");
            }
            else
            {
                if (faceWind == 1 && Dong(tileCountArray))
                {
                    calScoring.Add("門風東");
                }
                else if (faceWind == 2 && Nan(tileCountArray))
                {
                    calScoring.Add("門風南");
                }
                else if (faceWind == 3 && Xi(tileCountArray))
                {
                    calScoring.Add("門風西");
                }
                else if (faceWind == 4 && Bei(tileCountArray))
                {
                    calScoring.Add("門風北");
                }

                if (courtWind == 1 && Dong(tileCountArray))
                {
                    calScoring.Add("場風東");
                }
                else if (courtWind == 2 && Nan(tileCountArray))
                {
                    calScoring.Add("場風南");
                }
                else if (courtWind == 3 && Xi(tileCountArray))
                {
                    calScoring.Add("場風西");
                }
                else if (courtWind == 4 && Bei(tileCountArray))
                {
                    calScoring.Add("場風北");
                }
            }

            //三元
            if (DaSanYuan(tileCountArray))
            {
                calScoring.Add("大三元");
            }
            else if (XiaoSanYuan(tileCountArray))
            {
                calScoring.Add("小三元");
            }
            else
            {
                if (Zhong(tileCountArray))
                {
                    calScoring.Add("中");
                }
                if (Fa(tileCountArray))
                {
                    calScoring.Add("發");
                }
                if (Bai(tileCountArray))
                {
                    calScoring.Add("白");
                }
            }

            //花
            if (BaXianGuoHai())
            {
                calScoring.Add("八仙過海");
            }
            else if (QiQiangYi())
            {
                calScoring.Add("七搶一");
            }
            else
            {
                if (HuaGang(tileCountArray, true)) //春夏秋冬
                {
                    calScoring.Add("四季");
                }
                else if(HuaTai(tileCountArray, 40 + faceWind))
                {
                    calScoring.Add(faceWind.ToString()+"花");
                }

                if (HuaGang(tileCountArray, false)) //梅蘭竹菊
                {
                    calScoring.Add("四君子");
                }
                else if(HuaTai(tileCountArray, 44 + faceWind))
                {
                    calScoring.Add(faceWind.ToString()+"花");
                }
            }

            //門清、自摸、不求人
            if (!(TianHu() || DiHu()))
            {
                if (MenQing() && SelfDraw())
                {
                    calScoring.Add("門清一摸三");
                }
                else if(MenQing())
                {
                    calScoring.Add("門清");
                }
                else if(SelfDraw())
                {
                    calScoring.Add("自摸");
                }
            }

            //獨聽、槓上開花
            if (!TianHu())
            {
                if (DuTing())
                {
                    calScoring.Add("獨聽");
                }
                if (GangShangKaiHua())
                {
                    calScoring.Add("槓上開花");
                }
            }

            if (HaiDiLaoYue())
            {
                calScoring.Add("海底撈月");
            }

            if (HeDiLaoYu())
            {
                calScoring.Add("河底撈魚");
            }

            if (QuanQiuRen())
            {
                calScoring.Add("全求人");
            }

            if (PingHu(tileCountArray, handPonCnt))
            {
                calScoring.Add("平胡");
            }

            if (PengPengHu(handPonCnt))
            {
                calScoring.Add("碰碰胡");
            }

            if (SanAnKe(handPonCnt))
            {
                calScoring.Add("三暗刻");
            }
            else if (SiAnKe(handPonCnt))
            {
                calScoring.Add("四暗刻");
            }
            else if (WuAnKe(handPonCnt))
            {
                calScoring.Add("五暗刻");
            }

            if (ZiYiSe(tileCountArray))
            {
                calScoring.Add("字一色");
            }
            else if (QingYiSe(tileCountArray))
            {
                calScoring.Add("清一色");
            }
            else if (HunYiSe(tileCountArray))
            {
                calScoring.Add("混一色");
            }
            
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
            return deckGangCnt + deckPonCnt + deckStraightCnt == 0;
        }

        private bool Zhong(int[] tileCountArray) //中
        {
            return tileCountArray[45] >= 3;
        }

        private bool Fa(int[] tileCountArray) //發
        {
            return tileCountArray[46] >= 3;
        }

        private bool Bai(int[] tileCountArray) //白
        {
            return tileCountArray[47] >= 3;
        }

        private bool Dong(int[] tileCountArray) //東
        {
            return tileCountArray[31] >= 3;
        }

        private bool Nan(int[] tileCountArray)//南
        {
            return tileCountArray[32] >= 3;
        }

        private bool Xi(int[] tileCountArray)//西
        {
            return tileCountArray[33] >= 3;
        }

        private bool Bei(int[] tileCountArray)//北
        {
            return tileCountArray[34] >= 3;
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
            return tileCountArray[pos] == 1;
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
            return (!isSelfDraw) && deckGangCnt + deckPonCnt + deckStraightCnt == 5;
        }

        private bool PingHu(int[] tileCountArray, int handPonCnt)//平胡
        {
            //檢查無字無花
            int check = 0;
            for (int i = 31; i <= 48; i++)
            {
                check += tileCountArray[i];
            }

            return (!isSelfDraw) && deckGangCnt + deckPonCnt + hideGangCnt + handPonCnt + check == 0;
        }

        private bool SanAnKe(int handPonCnt)//三暗刻
        {
            return handPonCnt + hideGangCnt == 3;
        }

        private bool PengPengHu(int handPonCnt)//碰碰胡
        {
            return handPonCnt + hideGangCnt + deckPonCnt + deckGangCnt == 5;
        }

        private bool HunYiSe(int[] tileCountArray)//混一色
        {
            int characterCnt = 0;
            int dotCnt = 0;
            int bambooCnt = 0;
            int letterCnt = 0;

            for (int i = 1; i <= 9; i++)
            {
                characterCnt += tileCountArray[i];
            }
            for (int i = 11; i <= 19; i++)
            {
                dotCnt += tileCountArray[i];
            }
            for (int i = 21; i <= 29; i++)
            {
                bambooCnt += tileCountArray[i];
            }
            for (int i = 31; i <= 37; i++)
            {
                letterCnt += tileCountArray[i];
            }

            return (letterCnt > 0) && ((characterCnt + dotCnt == 0) || (characterCnt + bambooCnt == 0) || (dotCnt + bambooCnt == 0));
        }

        private bool XiaoSanYuan(int[] tileCountArray)//小三元
        {
            return tileCountArray[35] >= 2 && tileCountArray[36] >= 2 && tileCountArray[37] >= 2;
        }

        private bool SiAnKe(int handPonCnt)//四暗刻
        {
            return handPonCnt + hideGangCnt == 4;
        }

        private bool WuAnKe(int handPonCnt)//五暗刻
        {
            return handPonCnt + hideGangCnt == 5;
        }

        private bool QingYiSe(int[] tileCountArray)//清一色
        {
            int characterCnt = 0;
            int dotCnt = 0;
            int bambooCnt = 0;
            int letterCnt = 0;

            for (int i = 1; i <= 9; i++)
            {
                characterCnt += tileCountArray[i];
            }
            for (int i = 11; i <= 19; i++)
            {
                dotCnt += tileCountArray[i];
            }
            for (int i = 21; i <= 29; i++)
            {
                bambooCnt += tileCountArray[i];
            }
            for (int i = 31; i <= 37; i++)
            {
                letterCnt += tileCountArray[i];
            }

            return (letterCnt == 0) && ((characterCnt + dotCnt == 0) || (characterCnt + bambooCnt == 0) || (dotCnt + bambooCnt == 0));
        }

        private bool XiaoSiXi(int[] tileCountArray)//小四喜
        {
            return tileCountArray[31] >= 2 && tileCountArray[32] >= 2 && tileCountArray[33] >= 2 && tileCountArray[34] >= 2;
        }

        private bool DaSanYuan(int[] tileCountArray)//大三元
        {
            return tileCountArray[35] >= 3 && tileCountArray[36] >= 3 && tileCountArray[37] >= 3;
        }

        private bool QiQiangYi()//七搶一
        {
            return (!isSelfDraw) && (winningTile.GetComponent<Tile>().TileType == TileType.Season || winningTile.GetComponent<Tile>().TileType == TileType.Flower);
        }

        private bool BaXianGuoHai()//八仙過海
        {
            return isSelfDraw && (winningTile.GetComponent<Tile>().TileType == TileType.Season || winningTile.GetComponent<Tile>().TileType == TileType.Flower);
        }

        private bool ZiYiSe(int[] tileCountArray)//字一色
        {
            //檢查無萬筒條
            int cnt = 0;
            for (int i = 1; i <= 29; i++)
            {
                cnt += tileCountArray[i];
            }

            return cnt == 0;
        }

        private bool DaSiXi(int[] tileCountArray)//大四喜
        {
            return tileCountArray[31] >= 3 && tileCountArray[32] >= 3 && tileCountArray[33] >= 3 && tileCountArray[34] >= 3;
        }

        private bool DiHu()//地胡
        {
            return isFirstTile && isSelfDraw && (!isDealer);
        }

        private bool TianHu()//天胡
        {
            return isFirstTile && isSelfDraw && isDealer;
        }
    }
}
