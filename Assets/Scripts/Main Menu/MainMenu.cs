using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Canvas cMenu;
    public Canvas cLobby;
    public Canvas cHowToPlay;

    public void ToLobby()
    {
        cMenu.gameObject.SetActive(false);
        cLobby.gameObject.SetActive(true);
        cHowToPlay.gameObject.SetActive(false);
    }

    public void ToHowToPlay() {
        cMenu.gameObject.SetActive(false);
        cHowToPlay.gameObject.SetActive(true);
    }

    public void ToMainMenu()
    {
        cMenu.gameObject.SetActive(true);
        cLobby.gameObject.SetActive(false);
        cHowToPlay.gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Debug.Log("quitgame!!!");
        Application.Quit();
    }
}
