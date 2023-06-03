using UnityEngine;
using TMPro;

namespace Game.Lobby
{
    public class RoomUnit : MonoBehaviour
    {
        private LobbyManager lobbyManager = null;

        [SerializeField] private TextMeshProUGUI roomName = null;
        [SerializeField] private TextMeshProUGUI curPlayerNum = null;
        [SerializeField] private TextMeshProUGUI maxPlayerNum = null;

        public void SetInfo(LobbyManager lobbyManager, string roomName, int curPlayerNum, int maxPlayerNum)
        {
            this.lobbyManager = lobbyManager;
            this.roomName.text = roomName;
            this.curPlayerNum.text = curPlayerNum.ToString();
            this.maxPlayerNum.text = maxPlayerNum.ToString();
        }

        public async void OnMouseDown()
        {
            await lobbyManager.JoinRoom(roomName.text);
        }
    }
}