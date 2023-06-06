using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Play
{
    public enum TileType
    {
        Flower,
        Season,
        Wind,
        Dragon,
        Character,
        Bamboo,
        Dot
    }

    public class TileGameObjectComparer : IComparer<GameObject>
    {
        public int Compare(GameObject x, GameObject y)
        {
            return String.CompareOrdinal(x?.GetComponent<Tile>().TileId, y?.GetComponent<Tile>().TileId);
        }
    }

    public class TileComparer : IComparer<Tile>
    {
        public int Compare(Tile x, Tile y)
        {
            return String.CompareOrdinal(x?.TileId, y?.TileId);
        }
    }

    public class Tile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // {tile_type}_{tile_number}_{1..4}
        private string id;
        private Player player;
        private TableManager tableManager;
        private Transform self;
        private Vector3 oriPosition;

        void Start()
        {
            self = GetComponent<Transform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsValidDrag(self.transform.parent, tableManager.ActivePlayerId))
            {
                oriPosition = self.transform.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (IsValidDrag(self.transform.parent, tableManager.ActivePlayerId))
            {
                self.transform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (IsValidDrag(self.transform.parent, tableManager.ActivePlayerId))
            {
                if (transform.localPosition.y > 0)
                {
                    player.Discard(id);
                }
                else
                {
                    self.transform.position = oriPosition;
                }

                Debug.Log("Tile" + PlayerPrefs.GetFloat("Tile"));
                if (PlayerPrefs.GetFloat("Tile") > 0.0f)
                {
                    gameObject.GetComponent<AudioSource>().Play();
                }
            }
        }


        private bool IsValidDrag(Transform parent, string curPlayerId)
        {
            return parent == player.Hand && curPlayerId == player.PlayerId;
        }

        public TileType TileType { get; set; }

        public int TileNumber { get; set; }

        public int CardFaceIndex { get; set; }

        public string TileId => id;

        public string PlayerId => player.PlayerId;

        public void SetTileId(int serialNumber)
        {
            id = TileType.ToString() + "_" +
                TileNumber.ToString() + "_" +
                serialNumber.ToString();
        }

        public Player Player
        {
            get => player;
            set => player = value;
        }
        public void SetTableManager(TableManager currentTableManager)
        {
            this.tableManager = currentTableManager;
        }
        // Flower Season
        public bool IsFlower()
        {
            return (TileType == TileType.Flower) || (TileType == TileType.Season);
        }

        // Wind Dragon
        public bool IsHonor()
        {
            return (TileType == TileType.Wind) || (TileType == TileType.Dragon);
        }
        // Dot Bamboo Character
        public bool IsSuit()
        {
            return (TileType == TileType.Dot) || (TileType == TileType.Bamboo) || (TileType == TileType.Character);
        }

        public bool IsSame(Tile tile)
        {
            return tile.CardFaceIndex == this.CardFaceIndex;
        }
    }
}




