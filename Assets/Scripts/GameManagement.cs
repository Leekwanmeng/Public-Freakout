using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagement : MonoBehaviour
{
    public int m_NumRoundsToWin = 3;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;
    public GameObject m_MessageCanvas;
    public GameObject m_PauseCanvas;
    public GameObject m_PlayerPrefab;         
    public PlayerManager[] m_Players;
    public AudioClip m_LobbyMusic;
    public AudioClip m_TrafficMusic;

    private bool[] m_ReadyPlayers;
    private bool m_Paused;
    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private PlayerManager m_RoundWinner;
    private PlayerManager m_GameWinner;
    private ItemSpawner m_ItemSpawner; 
    private StageManager2 m_StageManager;
    private AudioSource m_AudioSource;

    void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);
        m_Paused = false;
        m_ItemSpawner = GameObject.Find("ItemManager").GetComponent<ItemSpawner>();
        m_StageManager = GameObject.Find("Stage").GetComponent<StageManager2>();
        PlayLobbyMusic();
    }

    public void PlayLobbyMusic() {
        m_AudioSource.loop = true;
        m_AudioSource.clip = m_LobbyMusic;
        m_AudioSource.Play();
    }

    public void StartTraffic() {
        m_AudioSource.Stop();
        m_AudioSource.clip = m_TrafficMusic;
        m_AudioSource.Play();
    }

    public void BeginGame(bool[] readyPlayers) {
        m_ReadyPlayers = readyPlayers;
        m_StageManager.StartScript();
        SpawnPlayers();
        StartTraffic();
        StartCoroutine(GameLoop());
    }

    private void SpawnPlayers()
    {
        for (int i = 0; i < m_ReadyPlayers.Length; i++)
        {
            m_Players[i].m_PlayerNumber = i + 1;
            if (m_ReadyPlayers[i]) {
                m_Players[i].m_Instance =
                    Instantiate(m_PlayerPrefab, m_Players[i].m_SpawnPoint.position, m_Players[i].m_SpawnPoint.rotation) as GameObject;
                m_Players[i].Setup();
            }
            else {
                m_Players[i].m_Instance =
                    Instantiate(m_PlayerPrefab, Vector3.zero, m_Players[i].m_SpawnPoint.rotation) as GameObject;
                m_Players[i].Setup();
                m_Players[i].m_Instance.SetActive(false);
            }
        }
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Players.Length];

        for (int i = 0; i < targets.Length; i++) {
            targets[i] = m_Players[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        m_ItemSpawner.ResetItems();
        m_StageManager.Reset();
        RespawnAllPlayers();
        SetCameraTargets();
        DisablePlayerControl();
        m_CameraControl.SetStartPositionAndSize();
        m_RoundNumber++;
        m_MessageText.text = "ROUND STARTING IN 3...";
        yield return new WaitForSeconds(1);
        m_MessageText.text = "ROUND STARTING IN 2...";
        yield return new WaitForSeconds(1);
        m_MessageText.text = "ROUND STARTING IN 1...";
        yield return new WaitForSeconds(1);
    }


    private IEnumerator RoundPlaying()
    {
        EnablePlayerControl();
        m_MessageText.text = string.Empty;
        while (!OnePlayerLeft()) {
            CheckPause();
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        DisablePlayerControl();
        m_RoundWinner = null;
        m_RoundWinner = GetRoundWinner();
        if (m_RoundWinner != null) {
            m_RoundWinner.m_Wins++;
        }
        m_GameWinner = GetGameWinner();
        string message = EndMessage();
        m_MessageText.text = message;
        yield return m_EndWait;
    }

    private void CheckPause() {
        if (Input.GetButtonDown("StartButton1")) {
            if (!m_Paused) {
                Time.timeScale = 0.0f;
                DisablePlayerControl();
                m_Paused = true;
                // m_MessageText.text = "PAUSED";
                m_MessageCanvas.SetActive(false);
                m_PauseCanvas.SetActive(true);
            } else {
                // m_MessageText.text = string.Empty;
                m_MessageCanvas.SetActive(true);
                m_PauseCanvas.SetActive(false);
                EnablePlayerControl();
                Time.timeScale = 1.0f;
                m_Paused = false;
                
            }
        }
    }


    private bool OnePlayerLeft()
    {
        int numPlayersLeft = 0;

        for (int i = 0; i < m_Players.Length; i++)
        {
            if (m_ReadyPlayers[i]) {
                if (m_Players[i].m_Instance.activeSelf)
                    numPlayersLeft++; 
            }
        }

        return numPlayersLeft <= 1;
    }


    private PlayerManager GetRoundWinner()
    {
        for (int i = 0; i < m_Players.Length; i++)
        {
            if (m_ReadyPlayers[i]) {
                if (m_Players[i].m_Instance.activeSelf)
                    return m_Players[i];
            }
        }

        return null;
    }


    private PlayerManager GetGameWinner()
    {
        for (int i = 0; i < m_Players.Length; i++)
        {
            if (m_ReadyPlayers[i]) {
                if (m_Players[i].m_Wins == m_NumRoundsToWin)
                    return m_Players[i];
            }
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = "ROUND WINNER: " + m_RoundWinner.m_ColoredPlayerText;

        message += "\n\n\n\n\n";

        for (int i = 0; i < m_Players.Length; i++)
        {
            if (m_ReadyPlayers[i]) message += m_Players[i].m_ColoredPlayerText + ": " + m_Players[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " VICTORY";

        return message;
    }


    private void RespawnAllPlayers() {
        for (int i = 0; i < m_ReadyPlayers.Length; i++)
        {
            Destroy(m_Players[i].m_Instance);
            if (m_ReadyPlayers[i]) {
                m_Players[i].m_Instance = Instantiate (
                        m_PlayerPrefab,
                        m_Players[i].m_SpawnPoint.position,
                        m_Players[i].m_SpawnPoint.rotation
                    ) as GameObject;
                m_Players[i].Setup();
            }
            else {
                m_Players[i].m_Instance = Instantiate (
                        m_PlayerPrefab,
                        Vector3.zero,
                        m_Players[i].m_SpawnPoint.rotation
                    ) as GameObject;
                m_Players[i].Setup();
                m_Players[i].m_Instance.SetActive(false);
            }

        }
    }

    private void EnablePlayerControl()
    {
        for (int i = 0; i < m_ReadyPlayers.Length; i++)
        {
            if (m_ReadyPlayers[i]) m_Players[i].EnableControl();
        }
    }


    private void DisablePlayerControl()
    {
        for (int i = 0; i < m_Players.Length; i++)
        {
            if (m_ReadyPlayers[i]) m_Players[i].DisableControl();
        }
    }
}