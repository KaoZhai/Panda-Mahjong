using System.Collections;
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
            return string.Compare(x.GetComponent<Tile>().TileId, y.GetComponent<Tile>().TileId);
        }
    }

    public class TileComparer : IComparer<Tile>
    {
        public int Compare(Tile x, Tile y)
        {
            return string.Compare(x.TileId, y.TileId);
        }
    }

    public class Tile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // {tile_type}_{tile_number}_{1..4}
        private string id;
        private TileType tileType;
        private int tileNumber;
        private int cardFaceIndex;
        private Player player;
        private TableManager tableManager;
        private Transform self;
        private Vector3 oriPosition;

        public void Start()
        {
            self = GetComponent<Transform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsValidDrag(self.transform.parent, tableManager.ActivePlayerIndex))
            {
                oriPosition = self.transform.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (IsValidDrag(self.transform.parent, tableManager.ActivePlayerIndex))
            {
                self.transform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (IsValidDrag(self.transform.parent, tableManager.ActivePlayerIndex))
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
                if(PlayerPrefs.GetFloat("Tile") > 0.0f)
                {
                    gameObject.GetComponent<AudioSource>().Play();
                }
            }
        }
        

        private bool IsValidDrag(Transform parent, int id)
        {
            return parent == player.Hand && id == player.PlayerIndex;
        }

        public int PlayerIndex
        {
            get { return player.PlayerIndex; }
        }

        public TileType TileType
        {
            get { return tileType; }
            set { tileType = value; }
        }

        public int TileNumber
        {
            get { return tileNumber; }
            set { tileNumber = value; }
        }

        public int CardFaceIndex
        {
            get { return cardFaceIndex; }
            set { cardFaceIndex = value; }
        }

        public string TileId
        {
            get { return id; }
        }
        public void SetTileId(int serialNumber) 
        {
            id = tileType.ToString() + "_" + 
                tileNumber.ToString() + "_" +
                serialNumber.ToString();
        }

        public Player Player
        {
            get { return player; }
            set { player = value; }
        } 
        public void SetTableManager(TableManager tableManager)
        {
            this.tableManager = tableManager;
        }
        // Flower Season
        public bool IsFlower()
        {
            return (tileType == TileType.Flower) || (tileType == TileType.Season);
        }

        // Wind Dragon
        public bool IsHonor()
        {
            return (tileType == TileType.Wind) || (tileType == TileType.Dragon);
        }
        // Dot Bamboo Character
        public bool IsSuit()
        {
            return (tileType == TileType.Dot) || (tileType == TileType.Bamboo) || (tileType == TileType.Character);
        }

        public bool IsSame(Tile tile)
        {
            return tile.CardFaceIndex == this.CardFaceIndex;
        }
    }
}




