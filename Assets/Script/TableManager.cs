using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.TableManager {
    
    public class TableManager : MonoBehaviour
    {

        public List<Game.Player.Player> players = new List<Game.Player.Player>();
        public Game.TileWall.TileWall tileWall;
        
        int localPlayerId = 1;
        void Start() 
        {

            tileWall.GetReady(this);
            for(int i = 1; i <= 4; i++)
            {
                players[i-1].PlayerId = i;
            }

            PickSeatsAndDecideDealer();
            DealTiles();
            foreach(Player.Player player in players)
            {
                if (player.PlayerId == 1)
                    player.DisplayHandTiles();
            }
        }


        void DealTiles()
        {
            for(int round = 0; round < 4; ++round)
            {
                for(int playerId = 0; playerId < 4; ++playerId)
                {
                    for(int i = 0; i < 4; ++i)
                    {
                        tileWall.DealTile(players[playerId]);
                    }
                }
            }
        }

        void PickSeatsAndDecideDealer()
        {

            for(int i = 0; i < players.Count; ++i)
            {
                int j = Random.Range(0, players.Count);
                Game.Player.Player tmp = players[i];
                players[i] = players[j];
                players[j] = tmp;
            }
        }

        

        
        // todo: just public for demo, it should be private
        public void GetTileFromTileWall(int playerId) 
        {
            // todo: just for demo, should not change the playerId
            playerId = localPlayerId;
            tileWall.DealTile(players[playerId]);
            
        }

        public void NextPlayer()
        {
            localPlayerId = (localPlayerId + 1) % 4;
            // Debug.Log("after: " + localPlayerId);
        }

        public int GetLocalPlayerId()
        {
            return localPlayerId;
        }
    }
}