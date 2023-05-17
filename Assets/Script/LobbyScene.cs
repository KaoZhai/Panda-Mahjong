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
        [SerializeField] private LobbyManager lobbyManager = null;
        [SerializeField] private RoomListPannel roomListPannel = null;
        // [SerializeField] private RoomCreatingPannel roomCreatingPannel = null;
        public void OnLeaveBtnClick()
        {
            SceneManager.LoadScene("Start");
        }

        public async void OnCreateRoomBtnClick()
        {
            // SceneManager.LoadScene("CreateRoom");

            // just for test
            await lobbyManager.CreateRoom("Room1", 4);
        }

        private void DisplayRoomList(bool value)
        {
            roomListPannel.DisplayPannel(value);
        }

        private void DisplayRoomCreating(bool value)
        {

        }
    }
}
