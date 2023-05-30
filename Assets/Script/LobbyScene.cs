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
        
        
        public void OnLeaveBtnClick()
        {
            SceneManager.LoadScene("Start");
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
