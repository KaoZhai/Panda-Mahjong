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

        private int activePlayerId = 0;

        public int ActivePlayerId
        {
            get { return activePlayerId; }
        }

        void Start() 
        {

            tileWall.GetReady(this);
            // todo: player will set player id, now is 0-3
            for(int i = 0; i < 4; i++)
            {
                players[i].PlayerId = i;
            }

            // PickSeatsAndDecideDealer();

            DealTiles();
            OpenDoor();

        }




        void OpenDoor()
        {
            tileWall.DealTile(players[activePlayerId]);
            int cnt = 0;
            while(cnt < 4)
            {
                foreach(Player.Player player in players)
                {
                    if (player.IsDoneReplace())
                    {
                        cnt += 1;
                    }
                    else
                    {
                        player.ReplaceFlower(tileWall);
                    }
                }
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
            activePlayerId = players[0].PlayerId;
        }

        public void Draw(Player.Player player)
        {
            tileWall.DealTile(players[activePlayerId]);
            while(!player.IsDoneReplace())
            {
                player.ReplaceFlower(tileWall);
            }
        }
        public void NextPlayer() 
        {
            activePlayerId = (activePlayerId + 1) % 4;
            Draw(players[activePlayerId]);
            //  only from single player
            if(activePlayerId != 0)
            {
                players[activePlayerId].DefaultDiscard();
                NextPlayer();
            }
        }


        public void BeforeNextPlayer()
        {
            // todo: check pong
            // if (IsPlayerCanPong())
            // {

            // }
            
            // todo: check chi
            
            NextPlayer();

        }
    }
}