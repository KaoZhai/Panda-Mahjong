using UnityEngine;

namespace Game.Lobby
{
    public class StartScene : MonoBehaviour
    {
        private LobbyManager lobbyManager = null;
        [SerializeField] private Game.Music.BgmController musicController = null;

        public void Start()
        {
            lobbyManager = LobbyManager.Instance;
            lobbyManager.PanelController.AddPanel(EnumPanel.Start, gameObject);
            lobbyManager.PanelController.OpenPanel(EnumPanel.Start);
            musicController.PlayMusic(Game.Music.EnumMusic.Start);
        }

        #region BtnCallBack
        public void StartButton()
        {
            lobbyManager.PanelController.OpenPanel(EnumPanel.RoomList);
        }

        public void SettingButton()
        {
            lobbyManager.PanelController.OpenPanel(EnumPanel.StartSetting);
        }

        public void ExitButton()
        {
            Application.Quit();
        }
        #endregion
    }
}
