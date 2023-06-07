using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Play {
    public class Player : MonoBehaviour
    {
        private readonly List<GameObject> handTiles = new();
        private readonly List<GameObject> showTiles = new();
        
        [SerializeField] private Transform hand;
        [SerializeField] private Transform tilePool;
        [SerializeField] private Transform showPool;

        [SerializeField] private GameObject chiTileSets;
        [SerializeField] private Transform chiTileSetsTransform;
        
        [SerializeField] private GameObject setPrefab;

        private TaiCalculator taiCalculator = new(null, null);
        private readonly List<List<GameObject>> canChiTileSet = new();

        private int kongCnt;
        private int ponCnt;
        private int straightCnt;

        private bool isSelfDraw;
        private readonly bool isDealer = true;
        private bool isOnly;

        public readonly IProcessStrategy Chi;
        public readonly IProcessStrategy Pong;
        public readonly IProcessStrategy Kong;

        public Player()
        {
            Chi = new ChiStrategy(this);
            Pong = new PongStrategy(this);
            Kong = new KongStrategy(this);
        }

        public TableManager TableManager { get; set; }

        public int PlayerIndex { set; get; } = -1;

        public Transform Hand => hand;

        public Transform TilePool => tilePool;

        public Transform ShowPool => showPool;

        public TaiCalculator TaiCalculator => taiCalculator;

        public int Point { get; set; }

        public bool IsSelfDraw => isSelfDraw;
        
        private int HandTilesCnt => handTiles.Count;


        #region - TileOperations
        
        public void CallCalculator()
        {
            if (!isSelfDraw)
            {
                showTiles.Add(TableManager.LastTile);
            }

            taiCalculator = new TaiCalculator(handTiles, showTiles, kongCnt, ponCnt, straightCnt, 1, 1, 0, 0,
                isDealer, false, false, isSelfDraw, false, isOnly);
        }
        
        public bool ReplaceFlower()
        {
            int cnt = 0;
            for(int i = handTiles.Count-1; i >= 0; --i)
            {
                if (handTiles[i].GetComponent<Tile>().IsFlower())
                {
                    ShowTile(i);
                    ++cnt;
                }
            }

            for(int i = 0; i < cnt; ++i)
            {
                TableManager.TileWall.Replenish(this);
            }

            return cnt != 0;
        }
        
        public void Discard(string tileId)
        {
            int idx = FindTileByTileId(handTiles, tileId);

            if (idx != -1)
            {
                TableManager.LastTile = handTiles[idx];
                handTiles[idx].transform.SetParent(tilePool, false);
                handTiles[idx].SetActive(true);
                handTiles.RemoveAt(idx);
            }
            else
            {
                Debug.LogError("Cannot find tile from Tile ID");
            }
            
            SortHandTiles();
            StartCoroutine(TableManager.BeforeNextPlayer());
        }
        
        public void DefaultDiscard()
        {
            Discard(handTiles[Random.Range(0, HandTilesCnt)].GetComponent<Tile>().TileId);
            // Discard(handTiles[handTiles.Count-1].GetComponent<Tile>().TileId);
        }

        public void SortHandTiles()
        {
            handTiles.Sort(new TileGameObjectComparer());
            foreach(GameObject t in handTiles)
            {
                t.transform.SetAsLastSibling();
            }
        }
        public void GetTile(GameObject tile)
        {
            handTiles.Add(tile);
            tile.GetComponent<Tile>().Player = this;
            tile.transform.SetParent(hand, false);
            tile.SetActive(true);
        }

        void ShowTile(int indexOfHandTiles)
        {
            handTiles[indexOfHandTiles].transform.SetParent(showPool, false);
            showTiles.Add(handTiles[indexOfHandTiles]);
            handTiles.RemoveAt(indexOfHandTiles);
        }
        
        void TakeTileFromOther()
        {
            TableManager.LastTile.transform.SetParent(showPool, false);
            showTiles.Add(TableManager.LastTile);
        }

        #endregion

        #region - ChinPongGangHu

        public interface IProcessStrategy
        {
            public bool CanDoOperation();
            public void DoOperation();
        }

        private class ChiStrategy: IProcessStrategy
        {
            private readonly Player player;

            public ChiStrategy(Player player)
            {
                this.player = player;
            }
            public bool CanDoOperation()
            {
                if (player.TableManager.ActivePlayerIndex == player.PlayerIndex)
                    return false;
                if ((player.TableManager.ActivePlayerIndex + 1) % 4 != player.PlayerIndex)
                    return false;
                player.canChiTileSet.Clear();
                Tile newTile = player.TableManager.LastTile.GetComponent<Tile>();
                // Debug.Log(newTile.TileType.ToString() + newTile.TileNumber.ToString());
                bool isCanChi = false;
                if (!newTile.IsSuit())
                    return false;
           
                int i = newTile.TileNumber;
                // i-2, i-1, i
                isCanChi |= player.Find(newTile.TileType, i-2, i-1);
                // i-1, i, i+1
                isCanChi |= player.Find(newTile.TileType, i-1, i+1);
                // i, i+1, i+2
                isCanChi |= player.Find(newTile.TileType, i+1, i+2);

                return isCanChi;
            }

            public void DoOperation()
            {
                GameObject set = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                Transform[] children = set.GetComponentsInChildren<Transform>();

                bool first = true;
                foreach (Transform child in children)
                {
                    if (child == set.transform) continue;
                    string tileId = child.name;

                    int idx = player.FindTileByTileId(player.handTiles, tileId, "REVERSE");

                    if (idx != -1)
                    {
                        player.ShowTile(idx);
                        if (first)
                        {
                            player.TakeTileFromOther();
                            first = false;
                        }
                    }
                }
            
                CloseChiSelection();
                player.straightCnt++;
            }
            
            private void CloseChiSelection()
            {
                player.chiTileSets.SetActive(false);
                Transform[] children = player.chiTileSets.GetComponentsInChildren<Transform>();
                foreach (Transform child in children)
                {
                    if (child == player.chiTileSets.transform) continue;
                    Destroy(child.gameObject);
                }
            }
        }

        private class PongStrategy : IProcessStrategy
        {
            private readonly Player player;

            public PongStrategy(Player player)
            {
                this.player = player;
            }
            public bool CanDoOperation()
            {
                if (player.TableManager.ActivePlayerIndex == player.PlayerIndex)
                    return false;
                bool isCanPong = false;
                int numSame = 0;
                foreach(GameObject tile in player.handTiles)
                {
                    if (player.TableManager.LastTile && tile.GetComponent<Tile>().Equals(player.TableManager.LastTile.GetComponent<Tile>()))
                        ++numSame;
                }
                if (numSame >= 2)
                {
                    isCanPong = true;

                }
                return isCanPong;
            }

            public void DoOperation()
            {
                player.TakeTileFromOther();
                int cnt = 0;
                for(int i = player.handTiles.Count-1; i >= 0; --i)
                {
                    if (player.handTiles[i].GetComponent<Tile>().Equals(player.TableManager.LastTile.GetComponent<Tile>()))
                    {
                        ++cnt;
                        player.ShowTile(i);
                    }
                    if (cnt == 2)
                        break;
                }
                player.ponCnt++;
            }
        }

        private class KongStrategy : IProcessStrategy
        {
            private readonly Player player;

            public KongStrategy(Player player)
            {
                this.player = player;
            }
            public bool CanDoOperation()
            {
                if (((player.TableManager.ActivePlayerIndex + 1) % 4 == player.PlayerIndex) || player.TableManager.LastTile.GetComponent<Tile>().PlayerIndex == player.PlayerIndex)
                {
                    return false;
                }
                bool isCanGang = false;
                int numSame = 0;
                foreach(GameObject tile in player.handTiles)
                {
                    if (player.TableManager.LastTile && tile.GetComponent<Tile>().Equals(player.TableManager.LastTile.GetComponent<Tile>()))
                        ++numSame;
                }
                if (numSame == 3)
                {
                    isCanGang = true;

                }
                return isCanGang;
            }

            public void DoOperation()
            {
                player.TakeTileFromOther();
                int cnt = 0;
                for(int i = player.handTiles.Count-1; i >= 0; --i)
                {
                    if (player.handTiles[i].GetComponent<Tile>().Equals(player.TableManager.LastTile.GetComponent<Tile>()))
                    {
                        ++cnt;
                        player.ShowTile(i);
                    }
                    if (cnt == 3)
                        break;
                }
                player.kongCnt++;
            }
        }

        public bool IsPlayerCanHu(bool selfDraw)
        {
            int[] tileCountArray = new int[50];
            TransToArray(tileCountArray, handTiles);

            isSelfDraw = selfDraw;

            if (!isSelfDraw)
            {
                this.isOnly = CheckIsOnly((int[])tileCountArray.Clone());
                GameObject[] lastTileArray = {TableManager.LastTile};
                List<GameObject> lastTile = new List<GameObject>(lastTileArray);
                TransToArray(tileCountArray, lastTile);
            }
            
            return CheckHu((int[])tileCountArray.Clone(), false);
        }
        
        #endregion
        
        #region - HelperFunction

        private int FindTileByTileId(List<GameObject> tiles, string tileId, string mode = "NORMAL")
        {
            if (mode == "NORMAL")
            {
                for (int i = 0; i < tiles.Count; ++i)
                {
                    if (tiles[i].GetComponent<Tile>().TileId == tileId)
                    {
                        return i;
                    }
                }
            }
            else if(mode == "REVERSE")
            {
                for (int i = tiles.Count - 1; i >= 0; --i)
                {
                    if (tiles[i].GetComponent<Tile>().TileId == tileId)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private bool CheckIsOnly(int[] nowHandArray)
        {
            int cnt = 0;
            for (int i = 1; i <= 37; i++)
            {
                if (i == 10 || i == 20 || i == 30)
                    continue;
                
                nowHandArray[i]++;
                if (CheckHu((int[])nowHandArray.Clone() ,false))
                {
                    cnt ++;
                }
                nowHandArray[i]--;
            }

            return cnt == 1;
        }

        // recursion
        private static bool CheckHu(int[] nowTileArray, bool havePair)
        {
            if(nowTileArray.Sum() == 0)
            {
                return true;
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
                        if (CheckHu((int[])nowTileArray.Clone(), true))
                        {
                            return true;
                        }
                        nowTileArray[i - 1] += 1;
                        nowTileArray[i] += 1;
                        nowTileArray[i + 1] += 1;
                    }
                    // check pon
                    if(nowTileArray[i] >= 3)
                    {
                        nowTileArray[i] -= 3;
                        if (CheckHu((int[])nowTileArray.Clone(), true))
                        {
                            return true;
                        }
                        nowTileArray[i] += 3;
                    }
                }
                // no pair yet
                else
                {
                    if(nowTileArray[i] >= 2)
                    {
                        nowTileArray[i] -= 2;
                        if (CheckHu((int[])nowTileArray.Clone(), true))
                        {
                            return true;
                        }
                        nowTileArray[i] += 2;
                    }
                }
            }
            return false;
        }

        private void TransToArray(int[] tileCountArray, List<GameObject> tileList)
        {
            foreach (var tile in tileList)
            {
                int offset = 0;
                switch (tile.GetComponent<Tile>().TileType)
                {
                    case TileType.Character:
                        offset = 0; // 1~9 萬
                        break;
                    case TileType.Dot:
                        offset = 10; // 11~19 筒
                        break;
                    case TileType.Bamboo:
                        offset = 20; // 21~29 條
                        break;
                    case TileType.Wind:
                        offset = 30; // 31~34 東南西北
                        break;
                    case TileType.Dragon:
                        offset = 34; // 35~37 中發白
                        break;
                    case TileType.Season:
                        offset = 40; // 41~44 春夏秋冬
                        break;
                    case TileType.Flower:
                        offset = 44; // 45~48 梅蘭竹菊
                        break;
                    default:
                        Debug.Log("Error tile type: " + tile.GetComponent<Tile>().TileType);
                        break;
                }
                
                tileCountArray[offset + tile.GetComponent<Tile>().TileNumber] += 1;
            }
        }

        
        private bool Find(TileType tileType, int lostA, int lostB)
        {
            if (lostA < 1 || lostB > 9)
                return false; 
            GameObject a = handTiles.Find(t => t.GetComponent<Tile>().TileType == tileType &&
                                                t.GetComponent<Tile>().TileNumber == lostA);
            GameObject b = handTiles.Find(t => t.GetComponent<Tile>().TileType == tileType &&
                                                t.GetComponent<Tile>().TileNumber == lostB);
    
            
            if (a != null && b != null)
            {
                List<GameObject> set = new List<GameObject>
                {
                    a,
                    b
                };
                canChiTileSet.Add(set);
                return true;
            }   
            return false;
        }

        #endregion

        #region - BtnCallBack

        public void DecideHowToChi()
        {
            chiTileSets.SetActive(true);
            foreach (var t in canChiTileSet)
            {
                GameObject set = Instantiate(setPrefab, new Vector3(0, 0, 0), Quaternion.identity, chiTileSetsTransform);
                set.SetActive(true);
                GameObject a = Instantiate(t[0], set.transform, false);
                a.name = t[0].GetComponent<Tile>().TileId;
                GameObject b = Instantiate(t[1], set.transform, false);
                b.name = t[1].GetComponent<Tile>().TileId;
            }
        }
        
        public void ChiBtnCallBack()
        {
            Chi.DoOperation();
        }

        public void PongBtnCallBack()
        {
            Pong.DoOperation();
        }

        public void KongBtnCallBack()
        {
            Kong.DoOperation();
        }

        #endregion
    }

}

