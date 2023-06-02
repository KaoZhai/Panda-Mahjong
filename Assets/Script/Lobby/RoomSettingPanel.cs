using UnityEngine;

namespace Game.Lobby
{
    public class RoomSettingPanel : MonoBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager = null;

        public void Start()
        {
            lobbyManager.PanelController.AddPanel(EnumPanel.WaitingSetting, gameObject);
        }
    }
}
