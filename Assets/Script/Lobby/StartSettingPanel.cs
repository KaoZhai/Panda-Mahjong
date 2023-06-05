using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Lobby
{
    public class StartSettingPanel : MonoBehaviour
    {
        private LobbyManager lobbyManager = null;
        // Start is called before the first frame update
        public void Start()
        {
            lobbyManager = LobbyManager.Instance;
            lobbyManager.PanelController.AddPanel(EnumPanel.StartSetting, gameObject);
            // Debug.Log("StartSetting");
        }

        #region BtnCallBack

        public void OnLeaveBtnClick()
        {
            lobbyManager.PanelController.OpenPanel(EnumPanel.Start);
        }

        #endregion
    }
}