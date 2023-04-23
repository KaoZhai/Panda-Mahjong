using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartButton()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void SettingButton()
    {
        SceneManager.LoadScene("Setting");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
