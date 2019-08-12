using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChargeUI : MonoBehaviour
{
    public int m_PlayerNumber = 1;
    public Slider m_Slider;                    
    public Image m_FillImage;
    private PlayerState m_PlayerState;
    void Awake()
    {
        m_PlayerState = GetComponent<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {
        SetSliderValue();
    }

    void SetSliderValue() {
        float pressure = m_PlayerState.m_ChargeShovePressure;
        float minPressure = m_PlayerState.m_MinChargeShovePressure;
        float maxPressure = m_PlayerState.m_MaxChargeShovePressure;
        pressure = Normalise(pressure, minPressure, maxPressure);
        m_Slider.value = pressure;
    }

    float Normalise(float value, float min, float max) {
        return (value) / (max - min);
    }
}
