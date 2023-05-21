using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player {
    public class Player : MonoBehaviour
    {
        private Vector3 startPosition; // 生成起始位置
        private float spacing = 2.0f; // 麻將間距
        List<GameObject> handTiles = new List<GameObject>();
        public Transform hand;
        public Transform tilePool;

        private int playerId = -1;
        private int point;

        public int PlayerId {
            set { playerId = value; }
            get { return playerId; }
        }

        public int Point {
            get { return point; }
            set { point = value; }
        }

        public void DisplayHandTiles()
        {
            for(int i = 0; i < handTiles.Count; ++i)
            {
                handTiles[i].transform.position = startPosition + new Vector3(i * spacing, 0, 0);
                handTiles[i].transform.SetParent(hand);
                handTiles[i].SetActive(true);
            }
        }

        public void GetTile(GameObject tile)
        {
            Debug.Log(handTiles);
            handTiles.Add(tile);
            tile.GetComponent<Game.Tile.Tile>().SetTilePlayer(this);
        }

    }

}

