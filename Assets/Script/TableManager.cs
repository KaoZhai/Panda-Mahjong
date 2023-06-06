using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Core;
using Utils;

namespace Game.Play
{

    public class TableManager : MonoBehaviour
    {
        public static TableManager Instance => null;

        private Dictionary<string, Player> players = new Dictionary<string, Player>();
        private Dictionary<string, PlayerController> playerControllerDict = new Dictionary<string, PlayerController>();
        [SerializeField] private TileWall tileWall;

        private string activePlayerId;

        [SerializeField] private GameObject winningBtn, chiBtn, pongBtn, kongBtn;
        [SerializeField] private GameObject roundPoints;
        [SerializeField] private Text points;
        [SerializeField] private List<Text> pointsChangeText;
        [SerializeField] private List<Text> winningType;
        [SerializeField] private Transform contentTrans;
        [SerializeField] private Player playerPrefab;
        [SerializeField] private RoomManager roomManager;


        public TileWall TileWall => tileWall;

        public GameObject LastTile { set; get; }

        public string ActivePlayerId => activePlayerId;


        private bool chiActive;
        private bool pongActive;
        private bool kongActive;
        private bool huActive;
        private GameManager gameManager;
        private List<string> orderList = new List<string>();


        void Start()
        {
            gameManager = GameManager.Instance;

            gameManager.TableManager = this;

            InitPlayerControllerDict();

            tileWall.GetReady();

            if (gameManager.Runner.IsServer)
            {
                // random pick order
                RandomOrder();

                // create player object and set info
                SpawnAllPlayers();

                // sync other player orderList
                SyncPlayerOrder();

                DealTiles();

                SyncHandTiles();
            }

            OpenDoor();
        }

        public void InitPlayerControllerDict()
        {
            foreach (var playerRef in gameManager.PlayerList.Keys)
            {
                if (roomManager.PlayerList.TryGetValue(playerRef, out PlayerController playerController))
                    playerControllerDict.Add(playerController.PlayerID, playerController);
            }
        }

        #region - host execute

        public void RandomOrder()
        {
            foreach (string playerId in playerControllerDict.Keys)
                orderList.Add(playerId);

            for (int i = 0; i < orderList.Count; ++i)
            {
                int j = Random.Range(0, orderList.Count);
                (orderList[i], orderList[j]) = (orderList[j], orderList[i]);
            }
        }

        public void SpawnAllPlayers()
        {
            foreach (string playerId in playerControllerDict.Keys)
            {
                var playerObj = Instantiate(playerPrefab, contentTrans);
                playerObj.SetInfo(playerId, this);
                players.Add(playerId, playerObj);
            }
        }

        public void SyncPlayerOrder()
        {
            for (int i = 0; i < orderList.Count; ++i)
            {
                if (playerControllerDict.TryGetValue(orderList[i], out PlayerController playerController))
                    playerController.SetPlayerOrder_RPC(i);
            }
        }

        public void DealTiles()
        {
            for (int round = 0; round < 4; ++round)
            {
                foreach (string playerId in orderList)
                {
                    for (int i = 0; i < 4; ++i)
                    {
                        tileWall.DealTile(players[playerId]);
                    }
                }
            }
        }

        public void SyncHandTiles()
        {
            foreach (string playerId in orderList)
            {
                if (playerControllerDict.TryGetValue(playerId, out PlayerController playerController))
                {
                    foreach (var tile in players[playerId].HandTiles)
                    {
                        string tileId = tile.GetComponent<Tile>().TileId;
                        playerController.DealTiles_RPC(tileId);
                    }
                }
            }
        }

        #endregion

        #region - client execute

        public void SetPlayerOrder(string playerId, int order)
        {
            orderList[order] = playerId;
        }

        public void AssignTileToPlayer(string playerId, string tileId)
        {
            tileWall.DealTile(players[playerId], tileId);
        }

        #endregion

        void OpenDoor()
        {
            tileWall.DealTile(players[activePlayerId]);
            int cnt = 0;
            while (cnt < 4)
            {
                foreach (Player player in players.Values)
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
            foreach (Player player in players.Values)
            {
                player.SortHandTiles();
            }
        }

        private void Draw()
        {
            tileWall.DealTile(players[activePlayerId]);
            while (!players[activePlayerId].IsDoneReplace())
            {
                players[activePlayerId].ReplaceFlower();
            }

            if (activePlayerId == orderList[0] && players[activePlayerId].IsPlayerCanKong())
            {
                SetButton(kongBtn, true);
            }

            if (activePlayerId == orderList[0] && players[activePlayerId].IsPlayerCanHu(true))
            {
                SetButton(winningBtn, true);
            }
        }

        private void BuKong()
        {
            tileWall.BuPai(players[activePlayerId]);
            while (!players[activePlayerId].IsDoneReplace())
            {
                players[activePlayerId].ReplaceFlower();
            }
        }

        public string NextPlayer()
        {
            int activePlayerIndex = orderList.FindIndex(id => id == activePlayerId);
            activePlayerIndex = (activePlayerIndex + 1) % 4;
            return orderList[activePlayerIndex];
        }

        private void TurnToPlayer(string playerId)
        {
            activePlayerId = playerId;
        }
        //  only from single player
        private void AutoPlay()
        {
            if (activePlayerId != "")
            {

                players[activePlayerId].DefaultDiscard();
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
            TurnToPlayer(orderList[winningPlayerIndex]);
            EndGame(winningPlayerIndex);
        }

        private void EndGame(int winningPlayerIndex)
        {
            // roundPoints.SetActive(true);
            // players[orderList[winningPlayerIndex]].CallCalculator();

            // points.text = "共 " + players[orderList[winningPlayerIndex]].TaiCalculator.Tai + " 台";

            // int pointsChange = 300 + 100 * players[orderList[winningPlayerIndex]].TaiCalculator.Tai; //還沒接上 gameManager 先用這個
            // // int pointsChange = gameManager.GameBasePoint + gameManager.GameTaiPoint * players[winningPlayerIndex].TaiCalculator.Tai;

            // if (players[orderList[winningPlayerIndex]].IsSelfDraw)
            // {
            //     pointsChangeText[winningPlayerIndex].text = "+" + (pointsChange * 3).ToString();
            //     pointsChangeText[(winningPlayerIndex + 1) % 4].text = "-" + pointsChange.ToString();
            //     pointsChangeText[(winningPlayerIndex + 2) % 4].text = "-" + pointsChange.ToString();
            //     pointsChangeText[(winningPlayerIndex + 3) % 4].text = "-" + pointsChange.ToString();
            // }
            // else
            // {
            //     for (int i = 0; i < 4; i++)
            //     {
            //         if (LastTile.GetComponent<Tile>().PlayerIndex == i)
            //         {
            //             pointsChangeText[i].text = "-" + pointsChange.ToString();
            //         }
            //         else
            //         {
            //             pointsChangeText[i].text = "+0";
            //         }
            //     }
            //     pointsChangeText[winningPlayerIndex].text = "+" + pointsChange.ToString();
            // }

            // List<string> scoringList = players[orderList[winningPlayerIndex]].TaiCalculator.ScoringList;

            // InitScoringList();

            // for (int i = 0; i < scoringList.Count; i++)
            // {
            //     winningType[i].text = scoringList[i];
            // }
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
            // if (players[0].IsPlayerCanChi())
            // {
            //     SetButton(chiBtn, true);
            // }
            // if (players[0].IsPlayerCanPong())
            // {
            //     SetButton(pongBtn, true);
            // }
            // if (players[0].IsPlayerCanKong())
            // {
            //     SetButton(kongBtn, true);
            // }
            // if (players[0].IsPlayerCanHu(false))
            // {
            //     SetButton(winningBtn, true);
            // }
            Debug.Log("開始停頓");
            // StartCoroutine(Countdown(3));
            yield return new WaitForSeconds(2f);
            Debug.Log("停頓結束");
            SetButton(chiBtn, false);
            SetButton(pongBtn, false);
            SetButton(kongBtn, false);
            SetButton(winningBtn, false);
            // todo: need to deal multiplayer move
            if (huActive)
            {

            }
            else if (kongActive)
            {
                TurnToPlayer(orderList[0]);
                kongActive = false;
                BuKong();
            }
            else if (pongActive)
            {
                TurnToPlayer(orderList[0]);
                pongActive = false;
            }
            else if (chiActive)
            {
                TurnToPlayer(orderList[0]);
                chiActive = false;
            }
            else
            {
                TurnToPlayer(NextPlayer());
                Draw();
                AutoPlay();
            }

        }

    }
}