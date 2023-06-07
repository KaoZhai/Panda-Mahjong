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
        private const int MaxRoomNum = 100;
        private readonly RoomID roomID = new();
        private GameManager gameManager;
        private SortedSet<string> roomNameSet = new();
        private PreparePanelController preparePanelController = new();

        [SerializeField] private RoomListPanel roomListPanel;
        [SerializeField] private PlayerNetworkData playerNetworkDataPrefab;

        public PreparePanelController PanelController => preparePanelController;

        public static LobbyManager Instance
        {
            get;
            private set;
        }

        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }
        }

        public async void Start()
        {
            gameManager = GameManager.Instance;

            // AddCallbacks to trigger callback function
            gameManager.Runner.AddCallbacks(this);

            // TODO: user can not operate until join session completely
            await JoinLobby(gameManager.Runner);
        }

        #region - RoomRelated

        private async Task JoinLobby(NetworkRunner runner)
        {
            var result = await runner.JoinSessionLobby(SessionLobby.ClientServer);

            if (!result.Ok)
                Debug.LogError($"Fail to start: {result.ShutdownReason}");
        }

        public async Task CreateRoom(string roomName, int maxPlayerNum)
        {
            if (roomNameSet.Count <= MaxRoomNum && !roomNameSet.Contains(roomName))
            {
                // TODO: session add unique id
                var customProps = new Dictionary<string, int>(){
                    {"roomID", roomID.Get(roomName)}
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
                    preparePanelController.OpenPanel(EnumPanel.Waiting);
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
                SessionName = roomName,
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = gameManager.gameObject.AddComponent<NetworkSceneManagerDefault>()
            });


            if (result.Ok)
                preparePanelController.OpenPanel(EnumPanel.Waiting);
            else
                Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }

        #endregion

        private void Disconnect()
        {
        }


        #region - PhontonCallBack

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            roomNameSet.Clear();
            foreach (var session in sessionList)
            {
                roomNameSet.Add(session.Name);
            }
            roomListPanel.UpdateRoomList(sessionList);
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
            }
        }

        #endregion

        #region - unused callbacks
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            if (gameManager.PlayerList.TryGetValue(runner.LocalPlayer, out PlayerNetworkData playerNetworkData))
            {
                runner.Despawn(playerNetworkData.Object);
            }
            Disconnect();
        }
        public void OnConnectedToServer(NetworkRunner runner) { }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            runner.Shutdown(false);
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            if (runner.CurrentScene.buildIndex == 0)
            {
                preparePanelController.OpenPanel(EnumPanel.Waiting);
            }
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        #endregion
    }
}