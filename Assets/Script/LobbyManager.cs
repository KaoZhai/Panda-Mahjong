using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;
using Fusion.Sockets;
using Game.Core;
using Utils;

namespace Game.Lobby
{
    public class LobbyManager : MonoBehaviour, INetworkRunnerCallbacks
    {
        private const int maxRoomNum = 100;
        private RoomID roomID = new RoomID();
        private GameManager gameManager = null;
        private SortedSet<string> roomNameSet = new SortedSet<string>();

        [SerializeField] private RoomListPannel roomListPannel = null;
        [SerializeField] private PlayerNetworkData playerNetworkDataPrefab = null;
        public async void Start()
        {
            gameManager = GameManager.Instance;

            // AddCallbacks to trigger callback function
            gameManager.Runner.AddCallbacks(this);

            // TODO: user can not operate until join session completely
            await JoinLobby(gameManager.Runner);
        }

        public async Task JoinLobby(NetworkRunner runner)
        {
            var result = await runner.JoinSessionLobby(SessionLobby.ClientServer);

            if (!result.Ok)
                Debug.LogError($"Fail to start: {result.ShutdownReason}");
        }

        public async Task CreateRoom(string roomName, int maxPlayerNum)
        {
            if (roomNameSet.Count <= maxRoomNum && !roomNameSet.Contains(roomName))
            {
                // TODO: session add unique id
                var customProps = new Dictionary<string, int>(){
                    {"roomID", (int)roomID.Get(roomName)}
                };

                // TODO: add fusion object pool
                var result = await gameManager.Runner.StartGame(new StartGameArgs()
                {
                    GameMode = GameMode.Host,
                    SessionName = roomName,
                    PlayerCount = maxPlayerNum,
                    Scene = SceneManager.GetActiveScene().buildIndex,
                    SceneManager = gameManager.gameObject.AddComponent<NetworkSceneManagerDefault>()
                });

                if (result.Ok)
                    SceneManager.LoadScene("WaitingRoom");
                else
                    Debug.LogError($"Failed to Start: {result.ShutdownReason}");
            }
            // TODO: display error message
        }

        public async Task JoinRoom(string roomName)
        {
            var result = await gameManager.Runner.StartGame(new StartGameArgs()
            {
                GameMode = GameMode.Client,
                SessionName = roomName
            });


            if (result.Ok)
                SceneManager.LoadScene("WaitingRoom");
            else
                Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            roomNameSet.Clear();
            foreach (var session in sessionList)
            {
                roomNameSet.Add(session.Name);
            }
            roomListPannel.UpdateRoomList(sessionList);
        }

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            runner.Spawn(playerNetworkDataPrefab, Vector3.zero, Quaternion.identity, player);
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (gameManager.PlayerList.TryGetValue(player, out PlayerNetworkData playerNetworkData))
            {
                runner.Despawn(playerNetworkData.Object);

                gameManager.PlayerList.Remove(player);
                gameManager.UpdatePlayerList();
            }
        }

        private async void Disconnect()
        {
            await JoinLobby(gameManager.Runner);
        }

        #region - unused callbacks
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnConnectedToServer(NetworkRunner runner) { }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Disconnect();
        }
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