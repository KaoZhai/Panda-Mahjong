using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class DragTile : MonoBehaviour, IBeginDragHandler , IDragHandler, IEndDragHandler
{
    public Transform tilePool, hand;
    private Transform self;
    private Vector3 oriPosition;
    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData) 
    {
        if(self.transform.parent == hand)
        {
            oriPosition = self.transform.position;
        }
    }

    public void OnDrag(PointerEventData eventData) 
    {
        if(self.transform.parent == hand)
        {
            self.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        if(transform.localPosition.y > 0)
        {
            self.transform.parent = tilePool;
        }
        else
        {
            self.transform.position = oriPosition;
        }
    }
}
