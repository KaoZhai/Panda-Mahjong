using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

namespace Game.Lobby
{
    public class RoomListPannel : MonoBehaviour
    {
        [SerializeField] private Transform contentTrans = null;
        [SerializeField] private CanvasGroup canvasGroup = null;
        [SerializeField] private RoomUnit roomUnitPrefab = null;
        [SerializeField] private LobbyManager lobbyManager = null;
        private List<RoomUnit> roomList = new List<RoomUnit>();

        public void DisplayPannel(bool value)
        {
            canvasGroup.alpha = value ? 1 : 0;
            canvasGroup.interactable = value;
            canvasGroup.blocksRaycasts = value;
        }

        public void UpdateRoomList(List<SessionInfo> sessionList)
        {
            foreach (Transform child in contentTrans)
            {
                Destroy(child.gameObject);
            }

            roomList.Clear();
            foreach (var session in sessionList)
            {
                RoomUnit roomUnit = Instantiate(roomUnitPrefab, contentTrans);
                roomUnit.SetInfo(lobbyManager, session.Name, session.PlayerCount, session.MaxPlayers);
                roomList.Add(roomUnit);
            }
        }
    }
}
