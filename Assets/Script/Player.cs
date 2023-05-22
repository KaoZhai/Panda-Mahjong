using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Player {
    public class Player : MonoBehaviour
    {
        private Vector3 startPosition; // 生成起始位置
        private float spacing = 2.0f; // 麻將間距
        List<GameObject> handTiles = new List<GameObject>();

        List<GameObject> showTiles = new List<GameObject>();
        public Transform hand;
        public Transform tilePool;

        public Transform showPool;

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

        // public void DisplayHandTiles()
        // {
        //     for(int i = 0; i < handTiles.Count; ++i)
        //     {
        //         // handTiles[i].transform.position = startPosition + new Vector3(i * spacing, 0, 0);
                
                
        //     }
        // }
        // public void DisplayShowTiles()
        // {
        //     for(int i = 0; i < showTiles.Count; ++i)
        //     {
        //         showTiles[i].SetActive(true);
                
        //     }
            
        // }

        public void GetTile(GameObject tile)
        {
            handTiles.Add(tile);
            tile.GetComponent<Game.Tile.Tile>().Player = this;
            tile.transform.SetParent(hand);
            tile.SetActive(true);
        }

        public bool IsDoneReplace()
        {
            bool isDone = true;
            for(int i = 0; i < handTiles.Count; ++i)
            {
                if (handTiles[i].GetComponent<Game.Tile.Tile>().IsFlower())
                {
                    isDone = false;
                }
            }
            return isDone;
        }
        
        public void ReplaceFlower(Game.TileWall.TileWall tileWall)
        {
            List<int> flowers = new List<int>();
            for(int i = 0; i < handTiles.Count; ++i)
            {
                if (handTiles[i].GetComponent<Game.Tile.Tile>().IsFlower())
                {
                    flowers.Add(i);
                }
            }
            // remove from back, index won't change
            for(int i = flowers.Count - 1; i >= 0; --i)
            {
                handTiles[flowers[i]].transform.SetParent(showPool);
                showTiles.Add(handTiles[flowers[i]]);
                handTiles.RemoveAt(flowers[i]);
            }

            for(int i = 0; i < flowers.Count; ++i)
            {
                tileWall.BuPai(this);
            }
        }


        public void DefaultDiscard()
        {
            Discord(handTiles[handTiles.Count-1].GetComponent<Game.Tile.Tile>().TileId);
            
        }

        public void Discord(string tileId)
        {
            for(int i = 0; i < handTiles.Count; ++i)
            {
                if (handTiles[i].GetComponent<Game.Tile.Tile>().TileId == tileId)
                {
                    handTiles[i].transform.SetParent(tilePool);
                    handTiles[i].SetActive(true);
                    handTiles.RemoveAt(i);
                    break;
                }
            }
        }

    }

}

