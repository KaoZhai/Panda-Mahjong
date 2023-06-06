using UnityEngine;
using UnityEngine.UI;

namespace Game.Lobby
{
    public class PlayerUnit : MonoBehaviour
    {
        // TODO: using TMP_text
        [SerializeField] private Text playerNameTxt;
        [SerializeField] private Text isReadyTxt;
        

        public void SetInfo(string playerName, bool isReady)
        {
            playerNameTxt.text = playerName;
            isReadyTxt.text = isReady ? "Ready" : "Not Ready";
        }
    }
}