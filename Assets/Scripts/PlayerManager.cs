using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class PlayerManager
{
    public Color m_PlayerColor;            
    public Transform m_SpawnPoint;         
    [HideInInspector] public int m_PlayerNumber;             
    [HideInInspector] public string m_ColoredPlayerText;
    [HideInInspector] public GameObject m_Instance;          
    [HideInInspector] public int m_Wins;

    private PlayerState m_PlayerState;
    private PlayerWalk m_PlayerWalk;
    private PlayerAction m_PlayerAction;
    private PlayerChargeUI m_PlayerChargeUI;
    private GameObject m_CanvasGameObject;


    public void Setup()
    {
        m_PlayerState = m_Instance.GetComponent<PlayerState>();
        m_PlayerWalk = m_Instance.GetComponent<PlayerWalk>();
        m_PlayerAction = m_Instance.GetComponent<PlayerAction>();
        m_PlayerChargeUI = m_Instance.GetComponent<PlayerChargeUI>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        m_PlayerState.m_PlayerNumber = m_PlayerNumber;
        m_PlayerChargeUI.m_PlayerNumber = m_PlayerNumber;
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        Color fillColor = m_PlayerColor;
        fillColor.a = 1f;
        m_PlayerChargeUI.m_FillImage.color = fillColor;
        m_PlayerState.m_selectImage.color = m_PlayerColor;

        // MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();
        // for (int i = 0; i < renderers.Length; i++)
        // {
        //     renderers[i].material.color = m_PlayerColor;
        // }
    }


    public void DisableControl()
    {
        // m_PlayerState.m_CanWalk = false;
        // m_PlayerState.enabled = false;
        m_PlayerWalk.enabled = false;
        m_PlayerAction.enabled = false;
        // m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        // m_PlayerState.enabled = true;
        m_PlayerWalk.enabled = true;
        m_PlayerAction.enabled = true;
        // m_PlayerState.m_CanWalk = true;
        // m_CanvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
