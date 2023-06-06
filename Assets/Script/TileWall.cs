using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Play {
    public class TileWall : MonoBehaviour
    {
        private Vector3 startPosition;
        [SerializeField] private Transform transformTileWall;
        [SerializeField] private GameObject mahjongPrefab; // 麻將Prefab
        private readonly List<GameObject> tileList = new();

        public void GetReady()
        {
            GenerateAllTile();
            Shuffle();
        }
        void SetTileFace(GameObject mahjong)
        {
            int cardFaceIndex = mahjong.GetComponent<Tile>().CardFaceIndex;
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

        void GenerateTile(TileType tileType, int tileNumber, int cardFaceIndex, int serialNumber)
        {
            GameObject tileObj = Instantiate(mahjongPrefab, startPosition + new Vector3(0, 0, 0), Quaternion.identity, transformTileWall);
            Tile tile =  tileObj.GetComponent<Tile>();
            if(tile==null)
            {
                Debug.LogError("沒有 tile_script");
            }

            // tile info
            tile.TileType = tileType;
            tile.TileNumber = tileNumber;
            tile.CardFaceIndex = cardFaceIndex;
            tile.SetTileId(serialNumber);

            tileList.Add(tileObj);
            tileObj.name = tile.TileId;
            SetTileFace(tileObj);
        }

        void GenerateAllTile()
        {
            int cardFaceIndex = 1;
            // Season       1-4
            for (int i = 1; i <= 4; ++i)
            {
                GenerateTile(TileType.Season, i, cardFaceIndex, 1);
                ++cardFaceIndex;
            }
            // Flower       5-8
            for (int i = 1; i <= 4; ++i)
            {
                GenerateTile(TileType.Flower, i, cardFaceIndex, 1);
                ++cardFaceIndex;
            }
            // Wind         9-12 
            for (int i = 1; i <= 4; ++i)
            {
                for (int j = 1; j <= 4; ++j)
                {
                    GenerateTile(TileType.Wind, i, cardFaceIndex, j);
                }
                ++cardFaceIndex; 
            }
            // Dragon       13-15
            for (int i = 1; i <= 3; ++i)
            {
                for (int j = 1; j <= 4; ++j)
                {
                    GenerateTile(TileType.Dragon, i, cardFaceIndex, j);
                }
                ++cardFaceIndex; 
            }
            // Character    16-24
            for (int i = 1; i <= 9; ++i)
            {
                for (int j = 1; j <= 4; ++j)
                {
                    GenerateTile(TileType.Character, i, cardFaceIndex, j);
                }
                ++cardFaceIndex; 
            }
            // Bamboo       25-33
            for (int i = 1; i <= 9; ++i)
            {
                for (int j = 1; j <= 4; ++j)
                {
                    GenerateTile(TileType.Bamboo, i, cardFaceIndex, j);
                }
                ++cardFaceIndex; 
            }
            // Dot          34-42
            for (int i = 1; i <= 9; ++i)
            {
                for (int j = 1; j <= 4; ++j)
                {
                    GenerateTile(TileType.Dot, i, cardFaceIndex, j);
                }
                ++cardFaceIndex; 
            }
        }

        public void DealTile(Player player)
        {
            if ( tileList.Count > 0)
            {
                GameObject tile = tileList[0];
                player.GetTile(tile);
                tileList.RemoveAt(0);
            }
            else
            {
                Debug.LogError("牌牆已空");
            }
        }

        public void BuPai(Player player)
        {
            if ( tileList.Count > 0)
            {
                GameObject tile = tileList[^1];
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
                (tileList[i], tileList[j]) = (tileList[j], tileList[i]);
            }
        }

    }


        
}
