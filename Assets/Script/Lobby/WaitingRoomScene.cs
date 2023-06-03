using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Core;

namespace Game.Lobby
{
    public class WaitingRoomScene : MonoBehaviour
    {
        private LobbyManager lobbyManager = null;
        private GameManager gameManager = null;

        [SerializeField] private GameObject startBtn = null;

        public void Start()
        {
            lobbyManager = LobbyManager.Instance;
            lobbyManager.PanelController.AddPanel(EnumPanel.Waiting, gameObject);
            gameManager = GameManager.Instance;
        }

        public void Update()
        {
            if (GameManager.Instance.Runner.GameMode == Fusion.GameMode.Host)
            {
                startBtn.SetActive(true);
            }
            else
            {
                startBtn.SetActive(false);
            }
        }

        #region BtnCallBack

        public void OnLeaveBtnClick()
        {
            GameManager.Instance.Disconnect();
            lobbyManager.PanelController.OpenPanel(EnumPanel.RoomList);
        }

        public void OnStartBtnClick()
        {
            GameManager.Instance.UpdatePlayerList();
        }

        public void OnReadyBtnClick()
        {
            var runner = gameManager.Runner;
            if (gameManager.PlayerList.TryGetValue(runner.LocalPlayer, out PlayerNetworkData playerNetworkData))
            {
                playerNetworkData.SetReady_RPC(true);
            }
        }

        #endregion
    }
}
