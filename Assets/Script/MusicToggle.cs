using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Game.Music
{
    public class MusicToggle : MonoBehaviour
    {
        [SerializeField] private string saveType = "";
        [SerializeField] private BgmController musicController = null;

        void Start()
        {
            if(PlayerPrefs.HasKey(saveType) == false)
            {
                PlayerPrefs.SetFloat(saveType, 1.0f);
            }
            SetVolume();
        }

        public void OnToogleClick()
        {
            if(gameObject.GetComponent<Toggle>().isOn)
            {
                PlayerPrefs.SetFloat(saveType, 1.0f);
            }
            else
            {
                PlayerPrefs.SetFloat(saveType, 0.0f);
            }
            SetVolume();
        }

        private void SetVolume()
        {
            if(musicController != null)
            {
                musicController.setVolume(PlayerPrefs.GetFloat(saveType));
            }
            gameObject.GetComponent<Toggle>().isOn = (PlayerPrefs.GetFloat(saveType) > 0.0f);
        }
    }
}