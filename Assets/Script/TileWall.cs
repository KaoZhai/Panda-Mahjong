using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.TileWall {
    public class TileWall : MonoBehaviour
    {
        private Vector3 startPosition;
        public Transform transformTileWall;
        public GameObject mahjongPrefab; // 麻將Prefab
        private List<GameObject> tileList = new List<GameObject>();

        private Game.TableManager.TableManager tableManager;

        public Game.TableManager.TableManager TableManager {
            set { tableManager = value; }
        }

        public void GetReady(Game.TableManager.TableManager tableManager)
        {
            Debug.Log(tableManager);
            this.tableManager = tableManager;
            GenerateAllTile();
            Shuffle();
        }
        void SetTileFace(GameObject mahjong)
        {
            int cardFaceIndex = mahjong.GetComponent<Game.Tile.Tile>().CardFaceIndex;
            GameObject face = mahjong.transform.Find("Face").gameObject;
            Image faceImage = face.GetComponent<Image>();
            Sprite img = Resources.Load<Sprite>("Image/Mahjong/" + cardFaceIndex.ToString());
            if (img) 
            {
                faceImage.sprite = img;
            }
            else
            {
                Debug.Log("無法設定圖像" + "Image/Mahjong/" + cardFaceIndex.ToString());
            }
            
        }

        void GenerateTile(Game.Tile.TileType tileType, int tileNumber, int cardFaceIndex, int serialNumber)
        {
            GameObject tile_obj = Instantiate(mahjongPrefab, startPosition + new Vector3(0, 0, 0), Quaternion.identity, transformTileWall);
            Game.Tile.Tile tile =  tile_obj.GetComponent<Game.Tile.Tile>();
            if(tile==null)
            {
                Debug.LogError("沒有 tile_script");
            }
            tile.SetTableManager(tableManager);

            // tile info
            tile.TileType = tileType;
            tile.TileNumber = tileNumber;
            tile.CardFaceIndex = cardFaceIndex;
            tile.SetTileId(serialNumber);

            tileList.Add(tile_obj);
            tile_obj.name = tile.TileId;
            SetTileFace(tile_obj);
        }

        void GenerateAllTile()
        {
            int cardFaceIndex = 1;
            // Season       1-4
            for (int i = 1; i <= 4; ++i)
            {
                GenerateTile(Game.Tile.TileType.Season, i, cardFaceIndex, 1);
                ++cardFaceIndex;
            }
            // Flower       5-8
            for (int i = 1; i <= 4; ++i)
            {
                GenerateTile(Game.Tile.TileType.Flower, i, cardFaceIndex, 1);
                ++cardFaceIndex;
            }
            // Wind         9-12 
            for (int i = 1; i <= 4; ++i)
            {
                for (int j = 1; j <= 4; ++j)
                {
                    GenerateTile(Game.Tile.TileType.Wind, i, cardFaceIndex, j);
                }
                ++cardFaceIndex; 
            }
            // Dragon       13-15
            for (int i = 1; i <= 3; ++i)
            {
                for (int j = 1; j <= 4; ++j)
                {
                    GenerateTile(Game.Tile.TileType.Dragon, i, cardFaceIndex, j);
                }
                ++cardFaceIndex; 
            }
            // Character    16-24
            for (int i = 1; i <= 9; ++i)
            {
                for (int j = 1; j <= 4; ++j)
                {
                    GenerateTile(Game.Tile.TileType.Character, i, cardFaceIndex, j);
                }
                ++cardFaceIndex; 
            }
            // Bamboo       25-33
            for (int i = 1; i <= 9; ++i)
            {
                for (int j = 1; j <= 4; ++j)
                {
                    GenerateTile(Game.Tile.TileType.Bamboo, i, cardFaceIndex, j);
                }
                ++cardFaceIndex; 
            }
            // Dot          34-42
            for (int i = 1; i <= 9; ++i)
            {
                for (int j = 1; j <= 4; ++j)
                {
                    GenerateTile(Game.Tile.TileType.Dot, i, cardFaceIndex, j);
                }
                ++cardFaceIndex; 
            }
        }

        public void DealTile(Game.Player.Player player)
        {
            GameObject tile = null;
            if ( tileList.Count > 0)
            {
                tile = tileList[0];
                player.GetTile(tile);
                tileList.RemoveAt(0);
            }
            else
            {
                Debug.LogError("牌牆已空");
            }
        }

        public void BuPai(Game.Player.Player player)
        {
            GameObject tile = null;
            if ( tileList.Count > 0)
            {
                tile = tileList[tileList.Count-1];
                player.GetTile(tile);
                tileList.RemoveAt(tileList.Count-1);
            }
            else
            {
                Debug.LogError("牌牆已空");
            }
        }


        void Shuffle()
        {
            for(int i = 0; i < tileList.Count; ++i)
            {
                int j = Random.Range(0, tileList.Count);
                GameObject tmp = tileList[i];
                tileList[i] = tileList[j];
                tileList[j] = tmp;
            }
        }

    }


        
}
