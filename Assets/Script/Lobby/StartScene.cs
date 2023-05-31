using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Game.Lobby.LobbyManager lobbyManager = null;

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
}
