using Game.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Lobby
{
    public class StartSettingPanel : MonoBehaviour
    {
        [SerializeField] private InputField playerName;
        private LobbyManager lobbyManager = null;
        // Start is called before the first frame update
        public void Start()
        {
            lobbyManager = LobbyManager.Instance;
            lobbyManager.PanelController.AddPanel(EnumPanel.StartSetting, gameObject);
            // Debug.Log("StartSetting");
        }

        #region - BtnCallBack

        public void OnLeaveBtnClick()
        {
            lobbyManager.PanelController.OpenPanel(EnumPanel.Start);
        }
        
        public void OnPlayerNameChanged()
        {
            GameManager.Instance.PlayerName = playerName.text;
        }

        #endregion
    }
}