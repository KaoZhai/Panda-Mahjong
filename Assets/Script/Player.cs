using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PlayingRoom {
    public enum KongType
    {
        Normal,
        Plus,
        Dark,
        None
    }
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
        private KongType kongStatus = KongType.None;

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

        void ShowBackTile(int indexOfHandTiles)
        {
            GameObject blankTile = handTiles[indexOfHandTiles].transform.GetChild(0).gameObject;
            blankTile.SetActive(false);
            GameObject face = handTiles[indexOfHandTiles].transform.GetChild(1).gameObject;
            face.SetActive(false);
            ShowTile(indexOfHandTiles);
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

        public bool IsPlayerCanKongBySelf()
        {
            
            // (pinyin) An Gang
            Dictionary<int, int> tileCounter = new Dictionary<int, int>();
            foreach(GameObject tile in handTiles)
            {
                int cardFaceIndex = tile.GetComponent<Tile>().CardFaceIndex;
                if (tileCounter.ContainsKey(cardFaceIndex))
                {
                    ++ tileCounter[cardFaceIndex];
                }
                else
                {
                    tileCounter.Add(cardFaceIndex, 1);
                }
            }
            foreach(KeyValuePair<int, int> cnt in tileCounter)
            {
                if (cnt.Value == 4)
                {
                    kongStatus = KongType.Dark;
                    return true;
                }
            }
            // (pinyin) Jia Gang
            foreach(GameObject handTile in handTiles)
            {
                foreach(List<GameObject> pongTiles in showPongTiles)
                {
                    if (handTile.GetComponent<Tile>().IsSame(pongTiles[0].GetComponent<Tile>()))
                    {
                        kongStatus = KongType.Plus;
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsPlayerCanKong()
        {
            if ((tableManager.ActivePlayerId + 1) % 4 == playerId || tableManager.ActivePlayerId == playerId)
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
                kongStatus = KongType.Normal;
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
        
        


        // todo
        public void Kong()
        {
            if(kongStatus == KongType.Normal)
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
            else if(kongStatus == KongType.Dark)
            {
                Dictionary<int, List<int>> tileCounter = new Dictionary<int, List<int>>();
                for(int i = handTiles.Count - 1; i >= 0 ; --i)
                {
                    int cardFaceIndex = handTiles[i].GetComponent<Tile>().CardFaceIndex;
                    if (!tileCounter.ContainsKey(cardFaceIndex))
                    {
                        tileCounter.Add(cardFaceIndex, new List<int>());
                    }
                        
                    tileCounter[cardFaceIndex].Add(i);
                  
                }
                List<GameObject> tileSet = new List<GameObject>();
                foreach(KeyValuePair<int, List<int>> cnt in tileCounter)
                {
                    if (cnt.Value.Count == 4)
                    {
                        foreach(int i in cnt.Value)
                        {
                            tileSet.Add(handTiles[i]);
                            ShowBackTile(i);
                        }
                        showKongTiles.Add(tileSet);
                        break;
                    }
                }
            }
            else if(kongStatus == KongType.Plus)
            {
                List<GameObject> tileSet = new List<GameObject>();
                
                for(int i = handTiles.Count-1; i >= 0; --i)
                {
                    for(int j = showPongTiles.Count - 1; j >= 0; --j)
                    {
                        if (showPongTiles[j][0].GetComponent<Tile>().IsSame(handTiles[i].GetComponent<Tile>()))
                        {
                            showPongTiles[j].Add(handTiles[i]);
                            ShowTile(i);
                            handTiles.RemoveAt(i);
                            showKongTiles.Add(showPongTiles[j]);
                            showPongTiles.RemoveAt(j);
                        }
                    }
                }
                showKongTiles.Add(tileSet);
            }
            else
            {
                Debug.Log("kong status error");
            }
        }
        
    }

}

