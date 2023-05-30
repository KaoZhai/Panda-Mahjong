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
        [SerializeField] private RoomListPannel roomListPannel = null;
        [SerializeField] private RoomCreatingPannel roomCreatingPannel = null;
        
        
        public void OnLeaveBtnClick()
        {
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
        }

        private void DisplayRoomList(bool value)
        {
            roomListPannel.DisplayPannel(value);
        }

        private void DisplayRoomCreating(bool value)
        {
            roomCreatingPannel.DisplayPannel(value);
        }
    }
}
