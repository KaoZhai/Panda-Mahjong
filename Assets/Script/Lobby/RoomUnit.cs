using UnityEngine;
using TMPro;

namespace Game.Lobby
{
    public class RoomUnit : MonoBehaviour
    {
        private LobbyManager lobbyManager;

        [SerializeField] private TextMeshProUGUI roomName;
        [SerializeField] private TextMeshProUGUI curPlayerNum;
        [SerializeField] private TextMeshProUGUI maxPlayerNum;

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