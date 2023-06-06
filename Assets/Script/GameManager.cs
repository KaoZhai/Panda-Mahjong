using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Game.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get;
            private set;
        }

        [SerializeField] private NetworkRunner runner = null;

        public NetworkRunner Runner
        {
            get
            {
                if (runner == null)
                {
                    runner = gameObject.AddComponent<NetworkRunner>();
                    runner.ProvideInput = true;
                }
                return runner;
            }
        }

        public int gameBasePoint = 100;

        public int gameTaiPoint = 30;

        public int GameBasePoint
        {
            get { return gameBasePoint; }
            set { gameBasePoint = value; }
        }

        public int GameTaiPoint
        {
            get { return gameTaiPoint; }
            set { gameTaiPoint = value; }
        }

        public string PlayerName { get; set; } = "User";


        public event Action OnPlayerListUpdated = null;

        public Dictionary<PlayerRef, PlayerNetworkData> PlayerList = new Dictionary<PlayerRef, PlayerNetworkData>();

        public void Disconnect()
        {
            Runner.Shutdown(false);
        }

        public void Awake()
        {
            Runner.ProvideInput = true;

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        private bool CheckAllPlayerIsReady()
        {
            if (!Runner.IsServer) return false;

            foreach (var playerData in PlayerList.Values)
            {
                if (!playerData.IsReady) return false;
            }

            foreach (var playerData in PlayerList.Values)
            {
                playerData.IsReady = false;
            }

            return true;
        }

        public void UpdatePlayerList()
        {
            OnPlayerListUpdated?.Invoke();

            if (CheckAllPlayerIsReady())
            {
                Runner.SetActiveScene("GamePlay");
            }
        }
    }
}