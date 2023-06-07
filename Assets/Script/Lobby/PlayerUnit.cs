using UnityEngine;
using UnityEngine.UI;

namespace Game.Lobby
{
    public class PlayerUnit : MonoBehaviour
    {
        [SerializeField] private Text playerNameTxt;
        [SerializeField] private Text isReadyTxt;
        [SerializeField] private Text pointsTxt;
        

        public void SetInfo(string playerName, bool isReady, int points)
        {
            playerNameTxt.text = playerName;
            isReadyTxt.text = isReady ? "Ready" : "Not Ready";
            pointsTxt.text = points.ToString();
        }
    }
}