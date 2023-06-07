using System;
using System.Collections;
using System.Collections.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using Game.Core;

namespace Game.Play
{
    public class RoomManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        private GameManager gameManager = null;
        private NetworkRunner networkRunner = null;
        private Dictionary<PlayerRef, PlayerController> playerList = new Dictionary<PlayerRef, PlayerController>();

        [SerializeField] private PlayerController playerControllerPrefab = null;

        public PlayerController LocalPlayerController
        {
            get => playerList[networkRunner.LocalPlayer];
        }

        public void Start()
        {
            gameManager = GameManager.Instance;

            networkRunner = gameManager.Runner;

            // AddCallbacks to trigger callback function
            gameManager.Runner.AddCallbacks(this);

            SpawnAllPlayers();
        }

        #region - use for network

        public void SpawnAllPlayers()
        {
            foreach (var player in gameManager.PlayerList.Keys)
                SpawnPlayer(networkRunner, player);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            SpawnPlayer(runner, player);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (playerList.TryGetValue(player, out PlayerController playerController))
            {
                playerList.Remove(player);
                runner.Despawn(playerController);
            }
        }

        private void SpawnPlayer(NetworkRunner runner, PlayerRef player)
        {
            var playerController = gameManager.Runner.Spawn(
                playerControllerPrefab,
                Vector3.zero,
                Quaternion.identity,
                player
            );

            networkRunner.SetPlayerObject(playerController);

            playerList.Add(player, playerController);
        }

        #endregion

        #region - unused callbacks

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnDisconnectedFromServer(NetworkRunner runner) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

        #endregion
    }
}
