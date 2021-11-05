using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void OpenWindow(GameObject window)
    {
        window.SetActive(true);
    }
    
    public void CloseWindow(GameObject window)
    {
        window.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
