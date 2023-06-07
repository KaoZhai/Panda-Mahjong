using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Play {
    internal class TileCategory
    {
        public TileType Type { get; private set; }
        public int MaxTileNumber { get; private set; }
        public int MaxSerialNumber { get; private set; }

        public TileCategory(TileType type, int maxTileNumber, int maxSerialNumber)
        {
            Type = type;
            MaxTileNumber = maxTileNumber;
            MaxSerialNumber = maxSerialNumber;
        }
    }

    public class TileWall : MonoBehaviour
    {
        [SerializeField] private Transform transformTileWall;
        [SerializeField] private GameObject mahjongPrefab; // 麻將Prefab
        [SerializeField] private Text tileRemain;
        private Vector3 startPosition;
        private readonly List<GameObject> tileList = new();
        
        private static readonly List<TileCategory> TileCategoriesList = new List<TileCategory>()
        {
            // Season       1-4
            new(TileType.Season, 4, 1),
            // Flower       5-8
            new(TileType.Flower, 4, 1),
            // Wind         9-12
            new(TileType.Wind, 4, 4),
            // Dragon       13-15
            new(TileType.Dragon, 3, 4),
            // Character    16-24
            new(TileType.Character, 9, 4),
            // Bamboo       25-33
            new(TileType.Bamboo, 9, 4),
            // Dot          34-42
            new(TileType.Dot, 9, 4)
        };

        public void GetReady()
        {
            GenerateAllTile();
            Shuffle();
        }

        public bool DealTile(Player player)
        {
            if(tileList.Count <= 16)
            {
                return false;
            }

            RemoveTile(player, 0);
            return true;
        }

        public void Replenish(Player player)
        {
            RemoveTile(player, tileList.Count - 1);
        }

        #region - HelperFunctions

        private void RemoveTile(Player player, int idx)
        {
            GameObject tile = tileList[idx];
            player.GetTile(tile);
            tileList.RemoveAt(idx);
            tileRemain.text = tileList.Count.ToString();
        }

        private void SetTileFace(GameObject mahjong)
        {
            int cardFaceIndex = mahjong.GetComponent<Tile>().CardFaceIndex;
            var faceImage = mahjong.transform.Find("Face").gameObject.GetComponent<Image>();
            
            var img = Resources.Load<Sprite>("Image/Mahjong/" + cardFaceIndex);
            if (img) 
            {
                faceImage.sprite = img;
            }
            else
            {
                Debug.Log("無法設定圖像" + "Image/Mahjong/" + cardFaceIndex);
            }
        }
        private void GenerateTile(TileType tileType, int tileNumber, int cardFaceIndex, int serialNumber)
        {
            var tileObj = Instantiate(mahjongPrefab, startPosition + new Vector3(0, 0, 0), Quaternion.identity, transformTileWall);
            var tile =  tileObj.GetComponent<Tile>();
            if(tile!=null)
            {
                tile.Init(tileType, tileNumber, serialNumber, cardFaceIndex);
                tileObj.name = tile.TileId; 
            }
            else
            {
                Debug.LogError("沒有 tile_script");
            }

            tileList.Add(tileObj);
            SetTileFace(tileObj);
        }

        private void GenerateAllTile()
        {
            int cardFaceIndex = 1;

            foreach (var tileCategory in TileCategoriesList)
            {
                for (int tileNumber = 0; tileNumber < tileCategory.MaxTileNumber; tileNumber++)
                {
                    for (int serialNumber = 0; serialNumber < tileCategory.MaxSerialNumber; serialNumber++)
                    {
                        GenerateTile(tileCategory.Type, tileNumber, cardFaceIndex, serialNumber);   
                    }
                    cardFaceIndex++;
                }
            }
        }
        private void Shuffle()
        {
            for(int i = 0; i < tileList.Count; ++i)
            {
                int j = Random.Range(0, tileList.Count);
                (tileList[i], tileList[j]) = (tileList[j], tileList[i]);
            }
        }

        #endregion
    }
}
