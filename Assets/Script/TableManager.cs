using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.PlayingRoom {
    
    public class TableManager : MonoBehaviour
    {
        public static TableManager Instance
        {
            get;
            private set;
        }
        [SerializeField] private List<Player> players = new List<Player>();
        [SerializeField] private TileWall tileWall;

        private int activePlayerId = 0;

        private GameObject lastTile = null;
        
        [SerializeField] private GameObject winningBtn, chiBtn, pongBtn, kongBtn;
        

        public TileWall TileWall
        {
            get { return tileWall;}
        }
        public GameObject LastTile
        {
            set { lastTile = value; }
            get { return lastTile; }
        }
        public int ActivePlayerId
        {
            get { return activePlayerId; }
        }


        private bool chiActive = false;
        private bool pongActive = false;
        private bool kongActive = false;
        private bool winningActive = false;

        void Start() 
        {

            tileWall.GetReady(this);
            // todo: player will set player id, now is 0-3
            for(int i = 0; i < 4; i++)
            {
                players[i].PlayerId = i;
                players[i].TableManager = this;
            }

            // DecideDealer();

            DealTiles();
            OpenDoor();
            while()
            {
                
            }
            StartCoroutine(PlayRound());
        }

        IEnumerator PlayRound()
        {

        }

        void OpenDoor()
        {
            tileWall.DealTile(players[activePlayerId]);
            int cnt = 0;
            while(cnt < 4)
            {
                foreach(Player player in players)
                {
                    if (player.IsDoneReplace())
                    {
                        cnt += 1;
                    }
                    else
                    {
                        player.ReplaceFlower();
                    }
                }
            }
            foreach(Player player in players)
            {
                player.SortHandTiles();
            }
            if(activePlayerId == 0 && players[0].IsPlayerCanKongBySelf())
            {
                SetButton(kongBtn, true);
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

        void DecideDealer()
        {

            for(int i = 0; i < players.Count; ++i)
            {
                int j = Random.Range(0, players.Count);
                Player tmp = players[i];
                players[i] = players[j];
                players[j] = tmp;
            }
            activePlayerId = players[0].PlayerId;
        }

        public void Draw()
        {
            tileWall.DealTile(players[activePlayerId]);
            while(!players[activePlayerId].IsDoneReplace())
            {
                players[activePlayerId].ReplaceFlower();
            }

            if(activePlayerId == 0 && players[0].IsPlayerCanKongBySelf())
            {
                SetButton(kongBtn, true);
            }
        }

        public void BuKong()
        {
            
            tileWall.BuPai(players[activePlayerId]);
            while(!players[activePlayerId].IsDoneReplace())
            {
                players[activePlayerId].ReplaceFlower();
            }
            kongActive = false;
            if(activePlayerId == 0 && players[0].IsPlayerCanKongBySelf())
            {
                SetButton(kongBtn, true);
            }
        }

        public void NextPlayer() 
        {
            activePlayerId = (activePlayerId + 1) % 4;
        }

        public void TurnToPlayer(int playerId)
        {
            activePlayerId = playerId;
        }
        //  only from single player
        public void AutoPlay()
        {
            if(activePlayerId != 0)
            {
                
                players[activePlayerId].DefaultDiscard();
            }
        }

        IEnumerator Countdown(int second)
        {
            yield return new WaitForSeconds(1f);
        }
        void SetButton(GameObject btn, bool isInteractable)
        {
            btn.GetComponent<Button>().interactable = isInteractable;
        }

        public void Chi()
        {
            TurnToPlayer(0);
            chiActive = true;
            SetButton(chiBtn, false);
        }
        public void Pong()
        {
            TurnToPlayer(0);
            pongActive = true;
            SetButton(pongBtn, false);
        }
        public void Kong()
        {
            TurnToPlayer(0);
            kongActive = true;
            SetButton(kongBtn, false);
            BuKong();
        }
        public void Win()
        {
            winningActive = true;
            SetButton(winningBtn, false);
        }

        public IEnumerator BeforeNextPlayer()
        {
            SetButton(chiBtn, false);   
            SetButton(pongBtn, false); 
            SetButton(kongBtn, false);
            // only control local player's button
            if(players[0].IsPlayerCanChi())
            {
                SetButton(chiBtn, true);
            }
            if(players[0].IsPlayerCanPong())
            {
                SetButton(pongBtn, true);
            }
            if(players[0].IsPlayerCanKong())
            {
                SetButton(kongBtn, true);
            }
            yield return new WaitForSeconds(2f);
            SetButton(chiBtn, false);   
            SetButton(pongBtn, false); 
            SetButton(kongBtn, false);
            // todo: need to deal multiplayer move
            if ( winningActive )
            {
                
            }
            else if ( kongActive )
            {
                kongActive = false;
            }
            else if ( pongActive )
            {
                pongActive = false;
            }
            else if ( chiActive )
            {
                chiActive = false;
            }
            else
            {
                NextPlayer();
                Draw();
                AutoPlay();
            }
            
        }

    }
}