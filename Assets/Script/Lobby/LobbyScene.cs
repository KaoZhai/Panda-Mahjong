using UnityEngine;

namespace Game.Lobby
{
    public class LobbyScene : MonoBehaviour
    {
        private LobbyManager lobbyManager;

        public void Start()
        {
            lobbyManager = LobbyManager.Instance;
            lobbyManager.PanelController.AddPanel(EnumPanel.Lobby, gameObject);
        }

        #region - BtnCallBack

        public void OnLeaveBtnClick()
        {
            lobbyManager.PanelController.OpenPanel(EnumPanel.Start);
        }

        public void OnCreateRoomBtnClick()
        {
            lobbyManager.PanelController.OpenPanel(EnumPanel.RoomCreating);
        }

        #endregion
    }
}
