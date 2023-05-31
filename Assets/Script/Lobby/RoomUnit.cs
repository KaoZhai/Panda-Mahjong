using UnityEngine;

namespace Game.Lobby
{
    public class RoomUnit : MonoBehaviour
    {
        private LobbyManager lobbyManager = null;

        [SerializeField] private int maxPlayerNum = 0;
        [SerializeField] private int curPlayerNum = 0;
        [SerializeField] private string roomName = null;

        public void SetInfo(LobbyManager lobbyManager, string roomName, int curPlayerNum, int maxPlayerNum)
        {
            this.lobbyManager = lobbyManager;
            this.roomName = roomName;
            this.curPlayerNum = curPlayerNum;
            this.maxPlayerNum = maxPlayerNum;
        }

        public async void OnMouseDown()
        {
            await lobbyManager.JoinRoom(roomName);
        }
    }
}