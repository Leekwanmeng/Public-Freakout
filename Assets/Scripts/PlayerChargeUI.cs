using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerChargeUI : MonoBehaviour
{
    public Slider m_Slider;                      
    public Image m_FillImage;
    private PlayerController m_playerController;
    void Awake()
    {
        m_playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        SetSliderValue();
    }

    void SetSliderValue() {
        float pressure = m_playerController.GetChargeShovePressure();
        float minPressure = m_playerController.m_MinChargeShovePressure;
        float maxPressure = m_playerController.m_MaxChargeShovePressure;
        pressure = Normalise(pressure, minPressure, maxPressure);
        m_Slider.value = pressure;
    }

    float Normalise(float value, float min, float max) {
        return (value - min) / (max - min);
    }
}
