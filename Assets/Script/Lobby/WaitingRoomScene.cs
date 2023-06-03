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

        public void Start()
        {
            lobbyManager = LobbyManager.Instance;
            lobbyManager.PanelController.AddPanel(EnumPanel.Waiting, gameObject);
            gameManager = GameManager.Instance;
        }

        #region BtnCallBack

        public void OnLeaveBtnClick()
        {
            GameManager.Instance.Disconnect();
            lobbyManager.PanelController.OpenPanel(EnumPanel.RoomList);
        }

        public void OnCreateRoomBtnClick()
        {
            // wait for all client ready and start game
        }

        #endregion
    }
}
