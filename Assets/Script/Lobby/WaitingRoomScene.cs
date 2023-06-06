using UnityEngine;
using Game.Core;

namespace Game.Lobby
{
    public class WaitingRoomScene : MonoBehaviour
    {
        private LobbyManager lobbyManager;
        private GameManager gameManager;

        [SerializeField] private GameObject startBtn;
        [SerializeField] private Music.BgmController musicController;

        public void Start()
        {
            lobbyManager = LobbyManager.Instance;
            lobbyManager.PanelController.AddPanel(EnumPanel.Waiting, gameObject);
            gameManager = GameManager.Instance;
            On100BaseBtnClick();
        }

        public void OnEnable()
        {
            startBtn.SetActive(GameManager.Instance.Runner.GameMode == Fusion.GameMode.Host);
        }

        #region - BtnCallBack

        public void OnLeaveBtnClick()
        {
            GameManager.Instance.Disconnect();
            musicController.PlayMusic(Music.EnumMusic.Start);
            lobbyManager.PanelController.OpenPanel(EnumPanel.RoomList);
        }

        public void OnStartBtnClick()
        {
            GameManager.Instance.UpdatePlayerList();
        }

        public void OnReadyBtnClick()
        {
            var runner = gameManager.Runner;
            if (gameManager.PlayerList.TryGetValue(runner.LocalPlayer, out PlayerNetworkData playerNetworkData))
            {
                playerNetworkData.SetReady_RPC(true);
            }
        }

        public void On100BaseBtnClick()
        {
            gameManager.GameBasePoint = 100;
            gameManager.GameTaiPoint = 30;
        }

        public void On300BaseBtnClick()
        {
            gameManager.GameBasePoint = 300;
            gameManager.GameTaiPoint = 100;
        }

        public void On500BaseBtnClick()
        {
            gameManager.GameBasePoint = 500;
            gameManager.GameTaiPoint = 200;
        }

        #endregion
    }
}
