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

        [SerializeField] private GameObject winningBtn, chiBtn, pongBtn, kongBtn;
        [SerializeField] private GameObject roundPoints;
        [SerializeField] private Text points;
        [SerializeField] private List<Text> pointsChangeText;
        [SerializeField] private List<Text> winningType;

        private List<GameObject> buttons;

        public TileWall TileWall => tileWall;

        public GameObject LastTile { set; get; }

        public int ActivePlayerIndex => activePlayerIndex;


        private bool chiActive;
        private bool pongActive;
        private bool kongActive;
        private bool huActive;


        void Start() 
        {
            tileWall.GetReady();
            // TODO: player will set player id, now is 0-3
            for(int i = 0; i < 4; i++)
            {
                players[i].PlayerIndex = i;
                players[i].TableManager = this;
            }

            buttons = new List<GameObject>() { winningBtn, chiBtn, kongBtn, pongBtn };

            // PickSeatsAndDecideDealer();

            DealTiles();
            OpenDoor();
        }

        #region - GameLogic

        void OpenDoor()
        {
            tileWall.DealTile(players[activePlayerIndex]);
            int cnt = 0;
            
            while(cnt < 4)
            {
                foreach(Player player in players)
                {
                    cnt += player.ReplaceFlower() ? 0 : 1;
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

        private void Draw()
        {
            tileWall.DealTile(players[activePlayerIndex]);
            while (players[activePlayerIndex].ReplaceFlower()) { }

            if(activePlayerIndex == 0 && players[0].IsPlayerCanKong())
            {
                SetButton(kongBtn, true);
            }

            if (activePlayerIndex == 0 && players[0].IsPlayerCanHu(true))
            {
                SetButton(winningBtn, true);
            }
        }

        private void BuKong()
        {
            tileWall.Replenish(players[activePlayerIndex]);
            while(players[activePlayerIndex].ReplaceFlower()) { }
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
            // StartCoroutine(Countdown(3));
            yield return new WaitForSeconds(2f);
            foreach (var button in buttons)
            {
                SetButton(button, false);
            }
            // TODO: need to deal multiplayer move
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

        #endregion

        #region - BtnCallBack

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
            // TODO: need to do multi player judge
            int winningPlayerIndex = 0;
            TurnToPlayer(winningPlayerIndex);
            EndGame(winningPlayerIndex);
        }

        #endregion

        #region - HelperFunction
        
        IEnumerator Countdown(int second)
        {
            yield return new WaitForSeconds(second);
        }
        
        private void NextPlayer() 
        {
            activePlayerIndex = (activePlayerIndex + 1) % 4;
        }

        private void TurnToPlayer(int playerIndex)
        {
            activePlayerIndex = playerIndex;
        }
        //  only from single player

        
        private void AutoPlay()
        {
            if(activePlayerIndex != 0)
            {
                players[activePlayerIndex].DefaultDiscard();
            }
        }

        private void SetButton(GameObject btn, bool isInteractable)
        {
            btn.GetComponent<Button>().interactable = isInteractable;
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
                pointsChangeText[winningPlayerIndex].text = "+" + (pointsChange * 3);
                pointsChangeText[(winningPlayerIndex + 1) % 4].text = "-" + pointsChange;
                pointsChangeText[(winningPlayerIndex + 2) % 4].text = "-" + pointsChange;
                pointsChangeText[(winningPlayerIndex + 3) % 4].text = "-" + pointsChange;
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (LastTile.GetComponent<Tile>().PlayerIndex == i)
                    {
                        pointsChangeText[i].text = "-" + pointsChange;
                    }
                    else
                    {
                        pointsChangeText[i].text = "+0";
                    }
                }
                pointsChangeText[winningPlayerIndex].text = "+" + pointsChange;
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

        #endregion
    }
}