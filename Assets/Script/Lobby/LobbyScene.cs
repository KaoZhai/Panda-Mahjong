using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fusion;

namespace Game.Lobby
{
    public class LobbyScene : MonoBehaviour
    {
        // [SerializeField] private LobbyManager lobbyManager = null;
        [SerializeField] private RoomListPannel roomListPannel = null;
        [SerializeField] private RoomCreatingPannel roomCreatingPannel = null;
        [SerializeField] private Game.Lobby.LobbyManager lobbyManager = null;

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
