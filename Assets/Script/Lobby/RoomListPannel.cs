using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

namespace Game.Lobby
{
    public class RoomListPanel : MonoBehaviour
    {
        [SerializeField] private Text panelText;
        [SerializeField] private Transform hideTrans;
        [SerializeField] private RoomUnit roomUnitPrefab;
        [SerializeField] private List<Transform> parentPositions = new List<Transform>();
        private int pageIndex = 1;
        private int pageCount;
        private int roomCount;
        private LobbyManager lobbyManager;
        private readonly List<RoomUnit> roomList = new List<RoomUnit>();

        public void Start()
        {
            lobbyManager = LobbyManager.Instance;
            lobbyManager.PanelController.AddPanel(EnumPanel.RoomList, gameObject);
        }

        public void UpdateRoomList(List<SessionInfo> sessionList)
        {
            foreach (Transform parentPos in parentPositions)
            {
                if (parentPos.childCount > 0)
                    Destroy(parentPos.GetChild(0).gameObject);
            }

            foreach (Transform child in hideTrans)
            {
                Destroy(child.gameObject);
            }

            roomList.Clear();
            foreach (var session in sessionList)
            {
                RoomUnit roomUnit = Instantiate(roomUnitPrefab, hideTrans);
                roomUnit.GetComponent<RoomUnit>().SetInfo(lobbyManager, session.Name, session.PlayerCount, session.MaxPlayers);
                roomList.Add(roomUnit);
            }

            roomCount = roomList.Count;
            pageCount = (roomCount % 8) == 0 ? roomCount / 8 : (roomCount / 8) + 1;
            BindPage(pageIndex);
            panelText.text = $"{pageIndex.ToString()}/{pageCount.ToString()}";

            Debug.Log($"room count: {roomCount}");
        }

        public void Next()
        {
            pageIndex += 1;
            UpdatePage();
        }

        public void Previous()
        {
            pageIndex -= 1;
            UpdatePage();
        }

        private void UpdatePage()
        {
            pageIndex = Mathf.Max(1, pageIndex);
            pageIndex = Mathf.Min(pageIndex, pageCount);

            BindPage(pageIndex);
            panelText.text = $"{pageIndex.ToString()}/{pageCount.ToString()}";
        }

        private void BindPage(int index)
        {
            if (roomList == null || roomCount <= 0) return;
            if (index < 0 || index > roomCount) return;

            for (int i = 0; i < roomCount; i++)
            {
                roomList[i].transform.SetParent(hideTrans);
            }


            for (int i = 0, j = 8 * (index - 1); j < Mathf.Min(8 * index, roomCount); i++, j++)
            {
                roomList[j].transform.SetParent(parentPositions[i]);
                roomList[j].transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }
}