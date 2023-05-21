using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Tile
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

    public class Tile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // {tile_type}_{tile_number}_{1..4}
        private string id;
        private Game.Tile.TileType tileType;
        private int tileNumber;
        private int cardFaceIndex;
        private int playerId = -1;
        public Transform tilePool, hand;
        private Game.TableManager.TableManager tableManager;
        private Transform self;
        private Vector3 oriPosition;

        void Start()
        {
            self = GetComponent<Transform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsValidDrag(self.transform.parent, tableManager.GetLocalPlayerId()))
            {
                oriPosition = self.transform.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (IsValidDrag(self.transform.parent, tableManager.GetLocalPlayerId()))
            {
                self.transform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (IsValidDrag(self.transform.parent, tableManager.GetLocalPlayerId()))
            {
                if (transform.localPosition.y > 0)
                {
                    self.transform.SetParent(tilePool);
                    tableManager.NextPlayer();
                }
                else
                {
                    self.transform.position = oriPosition;
                }
            }
        }
        

        private bool IsValidDrag(Transform parent, int id)
        {
            return parent == hand && id == playerId;
        }

        public int PlayerId
        {
            get { return playerId; }
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

        public void SetTilePlayer(Game.Player.Player player)
        {
            this.playerId = PlayerId;
            tilePool = player.tilePool;
            hand = player.hand;
        }
        public void SetHand(Transform hand)
        {
            this.hand = hand;
        }

        public void SetTilePool(Transform tilePool)
        {
            this.tilePool = tilePool;
        }

        public void SetTableManager(Game.TableManager.TableManager tableManager)
        {
            this.tableManager = tableManager;
        }
    }
}




