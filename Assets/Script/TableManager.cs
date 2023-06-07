using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace Game.Play
{

    public class TableManager : MonoBehaviour
    {
        public static TableManager Instance => null;

        [SerializeField] private List<Player> players = new List<Player>();
        [SerializeField] private TileWall tileWall;

        private int activePlayerIndex;

        private GameObject lastTile = null;

        private GameManager gameManager;

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
            gameManager = GameManager.Instance;
            tileWall.GetReady();
            // todo: player will set player id, now is 0-3
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
                foreach (Player player in players)
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
            for (int round = 0; round < 4; ++round)
            {
                for (int playerIndex = 0; playerIndex < 4; ++playerIndex)
                {
                    for (int i = 0; i < 4; ++i)
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
            if(!tileWall.DealTile(players[activePlayerIndex]))
            {
                EndGame(-1);
            }

            while (players[activePlayerIndex].ReplaceFlower()) { }

            if (activePlayerIndex == 0 && players[0].Kong.CanDoOperation())
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
            InitScoringList();
            if(winningPlayerIndex == -1)
            {
                points.text = "流局";
                foreach(Text obj in pointsChangeText)
                {
                    obj.text = "+0";
                }
                huActive = true;
            }
            else
            {
                players[winningPlayerIndex].CallCalculator();

                points.text = "共 " + players[winningPlayerIndex].TaiCalculator.Tai + " 台";
                int pointsChange = gameManager.GameBasePoint + gameManager.GameTaiPoint * players[winningPlayerIndex].TaiCalculator.Tai;

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

                for (int i = 0; i < scoringList.Count; i++)
                {
                    winningType[i].text = scoringList[i];
                }
            }
        }

        private void InitScoringList()
        {
            foreach(Text obj in winningType)
            {
                obj.text = "";
            }
        }

        public IEnumerator BeforeNextPlayer()
        {
            // only control local player's button
            if (players[0].Chi.CanDoOperation())
            {
                SetButton(chiBtn, true);
            }
            if (players[0].Pong.CanDoOperation())
            {
                SetButton(pongBtn, true);
            }
            if (players[0].Kong.CanDoOperation())
            {
                SetButton(kongBtn, true);
            }
            if (players[0].IsPlayerCanHu(false))
            {
                SetButton(winningBtn, true);
            }
            yield return new WaitForSeconds(2f);

            buttons.ForEach((button) => {SetButton(button, false);});
            
            // todo: need to deal multiplayer move
            if (huActive)
            {

            }
            else if (kongActive)
            {
                TurnToPlayer(0);
                kongActive = false;
                BuKong();
            }
            else if (pongActive)
            {
                TurnToPlayer(0);
                pongActive = false;
            }
            else if (chiActive)
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
    }
}