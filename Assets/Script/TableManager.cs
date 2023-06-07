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

        private List<GameObject> buttons;


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

            buttons = new List<GameObject>() { winningBtn, chiBtn, kongBtn, pongBtn };

            if (gameManager.Runner.IsServer)
            {
                // random pick order
                RandomOrder();

                // create player object and set info
                SpawnAllPlayers();

                // sync other player orderList
                SyncPlayerOrder();

                DealTiles();

                OpenDoor();

                SyncHandTiles();
            }

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
                    cnt += player.ReplaceFlower() ? 0 : 1;
                }
            }
            foreach (Player player in players.Values)
            {
                player.SortHandTiles();
            }
        }

        private void Draw()
        {
            tileWall.DealTile(players[ActivePlayerId]);
            while (players[ActivePlayerId].ReplaceFlower()) { }

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
            tileWall.Replenish(players[activePlayerId]);
            while (players[activePlayerId].ReplaceFlower()) { }
        }

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
            string winningPlayerId = gameManager.PlayerID;
            TurnToPlayer(winningPlayerId);
            EndGame(winningPlayerIndex, winningPlayerId);
        }

        #endregion

        #region - HelperFunction

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

        private void SetButton(GameObject btn, bool isInteractable)
        {
            btn.GetComponent<Button>().interactable = isInteractable;
        }
        private void EndGame(int winningPlayerIndex, string winningPlayerId)
        {
            roundPoints.SetActive(true);
            players[winningPlayerId].CallCalculator();

            points.text = "共 " + players[winningPlayerId].TaiCalculator.Tai + " 台";

            int pointsChange = 300 + 100 * players[winningPlayerId].TaiCalculator.Tai; //還沒接上 gameManager 先用這個
            // int pointsChange = gameManager.GameBasePoint + gameManager.GameTaiPoint * players[winningPlayerIndex].TaiCalculator.Tai;

            if (players[winningPlayerId].IsSelfDraw)
            {
                pointsChangeText[winningPlayerIndex].text = "+" + (pointsChange * 3).ToString();
                pointsChangeText[(winningPlayerIndex + 1) % 4].text = "-" + pointsChange.ToString();
                pointsChangeText[(winningPlayerIndex + 2) % 4].text = "-" + pointsChange.ToString();
                pointsChangeText[(winningPlayerIndex + 3) % 4].text = "-" + pointsChange.ToString();
            }
            else
            {
                for (int i = 0; i < orderList.Count; ++i)
                {
                    if (LastTile.GetComponent<Tile>().PlayerId == orderList[i])
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

            List<string> scoringList = players[winningPlayerId].TaiCalculator.ScoringList;

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
            string localPlayerId = gameManager.PlayerID;
            if (players[localPlayerId].IsPlayerCanChi())
            {
                SetButton(chiBtn, true);
            }
            if (players[localPlayerId].IsPlayerCanPong())
            {
                SetButton(pongBtn, true);
            }
            if (players[localPlayerId].IsPlayerCanKong())
            {
                SetButton(kongBtn, true);
            }
            if (players[localPlayerId].IsPlayerCanHu(false))
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

        #endregion
    }
}