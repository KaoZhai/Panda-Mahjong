using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Lobby
{
    public class PlayerUnit : MonoBehaviour
    {
        // TODO: using TMP_text
        [SerializeField] private Text playerNameTxt = null;
        [SerializeField] private Text isReadyTxt = null;

        private string playerName = null;
        private bool isReady = false;

        public void SetInfo(string playerName, bool isReady)
        {
            this.playerName = playerName;
            this.isReady = isReady;

            playerNameTxt.text = playerName;
            isReadyTxt.text = isReady ? "Ready" : "Not Ready";
        }
    }
}