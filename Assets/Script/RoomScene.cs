using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;
using Game.Core;

namespace Game.Room
{
    public class RoomScene : MonoBehaviour
    {
        private string roomName = null;
        private GameManager gameManager = null;

        [SerializeField] private CanvasGroup canvasGroup = null;
        [SerializeField] private SettingPanel settingPanel = null;
        // [SerializeField] private PlayerListPannel playerListPannel = null;
        [SerializeField] private GameObject readyBtn = null;
        [SerializeField] private GameObject startBtn = null;

        public void Start()
        {
            gameManager = GameManager.Instance;

            if (gameManager.IsRoomCreater)
            {
                settingPanel.DisplayPannel(true);
                startBtn.SetActive(true);
                readyBtn.SetActive(false);
            }
            else
            {
                settingPanel.DisplayPannel(false);
                startBtn.SetActive(false);
                readyBtn.SetActive(true);
            }
        }

        public void DisplayPannel(bool value)
        {
            canvasGroup.alpha = value ? 1 : 0;
            canvasGroup.interactable = value;
            canvasGroup.blocksRaycasts = value;

            roomName = gameManager.Runner.SessionInfo.Name;
        }

        public void OnReadyBtnClicked()
        {
            var runner = gameManager.Runner;

            if (gameManager.PlayerList.TryGetValue(runner.LocalPlayer, out PlayerNetworkData playerNetworkData))
            {
                playerNetworkData.SetReady_RPC(true);
            }

            //#TODO 更改當前玩家的資訊
        }

        public void OnStartBtnClicked()
        {
            //#TODO 開始遊戲
        }
    }
}