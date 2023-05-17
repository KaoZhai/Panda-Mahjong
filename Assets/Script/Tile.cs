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
        public string id;
        public Game.Tile.TileType tileType;
        public int tileNumber;
        public int cardFaceIndex;
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

        public Transform GetHand()
        {
            return hand;
        }

        public void SetHand(Transform hand)
        {
            this.hand = hand;
        }

        public Transform GetTilePool()
        {
            return tilePool;
        }

        public void SetTilePool(Transform tilePool)
        {
            this.tilePool = tilePool;
        }

        public TableManager GetTableManager()
        {
            return tableManager;
        }

        public void SetTableManager(TableManager tableManager)
        {
            this.tableManager = tableManager;
        }
    }
}




