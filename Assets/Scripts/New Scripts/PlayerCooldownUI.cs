using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCooldownUI : MonoBehaviour
{
    public int m_PlayerNumber;
    public Slider m_Slider;                    
    public Image m_FillImage;
    private PlayerState m_PlayerState;
    private float m_LocalMaxCooldown;

    void Awake()
    {
        m_PlayerState = GetComponent<PlayerState>();
    }

    void Start()
    {
        m_LocalMaxCooldown = 0f;
    }

    void Update()
    {
        CheckNewMaxCooldown();
        SetSliderValue();
    }

    void CheckNewMaxCooldown() {
        // Set new local starting cooldown
        if (m_PlayerState.m_Cooldown > m_LocalMaxCooldown) {
            m_LocalMaxCooldown = m_PlayerState.m_Cooldown;
        }

        // Reset to zero
        if (m_PlayerState.m_Cooldown <= 0f) {
            m_LocalMaxCooldown = 0f;
        }
    }

    void SetSliderValue() {
        float value = 0f;
        if (m_LocalMaxCooldown > 0f) {
            value = Normalise(m_PlayerState.m_Cooldown, 0f, m_LocalMaxCooldown);
        }
        m_Slider.value = value;
    }

    float Normalise(float value, float min, float max) {
        return (value - min) / (max - min);
    }
}
