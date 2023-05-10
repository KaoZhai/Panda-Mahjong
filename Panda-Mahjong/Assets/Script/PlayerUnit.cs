using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Lobby
{
    public class PlayerUnit : MonoBehaviour
    {
        // TODO: using TMP_text
        [SerializeField] private string playerNameTxt = null;
        [SerializeField] private string isReadyTxt = null;

        private string playerName = null;
        private bool isReady = false;

        public void SetInfo(string playerName, bool isReady)
        {
            this.playerName = playerName;
            this.isReady = isReady;

            playerNameTxt = playerName;
            isReadyTxt = isReady ? "Ready" : "";
        }
    }
}