using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Lobby
{
    public class RoomCreatingPannel : MonoBehaviour
    {
        [SerializeField] private Dropdown dropDown = null;
        [SerializeField] private CanvasGroup canvasGroup = null;
        // [SerializeField] private LobbyManager lobbyManager = null;
        // [SerializeField] private TMP_InputField roomNameInputField = null;

        public void Start()
        {
            List<int> valueList = new List<int>();
            for (int i = 1; i <= 4; ++i)
                valueList.Add(i);
            SetDropdownOptions(valueList);
        }

        public void DisplayPannel(bool value)
        {
            canvasGroup.alpha = value ? 1 : 0;
            canvasGroup.interactable = value;
            canvasGroup.blocksRaycasts = value;
        }

        public void SetDropdownOptions(List<int> valueList)
        {
            if (valueList.Count > 0)
            {
                dropDown.ClearOptions();
                foreach (int value in valueList)
                {
                    Dropdown.OptionData data = new Dropdown.OptionData();
                    data.text = value.ToString();
                    dropDown.options.Add(data);
                }
            }
        }

        public void OnDropDownChange(int itemIndex)
        {
            if (itemIndex >= dropDown.options.Count)
                itemIndex = dropDown.options.Count - 1;
            if (itemIndex < 0)
                itemIndex = 0;
            dropDown.value = itemIndex;
        }

        public void OnCreateRoomBtnClick()
        {

        }
    }
}
