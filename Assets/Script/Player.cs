using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.PlayingRoom {
    
    public enum TileStatus
    {
        Hand,
        Flower,
        Chi,
        Pong, 
        MingGang,
        AnGang
    }
    public class Player : MonoBehaviour
    {
        private List<(GameObject tile, TileStatus tileStatus)> tilesList = new List<Tuple<GameObject, TileStatus>>();
        private List<List<GameObject>> choiceGang = new List<List<GameObject>>();
        private List<List<GameObject>> choiceChi = new List<List<GameObject>>();
        [SerializeField] private Transform hand = null;
        [SerializeField] private Transform tilePool = null;
        [SerializeField] private Transform showPool = null;

        [SerializeField] private GameObject choicePanel = null;
        [SerializeField] private GameObject setPrefab = null;

        private string playerId = "-1";

        private string lastPlayerId = "-1";
        private int point;

        private TableManager tableManager;

        public TableManager TableManager
        {
            get { return tableManager; }
            set { tableManager = value; }
        }

        public string PlayerId {
            set { playerId = value; }
            get { return playerId; }
        }

        public int Point {
            get { return point; }
            set { point = value; }
        }   
        public Transform Hand {
            get { return hand; }
        }

        public string PlayerId {
            get { return playerId; }
        }
        public void SortHandTiles()
        {
            tilesList.Sort(new TileGameObjectComparer());
            foreach(GameObject t in tilesList)
            {
                tilesList.transform.SetAsLastSibling();
            }
        }
        public int HandTilesCnt
        {
            get { return tilesList.Count; }
        }
        public void GetTile(GameObject tile)
        {
            tilesList.Add((tile, TileStatus.Hand));
            tile.GetComponent<Tile>().Player = this;
            tile.transform.SetParent(hand, false);
            tile.SetActive(true);
        }

        public bool IsDoneBuHua()
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
        
        void ShowTile(int indexOfHandTiles, TileStatus tileStatus)
        {
            handTiles[indexOfHandTiles].transform.SetParent(showPool, false);
            tilesList[indexOfHandTiles].tileStatus = tileStatus;
        }

        void ShowBackTile(int indexOfHandTiles, TileStatus tileStatus)
        {
            GameObject blankTile = handTiles[indexOfHandTiles].transform.GetChild(0).gameObject;
            blankTile.SetActive(false);
            GameObject face = handTiles[indexOfHandTiles].transform.GetChild(1).gameObject;
            face.SetActive(false);
            ShowTile(indexOfHandTiles, tileStatus);
        }
        void TakeTileFromOther(TileStatus tileStatus)
        {
            tableManager.LastTile.transform.SetParent(showPool, false);
            tilesList.Add((tableManager.LastTile, tileStatus));
        }
        public int ShowFlower()
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
            return cnt;
        }


        public void DefaultDiscard()
        {
            Discard(handTiles[tilesList.Count-1].GetComponent<Tile>().TileId);
        }

        public void Discard(string tileId)
        {
            if (tableManager.MoveStatus != MovementType.None)
                return;
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
            tableManager.MoveStatus = MovementType.PlayTile;
        }

        public bool IsCanSelfDrawGang()
        {
            choiceGang.Clear();
            // An Gang
            Dictionary<int, List<GameObject>> tileCounter = new Dictionary<int, List<GameObject>>();
            for(int i = 0; i < tilesList.Count; ++i)
            {
                if (tilesList[i].status != TileStatus.Hand)
                    continue;
                int cardFaceIndex = tilesList[i].tile.GetComponent<Tile>().CardFaceIndex;
                if (!tileCounter.ContainsKey(cardFaceIndex))
                {
                    tileCounter.Add(cardFaceIndex, new List<GameObject>());
                }
                tileCounter[cardFaceIndex].Add(tilesList[i].tile);
            }
            foreach(KeyValuePair<int, int> cnt in tileCounter)
            {
                if (cnt.Value.Count == 4)
                {
                    choiceGang.Add(cnt.Value);

                }
            }
            // Jia Gang
            for(int i = 0; i < tilesList.Count; ++i)
            {
                if (tilesList[i].status != TileStatus.Hand)
                    continue;
                List<GameObject> set = new List<GameObject>();
                Tile tile = tilesList[i].tile.GetComponent<Tile>();
                for(int j = 0; j < tilesList.Count; ++j)
                {
                    if ( tilesList[j].tileStatus == TileStatus.Pong && tile.IsSame(tilesList[j].tile.GetComponent<Tile>()))
                    {
                        set.Add(tilesList[j].tile);
                    }
                }
                choiceGang.Add(set);
            }
            return choiceGang.Count > 0;
        }

        public bool IsCanSelfDrawWin()
        {

        }
        public bool IsPlayerCanWin()
        {

        }

        public bool IsPlayerCanGang()
        {
            if ( tableManager.ActivePlayerId == lastPlayerId)
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
            choicePanel.SetActive(true);
            for (int i = 0; i < canChiTileSet.Count; ++i)
            {
                GameObject set = Instantiate(setPrefab, new Vector3(0, 0, 0), Quaternion.identity, choicePanel.transform);
                set.SetActive(true);
                GameObject a = Instantiate(canChiTileSet[i][0]);
                a.transform.SetParent(set.transform, false);
                a.name = canChiTileSet[i][0].GetComponent<Tile>().TileId;
                GameObject b = Instantiate(canChiTileSet[i][1]);
                b.transform.SetParent(set.transform, false);
                b.name = canChiTileSet[i][1].GetComponent<Tile>().TileId;
            }
        }

        // only when an gang or jia gang 
        public void DecideHowToKong()
        {
            choicePanel.SetActive(true);
            for (int i = 0; i < canChiTileSet.Count; ++i)
            {
                GameObject set = Instantiate(setPrefab, new Vector3(0, 0, 0), Quaternion.identity, choicePanel.transform);
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
            choicePanel.SetActive(false);
            Transform[] children = choicePanel.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child == choicePanel.transform) continue;
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

