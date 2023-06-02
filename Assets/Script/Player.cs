using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.PlayingRoom {
    public class Player : MonoBehaviour
    {
        private List<GameObject> handTiles = new List<GameObject>();

        private List<GameObject> showTiles = new List<GameObject>();

        private List<List<GameObject>> showChiTiles = new List<List<GameObject>>();
        private List<List<GameObject>> showPongTiles = new List<List<GameObject>>();
        private List<List<GameObject>> showKongTiles = new List<List<GameObject>>();


        [SerializeField] private Transform hand = null;
        [SerializeField] private Transform tilePool = null;
        [SerializeField] private Transform showPool = null;

        [SerializeField] private GameObject chiTileSets = null;
        [SerializeField] private Transform chiTileSetsTransform = null;
        
        [SerializeField] private GameObject setPrefab = null;

        private int playerId = -1;
        private int point;

        private TableManager tableManager;

        public TableManager TableManager
        {
            get { return tableManager; }
            set { tableManager = value; }
        }

        public int PlayerId {
            set { playerId = value; }
            get { return playerId; }
        }

        public Transform Hand
        {
            get { return hand; }
        }

        public Transform TilePool
        {
            get { return tilePool; }
        }

        public Transform ShowPool
        {
            get { return showPool; }
        }
        public int Point {
            get { return point; }
            set { point = value; }
        }

        public void SortHandTiles()
        {
            handTiles.Sort(new TileGameObjectComparer());
            foreach(GameObject t in handTiles)
            {
                t.transform.SetAsLastSibling();
            }
        }
        public int HandTilesCnt
        {
            get { return handTiles.Count; }
        }
        public void GetTile(GameObject tile)
        {
            handTiles.Add(tile);
            tile.GetComponent<Tile>().Player = this;
            tile.transform.SetParent(hand, false);
            tile.SetActive(true);
        }

        public bool IsDoneReplace()
        {
            bool isDone = true;
            for(int i = 0; i < handTiles.Count; ++i)
            {
                if (handTiles[i].GetComponent<Tile>().IsFlower())
                {
                    isDone = false;
                }
            }
            return isDone;
        }
        
        void ShowTile(int indexOfHandTiles)
        {
            handTiles[indexOfHandTiles].transform.SetParent(showPool, false);
            showTiles.Add(handTiles[indexOfHandTiles]);
            handTiles.RemoveAt(indexOfHandTiles);
        }

        void TakeTileFromOther()
        {
            tableManager.LastTile.transform.SetParent(showPool, false);
            showTiles.Add(tableManager.LastTile);
        }
        public void ReplaceFlower()
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
                tableManager.TileWall.BuPai(this);
            }
        }


        public void DefaultDiscard()
        {
            Discard(handTiles[Random.Range(0, HandTilesCnt)].GetComponent<Tile>().TileId);

            
            // Discard(handTiles[handTiles.Count-1].GetComponent<Tile>().TileId);
        }

        public void Discard(string tileId)
        {
            for(int i = 0; i < handTiles.Count; ++i)
            {
                if (handTiles[i].GetComponent<Tile>().TileId == tileId)
                {
                    tableManager.LastTile = handTiles[i];
                    handTiles[i].transform.SetParent(tilePool, false);
                    handTiles[i].SetActive(true);
                    handTiles.RemoveAt(i);
                    break;
                }
            }
            SortHandTiles();
            StartCoroutine(tableManager.BeforeNextPlayer());
        }

        public bool IsPlayerCanKong()
        {
            if (((tableManager.ActivePlayerId + 1) % 4 == playerId) || tableManager.LastTile.GetComponent<Tile>().PlayerId == playerId)
            {
                return false;
            }
            bool isCanGang = false;
            int numSame = 0;
            foreach(GameObject tile in handTiles)
            {
                if (tableManager.LastTile && tile.GetComponent<Tile>().IsSame(tableManager.LastTile.GetComponent<Tile>()))
                    ++numSame;
            }
            if (numSame == 3)
            {
                isCanGang = true;

            }
            return isCanGang;
        }
        
        public bool IsPlayerCanPong()
        {
            if (tableManager.ActivePlayerId == playerId)
                return false;
            bool isCanPong = false;
            int numSame = 0;
            foreach(GameObject tile in handTiles)
            {
                if (tableManager.LastTile && tile.GetComponent<Tile>().IsSame(tableManager.LastTile.GetComponent<Tile>()))
                    ++numSame;
            }
            if (numSame >= 2)
            {
                isCanPong = true;

            }
            return isCanPong;
        }

        public bool IsPlayerCanHu(bool isSelfDraw)
        {
            bool isCanHu = false;
            int[] tileCountArray = new int[50];
            TransToArray(tileCountArray, handTiles);
            if (!isSelfDraw)
            {
                GameObject[] LastTileArray = {tableManager.LastTile};
                List<GameObject> lastTile = new List<GameObject>(LastTileArray);
                TransToArray(tileCountArray, lastTile);
            }

            isCanHu = checkHu((int[])tileCountArray.Clone(), false);
            return isCanHu;
        }

        // recursion
        private bool checkHu(int[] nowTileArray, bool havePair)
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
                        if (checkHu((int[])nowTileArray.Clone(), true))
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
                        if (checkHu((int[])nowTileArray.Clone(), true))
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
                        if (checkHu((int[])nowTileArray.Clone(), true))
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
                if (tile.GetComponent<Tile>().TileType == TileType.Character)
                {
                    tileCountArray[0 + tile.GetComponent<Tile>().TileNumber] += 1; // 1~9 萬
                }
                else if (tile.GetComponent<Tile>().TileType == TileType.Dot)
                {
                    tileCountArray[10 + tile.GetComponent<Tile>().TileNumber] += 1; // 11~19 筒
                }
                else if (tile.GetComponent<Tile>().TileType == TileType.Bamboo)
                {
                    tileCountArray[20 + tile.GetComponent<Tile>().TileNumber] += 1; // 21~29 條
                }
                else if (tile.GetComponent<Tile>().TileType == TileType.Wind)
                {
                    tileCountArray[30 + tile.GetComponent<Tile>().TileNumber] += 1; // 31~34 東南西北
                }
                else if (tile.GetComponent<Tile>().TileType == TileType.Dragon)
                {
                    tileCountArray[34 + tile.GetComponent<Tile>().TileNumber] += 1; // 35~37 中發白
                }
                else if (tile.GetComponent<Tile>().TileType == TileType.Season)
                {
                    tileCountArray[40 + tile.GetComponent<Tile>().TileNumber] += 1; // 41~44 春夏秋冬
                }
                else if (tile.GetComponent<Tile>().TileType == TileType.Flower)
                {
                    tileCountArray[44 + tile.GetComponent<Tile>().TileNumber] += 1; // 45~48 梅蘭竹菊
                }
                else
                {
                    Debug.Log("Error tile type: " + tile.GetComponent<Tile>().TileType);
                }
            }
        }

        private List<List<GameObject>> canChiTileSet = new List<List<GameObject>>();
        bool Find(TileType tileType, int lostA, int lostB)
        {
            if (lostA < 1 || lostB > 9)
                return false; 
            GameObject a = handTiles.Find(t => t.GetComponent<Tile>().TileType == tileType &&
                                                t.GetComponent<Tile>().TileNumber == lostA);
            GameObject b = handTiles.Find(t => t.GetComponent<Tile>().TileType == tileType &&
                                                t.GetComponent<Tile>().TileNumber == lostB);
    
            
            if (a != null && b != null)
            {
                List<GameObject> set = new List<GameObject>();
                set.Add(a);
                set.Add(b);
                canChiTileSet.Add(set);
                return true;
            }   
            return false;
        }
        public bool IsPlayerCanChi()
        {
            if (tableManager.ActivePlayerId == playerId)
                return false;
            if ((tableManager.ActivePlayerId + 1) % 4 != playerId)
                return false;
            canChiTileSet.Clear();
            Tile newTile = tableManager.LastTile.GetComponent<Tile>();
            // Debug.Log(newTile.TileType.ToString() + newTile.TileNumber.ToString());
            bool isCanChi = false;
            if (!newTile.IsSuit())
                return false;
           
            int i = newTile.TileNumber;
            // i-2, i-1, i
            isCanChi |= Find(newTile.TileType, i-2, i-1);
            // i-1, i, i+1
            isCanChi |= Find(newTile.TileType, i-1, i+1);
            // i, i+1, i+2
            isCanChi |= Find(newTile.TileType, i+1, i+2);

            return isCanChi;
        }

        public void DecideHowToChi()
        {
            chiTileSets.SetActive(true);
            for (int i = 0; i < canChiTileSet.Count; ++i)
            {
                GameObject set = Instantiate(setPrefab, new Vector3(0, 0, 0), Quaternion.identity, chiTileSetsTransform);
                set.SetActive(true);
                GameObject a = Instantiate(canChiTileSet[i][0]);
                a.transform.SetParent(set.transform, false);
                a.name = canChiTileSet[i][0].GetComponent<Tile>().TileId;
                GameObject b = Instantiate(canChiTileSet[i][1]);
                b.transform.SetParent(set.transform, false);
                b.name = canChiTileSet[i][1].GetComponent<Tile>().TileId;
            }
        }

        void CloseChiSelection()
        {
            chiTileSets.SetActive(false);
            Transform[] children = chiTileSets.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child == chiTileSets.transform) continue;
                string tileId = child.name;
                Destroy(child.gameObject);
            }
        }
        public void Chi()
        {
            List<GameObject> tileSet = new List<GameObject>();
            GameObject set = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            Transform[] children = set.GetComponentsInChildren<Transform>();

            bool first = true;
            foreach (Transform child in children)
            {
                if (child == set.transform) continue;
                string tileId = child.name;
                for(int i = handTiles.Count-1; i >= 0; --i)
                {
                    if (handTiles[i].GetComponent<Tile>().TileId == tileId)
                    {
                        tileSet.Add(handTiles[i]);
                        ShowTile(i);
                        if (first)
                        {
                            TakeTileFromOther();
                            first = false;
                        }
                        break;
                    }
                }
            }
            
            
            showChiTiles.Add(tileSet);
            CloseChiSelection();
        }
        public void Pong()
        {
            List<GameObject> tileSet = new List<GameObject>();
            TakeTileFromOther();
            int cnt = 0;
            for(int i = handTiles.Count-1; i >= 0; --i)
            {
                if (handTiles[i].GetComponent<Tile>().IsSame(tableManager.LastTile.GetComponent<Tile>()))
                {
                    ++cnt;
                    tileSet.Add(handTiles[i]);
                    ShowTile(i);
                }
                if (cnt == 2)
                    break;
            }
            showPongTiles.Add(tileSet);
        }
        
        public void Kong()
        {
            List<GameObject> tileSet = new List<GameObject>();
            TakeTileFromOther();
            int cnt = 0;
            for(int i = handTiles.Count-1; i >= 0; --i)
            {
                if (handTiles[i].GetComponent<Tile>().IsSame(tableManager.LastTile.GetComponent<Tile>()))
                {
                    ++cnt;
                    tileSet.Add(handTiles[i]);
                    ShowTile(i);
                }
                if (cnt == 3)
                    break;
            }
            showKongTiles.Add(tileSet);
        }
        
    }

}

