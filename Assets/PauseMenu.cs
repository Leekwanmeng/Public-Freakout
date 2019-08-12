using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public Canvas cOwnCanvas;

    void Update()
    {
        if (Input.GetButtonDown("AButton1")) {
            cOwnCanvas.gameObject.SetActive(false);
            SceneManager.LoadScene(0);
        }
    }
}
