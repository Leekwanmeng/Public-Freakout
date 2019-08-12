using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToMenu : MonoBehaviour
{
    public Canvas cMenu;
    public Canvas cLobby;
    public Canvas cOwnCanvas;

    void Update()
    {
        if (Input.GetButtonDown("AButton1") || Input.GetButtonDown("BButton1")) {
            ToMainMenu();
        }
    }
    public void ToMainMenu()
    {
        cMenu.gameObject.SetActive(true);
        cLobby.gameObject.SetActive(false);
        cOwnCanvas.gameObject.SetActive(false);
    }
}
