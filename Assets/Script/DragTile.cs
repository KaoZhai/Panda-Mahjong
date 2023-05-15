using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class DragTile : MonoBehaviour, IBeginDragHandler , IDragHandler, IEndDragHandler
{
    public Transform tilePool, hand;
    Transform self;
    Vector3 oriPosition;
    Tile selfTile;
    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Transform>();
        selfTile = GetComponent<Tile>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData) 
    {
        if(self.transform.parent == hand && selfTile.playerId == selfTile.tableManager.GetLocalPlayerId())
        {
            oriPosition = self.transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData) 
    {
        if(self.transform.parent == hand && selfTile.playerId == selfTile.tableManager.GetLocalPlayerId())
        {
            self.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        if(self.transform.parent == hand && selfTile.playerId == selfTile.tableManager.GetLocalPlayerId())
        {
            if(transform.localPosition.y > 0)
            {
                self.transform.SetParent(tilePool);
                selfTile.tableManager.NextPlayer();
            }
            else
            {
                self.transform.position = oriPosition;
            }
        }
    }
}
