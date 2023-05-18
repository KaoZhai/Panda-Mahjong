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

    public class Tile : MonoBehaviour, IBeginDragHandler , IDragHandler, IEndDragHandler
    {
        // {tile_type}_{tile_number}_{1..4}
        private string id;
        private Game.Tile.TileType tileType;
        private int tileNumber;
        private int cardFaceIndex;
        private int playerId;
        private Transform tilePool, hand;
        private TableManager tableManager;
        private Transform self;
        private Vector3 oriPosition;
        
        void Start()
        {
            self = GetComponent<Transform>();
        }

        public void OnBeginDrag(PointerEventData eventData) 
        {
            if(IsValidDrag(self.transform.parent, tableManager.GetLocalPlayerId()))
            {
                oriPosition = self.transform.position;
            }
        }

        public void OnDrag(PointerEventData eventData) 
        {
            if(IsValidDrag(self.transform.parent, tableManager.GetLocalPlayerId()))
            {
                self.transform.position = eventData.position;
            }
        }

        public void OnEndDrag(PointerEventData eventData) 
        {
            if(IsValidDrag(self.transform.parent, tableManager.GetLocalPlayerId()))
            {
                if(transform.localPosition.y > 0)
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

        public int GetPlayerId()
        {
            return playerId;
        }

        public void SetPlayerId(int id)
        {
            playerId = id;
        }

        public void SetHand(Transform hand)
        {
            this.hand = hand;
        }

        public void SetTilePool(Transform tilePool)
        {
            this.tilePool = tilePool;
        }

        public void SetTableManager(TableManager tableManager)
        {
            this.tableManager = tableManager;
        }

        public Game.Tile.TileType GetTileType()
        {
            return tileType;
        }

        public void SetTileType(TileType type)
        {
            tileType = type;
        }

        public int GetTileNumber()
        {
            return tileNumber;
        }

        public void SetTileNumber(int number)
        {
            tileNumber = number;
        }

        public int GetCardFaceIndex()
        {
            return cardFaceIndex;
        }

        public void SetCardFaceIndex(int cardFaceIndex)
        {
            this.cardFaceIndex = cardFaceIndex;
        }

        public string GetTileId()
        {
            return id;
        }

        public void SetTileId(string id)
        {
            this.id = id;
        }
    }
}




