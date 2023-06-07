using UnityEngine;
using UnityEngine.UI;
namespace Game.Music
{
    public class MusicToggle : MonoBehaviour
    {
        [SerializeField] private string saveType = "";
        [SerializeField] private BgmController musicController;

        void Start()
        {
            if(PlayerPrefs.HasKey(saveType) == false)
            {
                PlayerPrefs.SetFloat(saveType, 1.0f);
            }
            SetVolume();
        }

        public void OnToggleClick()
        {
            PlayerPrefs.SetFloat(saveType, gameObject.GetComponent<Toggle>().isOn ? 1.0f : 0.0f);
            SetVolume();
        }

        private void SetVolume()
        {
            if(musicController != null)
            {
                musicController.SetVolume(PlayerPrefs.GetFloat(saveType));
            }
            gameObject.GetComponent<Toggle>().isOn = (PlayerPrefs.GetFloat(saveType) > 0.0f);
        }
    }
}