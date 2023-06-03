using UnityEngine;

namespace Game.Lobby
{
    public class StartScene : MonoBehaviour
    {
        private LobbyManager lobbyManager = null;

        public void Start()
        {
            lobbyManager = LobbyManager.Instance;
            lobbyManager.PanelController.AddPanel(EnumPanel.Start, gameObject);
            lobbyManager.PanelController.OpenPanel(EnumPanel.Start);
        }

        #region BtnCallBack
        public void StartButton()
        {
            lobbyManager.PanelController.OpenPanel(EnumPanel.RoomList);
        }

        public void SettingButton()
        {
            // Setting Panel
        }

        public void ExitButton()
        {
            Application.Quit();
        }
        #endregion
    }
}
