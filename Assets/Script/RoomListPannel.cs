using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        private int pageIndex = 1;
        private int pageCount = 0;
        private int roomCount = 0;
        public List<Transform> parent = new List<Transform>();
        public Button btnPrevious;
        public Button btnNext;
        public Text panelText;

        void Start()
        {
            InitGUI();

            // 底下為 debug 用，生成假房間，以後請刪掉
            for(int i = 1 ; i <= 8 ; i++)
            {
                RoomUnit roomUnit = Instantiate(roomUnitPrefab, contentTrans);
                roomUnit.SetInfo(lobbyManager, i.ToString(), 1, 4);
                roomList.Add(roomUnit);
            }

            roomCount = roomList.Count;
            pageCount = (roomCount % 8) == 0 ? roomCount / 8 : (roomCount / 8) + 1;
            BindPage(pageIndex);
            panelText.text = string.Format("{0}/{1}", pageIndex.ToString(), pageCount.ToString());
            // 以上為 debug 用，生成假房間，以後請刪掉
        }

        private void InitGUI()
        {
            btnNext.onClick.AddListener(() => { Next(); });
            btnPrevious.onClick.AddListener(() => { Previous(); });
        }

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

            roomCount = roomList.Count;
            pageCount = (roomCount % 8) == 0 ? roomCount / 8 : (roomCount / 8) + 1;
            BindPage(pageIndex);
            panelText.text = string.Format("{0}/{1}", pageIndex.ToString(), pageCount.ToString());
        }

        public void Next()
        {
            if(pageCount <= 0)         return;
            if(pageIndex >= pageCount) return;

            pageIndex += 1;
            if(pageIndex >= pageCount) pageIndex = pageCount;

            BindPage(pageIndex);

            panelText.text = string.Format("{0}/{1}", pageIndex.ToString(), pageCount.ToString());
        }

        public void Previous()
        {
            if(pageCount <= 0) return;
            if(pageIndex <= 1) return;

            pageIndex -= 1;
            if(pageIndex < 1) pageIndex = 1;

            BindPage(pageIndex);

            panelText.text = string.Format("{0}/{1}", pageIndex.ToString(), pageCount.ToString());
        }

        private void BindPage(int index)
        {
            if(roomList == null || roomCount <= 0) return;
            if(index < 0 || index > roomCount)     return;

            if(pageCount == 1)
            {
                for(int i = 0 ; i < Mathf.Min(8, roomCount) ; i++)
                {
                    // roomList[i].SetActive(true);
                    roomList[i].transform.SetParent(parent[i]);
                    roomList[i].transform.localPosition = new Vector3(0, 0, 0);
                }

                // int canDisplay = 0;

                // for(int i = 8 ; i > 0 ; i--)
                // {
                //     if(canDisplay < 8)
                //     {
                //         transform.GetChild(canDisplay).gameObject.SetActive( true );
                //         transform.GetChild(canDisplay).gameObject.transform.SetParent(parent[canDisplay]);
                //     }
                //     else
                //     {
                //         transform.GetChild(canDisplay).gameObject.SetActive( false );
                //     }

                //     canDisplay += 1;
                // }
            }
            else if(pageCount > 1)
            {
                if(index == pageCount)
                {
                    int canDisplay = 0;

                    for(int i = 8 ; i > 0 ; i--)
                    {
                        if(canDisplay < roomCount - 8 * (index - 1))
                        {
                            transform.GetChild(canDisplay).gameObject.SetActive( true );
                        }
                        else
                        {
                            transform.GetChild(canDisplay).gameObject.SetActive( false );
                        }

                        canDisplay += 1;
                    }
                }
            }
            else
            {
                for(int i = 8 ; i > 0 ; i--)
                {
                    transform.GetChild(8 - i).gameObject.SetActive( true );
                }
            }
        }
    }
}