using UnityEngine;
public class StartScene : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Game.Lobby.LobbyManager lobbyManager = null;

    void Start() 
    {
        lobbyManager.SetPairState(Game.Lobby.PanelState.Start);
        Debug.Log("Start");
    }

    #region BtnCallBack
    public void StartButton()
    {
        lobbyManager.SetPairState(Game.Lobby.PanelState.Lobby);
    }

    public void SettingButton()
    {
        // Setting Panel
    }

    public void ExitButton()
    {
        Application.Quit();
    }
    #endregion
}
