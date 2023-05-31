using UnityEngine;

namespace Game.Lobby
{
    public class RoomSettingPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup = null;

        public void DisplayPannel(bool value)
        {
            canvasGroup.alpha = value ? 1 : 0;
            canvasGroup.interactable = value;
            canvasGroup.blocksRaycasts = value;
        }
    }
}
