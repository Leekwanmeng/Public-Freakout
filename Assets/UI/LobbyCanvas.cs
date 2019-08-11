using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// attached to CanvasLobby in MainMenu scene, need to assign the textPressToStart gameobject

public class LobbyCanvas : MonoBehaviour
{
    public GameObject textPressA;
    public GameObject game;
    private Canvas cLobby;

    private GameObject[] lobbyPlayers;
    private bool[] ready;
    private int numReady;
    private bool player;
    private Camera mCam;

    // Start is called before the first frame update
    void Start()
    {
        cLobby = gameObject.GetComponent<Canvas>();
        lobbyPlayers = GameObject.FindGameObjectsWithTag("LobbyPlayer");
        ready = new bool[lobbyPlayers.Length];
        numReady = 0;
        mCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if(readyToStart() && Input.GetKeyDown("r"))
        {
            Debug.Log("start!");
            cLobby.gameObject.SetActive(false);
            mCam.orthographic = true;
            game.SetActive(true);
            //mCam.orthographicSize = 2.5f;

        }
        
    }

    bool readyToStart()
    {
        numReady = 0;
        for (int i = 0; i < lobbyPlayers.Length; i++)
        {
            player = lobbyPlayers[i].gameObject.GetComponent<PlayerJoin>().joined;
            ready[i] = player;
        }

        for (int j = 0; j < ready.Length; j++)
        {
            if (ready[j])
            {
                numReady++;
            }
        }
        if (!textPressA.activeSelf && numReady >= 2)
        {
            textPressA.gameObject.SetActive(true);
        }
        if (textPressA.activeSelf && numReady <= 1)
        {
            textPressA.gameObject.SetActive(false);
        }

        if (textPressA.activeSelf)
        {
            return true;
        }
        return false;
    }
}
