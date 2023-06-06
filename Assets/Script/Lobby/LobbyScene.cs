using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;
using Game.Core;

namespace Game.Lobby
{
    public class LobbyScene : MonoBehaviour
    {
        private LobbyManager lobbyManager = null;
        [SerializeField] private Text roomName = null;

        public void Start()
        {
            lobbyManager = LobbyManager.Instance;
            lobbyManager.PanelController.AddPanel(EnumPanel.Lobby, gameObject);
        }

        #region - BtnCallBack

        public void OnLeaveBtnClick()
        {
            lobbyManager.PanelController.OpenPanel(EnumPanel.Start);
        }

        public void OnCreateRoomBtnClick()
        {
            lobbyManager.PanelController.OpenPanel(EnumPanel.RoomCreating);
        }

        #endregion
    }
}
