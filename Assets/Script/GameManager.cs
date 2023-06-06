using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.Serialization;

namespace Game.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance
        {
            get;
            private set;
        }

        [SerializeField] private NetworkRunner runner;

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

        public int GameBasePoint { get; set; } = 100;

        public int GameTaiPoint { get; set; } = 30;

        public string PlayerName { get; set; } = "User";


        public event Action OnPlayerListUpdated;

        private Guid playerID = Guid.NewGuid();

        [FormerlySerializedAs("playerList")] public Dictionary<PlayerRef, PlayerNetworkData> PlayerList = new Dictionary<PlayerRef, PlayerNetworkData>();

        public Guid PlayerID
        {
            get => playerID;
        }

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