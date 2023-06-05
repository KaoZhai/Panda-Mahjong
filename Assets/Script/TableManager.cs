using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Play {
    
    public class TableManager : MonoBehaviour
    {
        public static TableManager Instance
        {
            get;
            private set;
        }
        [SerializeField] private List<Player> players = new List<Player>();
        [SerializeField] private TileWall tileWall;

        private int activePlayerIndex = 0;

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
        public int ActivePlayerIndex
        {
            get { return activePlayerIndex; }
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
                players[i].PlayerIndex = i;
                players[i].TableManager = this;
            }

            // PickSeatsAndDecideDealer();

            DealTiles();
            OpenDoor();
        }




        void OpenDoor()
        {
            tileWall.DealTile(players[activePlayerIndex]);
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
        }

        void DealTiles()
        {
            for(int round = 0; round < 4; ++round)
            {
                for(int playerIndex = 0; playerIndex < 4; ++playerIndex)
                {
                    for(int i = 0; i < 4; ++i)
                    {
                        tileWall.DealTile(players[playerIndex]);
                    }
                }
            }
        }

        void PickSeatsAndDecideDealer()
        {

            for(int i = 0; i < players.Count; ++i)
            {
                int j = Random.Range(0, players.Count);
                Player tmp = players[i];
                players[i] = players[j];
                players[j] = tmp;
            }
            activePlayerIndex = 0;
        }

        public void Draw()
        {
            tileWall.DealTile(players[activePlayerIndex]);
            while(!players[activePlayerIndex].IsDoneReplace())
            {
                players[activePlayerIndex].ReplaceFlower();
            }

            if(activePlayerIndex == 0 && players[0].IsPlayerCanKong())
            {
                SetButton(kongBtn, true);
            }

            if (activePlayerIndex == 0 && players[0].IsPlayerCanHu(true))
            {
                SetButton(winningBtn, true);
            }
        }

        public void BuKong()
        {
            tileWall.BuPai(players[activePlayerIndex]);
            while(!players[activePlayerIndex].IsDoneReplace())
            {
                players[activePlayerIndex].ReplaceFlower();
            }
        }

        public void NextPlayer() 
        {
            activePlayerIndex = (activePlayerIndex + 1) % 4;
        }

        public void TurnToPlayer(int playerIndex)
        {
            activePlayerIndex = playerIndex;
        }
        //  only from single player
        public void AutoPlay()
        {
            if(activePlayerIndex != 0)
            {
                
                players[activePlayerIndex].DefaultDiscard();
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
            chiActive = true;
        }
        public void Pong()
        {
            pongActive = true;
        }
        public void Kong()
        {
            kongActive = true;
        }
        public void Win()
        {
            winningActive = true;
            Debug.Log("Winning");
        }

        public IEnumerator BeforeNextPlayer()
        {
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
            if(players[0].IsPlayerCanHu(false))
            {
                SetButton(winningBtn, true);
            }
            Debug.Log("開始停頓");
            // StartCoroutine(Countdown(3));
            yield return new WaitForSeconds(2f);
            Debug.Log("停頓結束");
            SetButton(chiBtn, false);   
            SetButton(pongBtn, false); 
            SetButton(kongBtn, false);
            SetButton(winningBtn, false);
            // todo: need to deal multiplayer move
            if ( winningActive )
            {
                TurnToPlayer(0);
                winningActive = false;
            }
            else if ( kongActive )
            {
                TurnToPlayer(0);
                kongActive = false;
                BuKong();
            }
            else if ( pongActive )
            {
                TurnToPlayer(0);
                pongActive = false;
            }
            else if ( chiActive )
            {
                TurnToPlayer(0);
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