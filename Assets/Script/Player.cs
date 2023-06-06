using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Play
{
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

        private int kongCnt;
        private int ponCnt;
        private int straightCnt;

        private bool isSelfDraw;
        private readonly bool isDealer = true;
        private bool isOnly;

        public TableManager TableManager { get; set; }

        public string PlayerId { set; get; } = "";

        public List<GameObject> HandTiles { get => handTiles; }

        public Transform Hand => hand;

        public Transform TilePool => tilePool;

        public Transform ShowPool => showPool;

        public TaiCalculator TaiCalculator => taiCalculator;

        public int Point { get; set; }

        public bool IsSelfDraw => isSelfDraw;

        public void SetInfo(string playerId, TableManager tableManager)
        {
            PlayerId = playerId;
            TableManager = tableManager;
        }

        public void SortHandTiles()
        {
            handTiles.Sort(new TileGameObjectComparer());
            foreach (GameObject t in handTiles)
            {
                t.transform.SetAsLastSibling();
            }
        }

        private int HandTilesCnt => handTiles.Count;

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
            foreach (var t in handTiles)
            {
                if (t.GetComponent<Tile>().IsFlower())
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
            TableManager.LastTile.transform.SetParent(showPool, false);
            showTiles.Add(TableManager.LastTile);
        }
        public void ReplaceFlower()
        {
            int cnt = 0;
            for (int i = handTiles.Count - 1; i >= 0; --i)
            {
                if (handTiles[i].GetComponent<Tile>().IsFlower())
                {
                    ShowTile(i);
                    ++cnt;
                }
            }

            for (int i = 0; i < cnt; ++i)
            {
                TableManager.TileWall.BuPai(this);
            }
        }


        public void DefaultDiscard()
        {
            Discard(handTiles[Random.Range(0, HandTilesCnt)].GetComponent<Tile>().TileId);


            // Discard(handTiles[handTiles.Count-1].GetComponent<Tile>().TileId);
        }

        public void Discard(string tileId)
        {
            for (int i = 0; i < handTiles.Count; ++i)
            {
                if (handTiles[i].GetComponent<Tile>().TileId == tileId)
                {
                    TableManager.LastTile = handTiles[i];
                    handTiles[i].transform.SetParent(tilePool, false);
                    handTiles[i].SetActive(true);
                    handTiles.RemoveAt(i);
                    break;
                }
            }
            SortHandTiles();
            StartCoroutine(TableManager.BeforeNextPlayer());
        }

        public bool IsPlayerCanKong()
        {
            if ((TableManager.NextPlayer() == PlayerId) || TableManager.LastTile.GetComponent<Tile>().PlayerId == PlayerId)
            {
                return false;
            }
            bool isCanGang = false;
            int numSame = 0;
            foreach (GameObject tile in handTiles)
            {
                if (TableManager.LastTile && tile.GetComponent<Tile>().IsSame(TableManager.LastTile.GetComponent<Tile>()))
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
            if (TableManager.ActivePlayerId == PlayerId)
                return false;
            bool isCanPong = false;
            int numSame = 0;
            foreach (GameObject tile in handTiles)
            {
                if (TableManager.LastTile && tile.GetComponent<Tile>().IsSame(TableManager.LastTile.GetComponent<Tile>()))
                    ++numSame;
            }
            if (numSame >= 2)
            {
                isCanPong = true;

            }
            return isCanPong;
        }

        public bool IsPlayerCanHu(bool selfDraw)
        {
            int[] tileCountArray = new int[50];
            TransToArray(tileCountArray, handTiles);

            isSelfDraw = selfDraw;

            if (!isSelfDraw)
            {
                this.isOnly = CheckIsOnly((int[])tileCountArray.Clone());
                GameObject[] lastTileArray = { TableManager.LastTile };
                List<GameObject> lastTile = new List<GameObject>(lastTileArray);
                TransToArray(tileCountArray, lastTile);
            }

            return CheckHu((int[])tileCountArray.Clone(), false);
        }

        private bool CheckIsOnly(int[] nowHandArray)
        {
            int cnt = 0;
            for (int i = 1; i <= 37; i++)
            {
                if (i == 10 || i == 20 || i == 30)
                    continue;

                nowHandArray[i]++;
                if (CheckHu((int[])nowHandArray.Clone(), false))
                {
                    cnt++;
                }
                nowHandArray[i]--;
            }

            return cnt == 1;
        }

        // recursion
        private static bool CheckHu(int[] nowTileArray, bool havePair)
        {
            if (nowTileArray.Sum() == 0)
            {
                return true;
            }

            for (int i = 1; i <= 37; i++)
            {
                if (nowTileArray[i] == 0)
                    continue;

                // have pair
                if (havePair)
                {
                    // check straight
                    if (nowTileArray[i] > 0 && nowTileArray[i - 1] > 0 && nowTileArray[i + 1] > 0)
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
                    if (nowTileArray[i] >= 3)
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
                    if (nowTileArray[i] >= 2)
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

        private readonly List<List<GameObject>> canChiTileSet = new();
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
        public bool IsPlayerCanChi()
        {
            if (TableManager.ActivePlayerId == PlayerId)
                return false;
            if (TableManager.NextPlayer() != PlayerId)
                return false;
            canChiTileSet.Clear();
            Tile newTile = TableManager.LastTile.GetComponent<Tile>();
            // Debug.Log(newTile.TileType.ToString() + newTile.TileNumber.ToString());
            bool isCanChi = false;
            if (!newTile.IsSuit())
                return false;

            int i = newTile.TileNumber;
            // i-2, i-1, i
            isCanChi |= Find(newTile.TileType, i - 2, i - 1);
            // i-1, i, i+1
            isCanChi |= Find(newTile.TileType, i - 1, i + 1);
            // i, i+1, i+2
            isCanChi |= Find(newTile.TileType, i + 1, i + 2);

            return isCanChi;
        }

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

        void CloseChiSelection()
        {
            chiTileSets.SetActive(false);
            Transform[] children = chiTileSets.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child == chiTileSets.transform) continue;
                Destroy(child.gameObject);
            }
        }
        public void Chi()
        {
            GameObject set = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            Transform[] children = set.GetComponentsInChildren<Transform>();

            bool first = true;
            foreach (Transform child in children)
            {
                if (child == set.transform) continue;
                string tileId = child.name;
                for (int i = handTiles.Count - 1; i >= 0; --i)
                {
                    if (handTiles[i].GetComponent<Tile>().TileId == tileId)
                    {
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

            CloseChiSelection();
            straightCnt++;
        }
        public void Pong()
        {
            TakeTileFromOther();
            int cnt = 0;
            for (int i = handTiles.Count - 1; i >= 0; --i)
            {
                if (handTiles[i].GetComponent<Tile>().IsSame(TableManager.LastTile.GetComponent<Tile>()))
                {
                    ++cnt;
                    ShowTile(i);
                }
                if (cnt == 2)
                    break;
            }
            ponCnt++;
        }

        public void Kong()
        {
            TakeTileFromOther();
            int cnt = 0;
            for (int i = handTiles.Count - 1; i >= 0; --i)
            {
                if (handTiles[i].GetComponent<Tile>().IsSame(TableManager.LastTile.GetComponent<Tile>()))
                {
                    ++cnt;
                    ShowTile(i);
                }
                if (cnt == 3)
                    break;
            }
            kongCnt++;
        }

        public void CallCalculator()
        {
            if (!isSelfDraw)
            {
                showTiles.Add(TableManager.LastTile);
            }

            this.taiCalculator = new TaiCalculator(handTiles, showTiles, kongCnt, ponCnt, straightCnt, 1, 1, 0, 0,
            isDealer, false, false, isSelfDraw, false, isOnly);
        }

    }

}

