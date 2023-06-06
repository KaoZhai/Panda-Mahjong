using UnityEngine;

public class ClickSeController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && PlayerPrefs.GetFloat("Click") > 0.0f)
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}
