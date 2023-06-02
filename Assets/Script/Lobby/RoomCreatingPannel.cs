using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Lobby
{
    public class RoomCreatingPannel : MonoBehaviour
    {
        [SerializeField] private Dropdown dropDown = null;
        [SerializeField] private LobbyManager lobbyManager = null;
        [SerializeField] private InputField roomNameInputField = null;

        public void Start()
        {
            lobbyManager.PanelController.AddPanel(EnumPanel.RoomCreating, gameObject);

            List<int> valueList = new List<int>();
            for (int i = 1; i <= 4; ++i)
                valueList.Add(i);
            SetDropdownOptions(valueList);
        }

        #region BtnCallBack

        public void OnCancelBtnClick()
        {
            lobbyManager.PanelController.OpenPanel(EnumPanel.RoomList);
        }

        public async void OnCreateRoomBtnClick()
        {
            Debug.Log(roomNameInputField.text);
            await lobbyManager.CreateRoom(roomNameInputField.text, 4);
        }

        #endregion

        #region Helper Function
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

        #endregion
    }
}
