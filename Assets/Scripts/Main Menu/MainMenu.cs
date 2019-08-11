using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Canvas cMenu;
    public Canvas cLobby;
    public Camera mCamera;


    public void ToLobby()
    {
        cMenu.gameObject.SetActive(false);
        cLobby.gameObject.SetActive(true);

        mCamera.fieldOfView = 3f;
    }

    public void QuitGame()
    {
        Debug.Log("quitgame!!!");
        Application.Quit();
    }
}
