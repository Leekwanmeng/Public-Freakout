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
    private bool[] m_ReadyPlayers;
    private int numReady;
    private Camera mCam;
    private GameManagement m_GameManagement;
    private GameObject m_Stage;
    private StartSparklers m_StartSparklers;
    private CameraControl m_CameraControl;

    // Start is called before the first frame update
    void Start()
    {
        cLobby = gameObject.GetComponent<Canvas>();
        lobbyPlayers = GameObject.FindGameObjectsWithTag("LobbyPlayer");
        m_GameManagement = GameObject.Find("GameManager").GetComponent<GameManagement>();
        m_StartSparklers = GameObject.Find("StartSparklers").GetComponent<StartSparklers>();
        m_Stage = GameObject.Find("Stage");

        m_ReadyPlayers = new bool[lobbyPlayers.Length];
        for (int i=0; i<m_ReadyPlayers.Length; i++) {
            m_ReadyPlayers[i] = false;
        }
        numReady = 0;
        mCam = Camera.main;
        mCam.transform.Translate(new Vector3(0f, -1.8f, 40f));
    }

    void Update()
    {
        if (readyToStart() && Input.GetButtonDown("StartButton1"))
        {
            StartCoroutine(StartGame());
        }
        
    }

    IEnumerator StartGame() {
        m_StartSparklers.started = true;
        yield return new WaitForSeconds(2);
        m_StartSparklers.started = false;
        yield return new WaitForSeconds(1);
        Destroy(m_StartSparklers);
        mCam.transform.Translate(new Vector3(0f, 1.8f, -40f));
        cLobby.gameObject.SetActive(false);
        mCam.orthographic = true;
        m_Stage.gameObject.SetActive(true);
        m_GameManagement.BeginGame(m_ReadyPlayers);
    }

    bool readyToStart()
    {
        numReady = 2;
        for (int i = 0; i < lobbyPlayers.Length; i++)
        {
            PlayerJoin playerJoin = lobbyPlayers[i].gameObject.GetComponent<PlayerJoin>();
            m_ReadyPlayers[playerJoin.m_PlayerNumber-1] = playerJoin.joined;
        }

        for (int j = 0; j < m_ReadyPlayers.Length; j++) {
            if (m_ReadyPlayers[j]) {
                numReady++;
            }
        }
        

        if (numReady >= 2) {
            m_StartText.gameObject.SetActive(true);
            return true;
        }
        else {
            m_StartText.gameObject.SetActive(false);
            return false;
        }

    }
}
