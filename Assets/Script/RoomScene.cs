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
        [SerializeField] private RoomSettingPanel settingPanel = null;
        // [SerializeField] private PlayerListPannel playerListPannel = null;

        public void Start()
        {
            gameManager = GameManager.Instance;

            if (gameManager.IsRoomCreater)
            {
                settingPanel.DisplayPannel(true);
            }
            else
            {
                settingPanel.DisplayPannel(false);
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
        }

        public void OnLeaveBtnClicked()
        {
            var runner = gameManager.Runner;

            if (gameManager.PlayerList.TryGetValue(runner.LocalPlayer, out PlayerNetworkData playerNetworkData))
            {
                if (gameManager.Runner.GameMode == GameMode.Host)
                {
                    foreach (var player in gameManager.PlayerList.Keys)
                    {
                        gameManager.Runner.Disconnect(player);
                    }
                } else
                {
                    gameManager.Runner.Disconnect(runner.LocalPlayer);
                }
            }
        }
    }
}