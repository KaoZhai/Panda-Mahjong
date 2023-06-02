using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Core;

namespace Game.Lobby
{
    public class WaitingRoomScene : MonoBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager = null;

        public void Start()
        {
            lobbyManager.PanelController.AddPanel(EnumPanel.Waiting, gameObject);
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
