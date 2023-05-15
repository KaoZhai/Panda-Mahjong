using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public TileType tile_type;
    public int tile_number;
    public int cardFace_index;
    public int playerId;
    public Transform tilePool, hand;
    public TableManager tableManager;
    private Transform self;
    private Vector3 oriPosition;
    
    void Start()
    {
        self = GetComponent<Transform>();
    }

    public void OnBeginDrag(PointerEventData eventData) 
    {
        if(self.transform.parent == hand && playerId == tableManager.GetLocalPlayerId())
        {
            oriPosition = self.transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData) 
    {
        if(self.transform.parent == hand && playerId == tableManager.GetLocalPlayerId())
        {
            self.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        if(self.transform.parent == hand && playerId == tableManager.GetLocalPlayerId())
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
}
