using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// attached to CanvasLobby in MainMenu scene, need to assign the textPressToStart gameobject

public class LobbyCanvas : MonoBehaviour
{
    public GameObject m_StartText;
    private Canvas cLobby;
    private GameObject[] lobbyPlayers;
    private bool[] ready;
    private int numReady;
    private bool player;
    private Camera mCam;
    private GameManagement m_GameManagement;
    private GameObject m_Stage;
    private CameraControl m_CameraControl;

    // Start is called before the first frame update
    void Start()
    {
        cLobby = gameObject.GetComponent<Canvas>();
        lobbyPlayers = GameObject.FindGameObjectsWithTag("LobbyPlayer");
        m_GameManagement = GameObject.Find("GameManager").GetComponent<GameManagement>();
        m_Stage = GameObject.Find("Stage");

        ready = new bool[lobbyPlayers.Length];
        numReady = 0;
        mCam = Camera.main;
        mCam.transform.Translate(new Vector3(0f, -1.8f, 40f));
    }

    // Update is called once per frame
    void Update()
    {
        if(readyToStart() && Input.GetButtonDown("StartButton1"))
        {
            Debug.Log("start!");
            mCam.transform.Translate(new Vector3(0f, 1.8f, -40f));
            cLobby.gameObject.SetActive(false);
            mCam.orthographic = true;
            m_Stage.gameObject.SetActive(true);
            m_GameManagement.BeginGame();

            //mCam.orthographicSize = 2.5f;
            // SceneManager.LoadScene("Main");

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
        if (numReady >= 2)
        {
            m_StartText.gameObject.SetActive(true);
        }
        if ( numReady <= 1)
        {
            m_StartText.gameObject.SetActive(false);
        }

        if (m_StartText.activeSelf)
        {
            return true;
        }
        return false;
    }
}
