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
        [SerializeField] private Text roomName = null;
        [SerializeField] private LobbyManager lobbyManager = null;

        public void Start()
        {
            lobbyManager.PanelController.AddPanel(EnumPanel.Lobby, gameObject);
        }

        #region BtnCallBack

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
