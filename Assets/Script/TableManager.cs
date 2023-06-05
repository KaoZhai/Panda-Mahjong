using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Game.Core;

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

        private GameManager gameManager = null;
        
        [SerializeField] private GameObject winningBtn, chiBtn, pongBtn, kongBtn;
        [SerializeField] private GameObject roundPoints;
        [SerializeField] private Text points;
        [SerializeField] private Text underPointsChange;
        [SerializeField] private Text upperPointsChange;
        [SerializeField] private Text rightPointsChange;
        [SerializeField] private Text leftPointsChange;
        [SerializeField] private List<Text> WinningType;
        

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
        private bool huActive = false;


        void Start() 
        {
            gameManager = GameManager.Instance;
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
            huActive = true;
            TurnToPlayer(0);

            roundPoints.SetActive(true);
            players[0].callCalculator();

            points.text = "共 "+players[0].TaiCalculator.Tai+" 台";
            int pointsChange = 300 + 100 * players[0].TaiCalculator.Tai;
            // int pointsChange = gameManager.GameBasePoint + gameManager.GameTaiPoint * players[0].TaiCalculator.Tai;
            if (players[0].IsSelfDraw)
            {
                underPointsChange.text = "+" + (pointsChange * 3).ToString();
                rightPointsChange.text = "-" + pointsChange.ToString();
                upperPointsChange.text = "-" + pointsChange.ToString();
                leftPointsChange.text = "-" + pointsChange.ToString();
            }
            else if (LastTile.GetComponent<Tile>().PlayerIndex == 1)
            {
                underPointsChange.text = "+" + pointsChange.ToString();
                rightPointsChange.text = "-" + pointsChange.ToString();
                upperPointsChange.text = "+0";
                leftPointsChange.text = "+0";
            }
            else if (LastTile.GetComponent<Tile>().PlayerIndex == 2)
            {
                underPointsChange.text = "+" + pointsChange.ToString();
                rightPointsChange.text = "+0";
                upperPointsChange.text = "-" + pointsChange.ToString();
                leftPointsChange.text = "+0";
            }
            else if (LastTile.GetComponent<Tile>().PlayerIndex == 3)
            {
                underPointsChange.text = "+" + pointsChange.ToString();
                rightPointsChange.text = "+0";
                upperPointsChange.text = "+0";
                leftPointsChange.text = "-" + pointsChange.ToString();
            }

            List<string> scoringList = players[0].TaiCalculator.ScoringList;

            InitScoringList();

            for (int i = 0; i < scoringList.Count; i++)
            {
                WinningType[i].text = scoringList[i];
            }
        }

        private void InitScoringList()
        {
            for (int i = 0; i < 18; i++)
            {
                WinningType[i].text = "";
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