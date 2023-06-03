using UnityEngine;

namespace Game.Lobby
{
    public class RoomSettingPanel : MonoBehaviour
    {
        public void Start()
        {
            LobbyManager.Instance.PanelController.AddPanel(EnumPanel.WaitingSetting, gameObject);
        }
    }
}
