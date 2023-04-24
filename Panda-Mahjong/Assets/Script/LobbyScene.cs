using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LeaveButton()
    {
        SceneManager.LoadScene("Start");
    }
}
