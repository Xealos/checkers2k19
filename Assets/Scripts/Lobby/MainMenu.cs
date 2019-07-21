using UnityEngine;

public class MainMenu : MonoBehaviour
{
    void Awake()
    {
        //TODO Brandon: This might need to become an option at some point
        Screen.fullScreen = false;
    }

    public void OnClickOptionsButton()
    {
        //TODO Brandon: Add options menu interface.
    }
    
    public void OnClickQuitButton()
    {
        Application.Quit();
    }
}