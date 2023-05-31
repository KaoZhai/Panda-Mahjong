using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingRoomScene : MonoBehaviour
{
    [SerializeField] private Game.Lobby.LobbyManager lobbyManager = null;

    #region BtnCallBack

    public void OnLeaveBtnClick()
    {
        lobbyManager.SetPairState(Game.Lobby.PanelState.Lobby);
    }

    public void OnCreateRoomBtnClick()
    {
        // DisplayRoomCreating(true);
        // DisplayRoomList(false);
    }

    #endregion

}
