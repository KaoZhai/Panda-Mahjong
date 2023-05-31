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
        // [SerializeField] private LobbyManager lobbyManager = null;
        [SerializeField] private Text roomName = null;
        [SerializeField] private RoomListPannel roomListPannel = null;
        [SerializeField] private RoomCreatingPannel roomCreatingPannel = null;
        [SerializeField] private LobbyManager lobbyManager = null;

        #region BtnCallBack

        public void OnLeaveBtnClick()
        {
            lobbyManager.SetPairState(Game.Lobby.PanelState.Start);
        }

        public void OnCreateRoomBtnClick()
        {
            DisplayRoomCreating(true);
            DisplayRoomList(false);
        }

        public void OnWaitingLeaveBtnClick()
        {
            DisplayRoomCreating(false);
            DisplayRoomList(true);
            GameManager.Instance.Disconnect();
            roomName.text = "";
        }

        #endregion

        #region Helper Functions

        private void DisplayRoomList(bool value)
        {
            roomListPannel.DisplayPannel(value);
        }

        private void DisplayRoomCreating(bool value)
        {
            roomCreatingPannel.DisplayPannel(value);
        }

        #endregion

    }
}
