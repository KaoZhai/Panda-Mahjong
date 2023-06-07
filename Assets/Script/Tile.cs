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

    public class Tile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // {tile_type}_{tile_number}_{1..4}
        private string id;
        private Transform self;
        private Vector3 oriPosition;
        
        public int PlayerIndex => Player.PlayerIndex;
        public TileType TileType { get; private set; }
        public int TileNumber { get; private set; }
        public int CardFaceIndex { get; private set; }
        public Player Player { get; set; }
        public string TileId => id;

        void Start()
        {
            self = GetComponent<Transform>();
        }

        public void Init(TileType tileType, int tileNumber, int serialNumber, int cardFaceIndex)
        {
            TileType = tileType;
            TileNumber = tileNumber;
            CardFaceIndex = cardFaceIndex;
            id = TileType + "_" + TileNumber + "_" + serialNumber;
        }
        
        public bool Equals(Tile tile)
        {
            return tile.CardFaceIndex == CardFaceIndex;
        }

        #region - Drag Process

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsValidDrag(self.transform.parent))
            {
                oriPosition = self.transform.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (IsValidDrag(self.transform.parent))
            {
                self.transform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (IsValidDrag(self.transform.parent))
            {
                if (transform.localPosition.y > 0)
                {
                    if(!Player.Discard(id))
                    {
                        self.transform.position = oriPosition;
                    }
                }
                else
                {
                    self.transform.position = oriPosition;
                }

                Debug.Log("Tile" + PlayerPrefs.GetFloat("Tile"));
                if(PlayerPrefs.GetFloat("Tile") > 0.0f)
                {
                    gameObject.GetComponent<AudioSource>().Play();
                }
            }
        }
        
        private bool IsValidDrag(Transform parent)
        {
            return parent == Player.Hand;
        }

        #endregion

        #region - Judge

        // Flower Season
        public bool IsFlower()
        {
            return TileType is TileType.Flower or TileType.Season;
        }

        // Wind Dragon
        public bool IsHonor()
        {
            return TileType is TileType.Wind or TileType.Dragon;
        }
        // Dot Bamboo Character
        public bool IsSuit()
        {
            return TileType is TileType.Dot or TileType.Bamboo or TileType.Character;
        }

        #endregion

    }
}




