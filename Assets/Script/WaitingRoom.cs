using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WaitingRoom : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LeaveButton()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void StartButton()
    {
        SceneManager.LoadScene("PlayingRoom");
    }

}
