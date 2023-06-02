using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

namespace Game.Lobby
{
    public class RoomListPannel : MonoBehaviour
    {
        [SerializeField] private Text panelText;
        [SerializeField] private Transform hideTrans = null;
        [SerializeField] private RoomUnit roomUnitPrefab = null;
        [SerializeField] private List<Transform> parentPostitions = new List<Transform>();
        private int pageIndex = 1;
        private int pageCount = 0;
        private int roomCount = 0;
        private LobbyManager lobbyManager = null;
        private List<RoomUnit> roomList = new List<RoomUnit>();

        public void Start()
        {
            lobbyManager = LobbyManager.Instance;
            lobbyManager.PanelController.AddPanel(EnumPanel.RoomList, gameObject);
        }

        public void UpdateRoomList(List<SessionInfo> sessionList)
        {
            foreach (Transform child in hideTrans)
            {
                Destroy(child.gameObject);
            }

            roomList.Clear();
            foreach (var session in sessionList)
            {
                RoomUnit roomUnit = Instantiate(roomUnitPrefab, hideTrans);
                roomUnit.SetInfo(lobbyManager, session.Name, session.PlayerCount, session.MaxPlayers);
                roomList.Add(roomUnit);
            }

            roomCount = roomList.Count;
            pageCount = (roomCount % 8) == 0 ? roomCount / 8 : (roomCount / 8) + 1;
            BindPage(pageIndex);
            panelText.text = string.Format("{0}/{1}", pageIndex.ToString(), pageCount.ToString());

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
            panelText.text = string.Format("{0}/{1}", pageIndex.ToString(), pageCount.ToString());
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
                roomList[j].transform.SetParent(parentPostitions[i]);
                roomList[j].transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }
}