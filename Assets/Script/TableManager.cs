using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Play {
    
    public class TableManager : MonoBehaviour
    {
        public static TableManager Instance => null;

        [SerializeField] private List<Player> players = new List<Player>();
        [SerializeField] private TileWall tileWall;

        private int activePlayerIndex;

        private GameObject lastTile;

        [SerializeField] private GameObject winningBtn, chiBtn, pongBtn, kongBtn;
        [SerializeField] private GameObject roundPoints;
        [SerializeField] private Text points;
        [SerializeField] private List<Text> pointsChangeText;
        [SerializeField] private List<Text> winningType;
        

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


        private bool chiActive;
        private bool pongActive;
        private bool kongActive;
        private bool huActive;


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
                (players[i], players[j]) = (players[j], players[i]);
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
            yield return new WaitForSeconds(second);
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
            huActive = true;
            int winningPlayerIndex = 0;
            TurnToPlayer(winningPlayerIndex);
            EndGame(winningPlayerIndex);
        }

        private void EndGame(int winningPlayerIndex)
        {
            roundPoints.SetActive(true);
            players[winningPlayerIndex].CallCalculator();

            points.text = "共 "+players[winningPlayerIndex].TaiCalculator.Tai+" 台";

            int pointsChange = 300 + 100 * players[winningPlayerIndex].TaiCalculator.Tai; //還沒接上 gameManager 先用這個
            // int pointsChange = gameManager.GameBasePoint + gameManager.GameTaiPoint * players[winningPlayerIndex].TaiCalculator.Tai;

            if (players[winningPlayerIndex].IsSelfDraw)
            {
                pointsChangeText[winningPlayerIndex].text = "+" + (pointsChange * 3).ToString();
                pointsChangeText[(winningPlayerIndex + 1) % 4].text = "-" + pointsChange.ToString();
                pointsChangeText[(winningPlayerIndex + 2) % 4].text = "-" + pointsChange.ToString();
                pointsChangeText[(winningPlayerIndex + 3) % 4].text = "-" + pointsChange.ToString();
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (LastTile.GetComponent<Tile>().PlayerIndex == i)
                    {
                        pointsChangeText[i].text = "-" + pointsChange.ToString();
                    }
                    else
                    {
                        pointsChangeText[i].text = "+0";
                    }
                }
                pointsChangeText[winningPlayerIndex].text = "+" + pointsChange.ToString();
            }

            List<string> scoringList = players[winningPlayerIndex].TaiCalculator.ScoringList;

            InitScoringList();

            for (int i = 0; i < scoringList.Count; i++)
            {
                winningType[i].text = scoringList[i];
            }
        }

        private void InitScoringList()
        {
            for (int i = 0; i < 18; i++)
            {
                winningType[i].text = "";
            }
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
            if ( huActive )
            {

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