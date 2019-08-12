using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSparklers : MonoBehaviour
{
    public bool started;
    private ParticleSystem[] m_ParticleSystems;
    void Awake()
    {
        m_ParticleSystems = GetComponentsInChildren<ParticleSystem>();
    }
    
    void Start()
    {
        started = false;
    }

    void Update()
    {
        Play();
    }

    void Play() {
        if (started) {
            foreach (ParticleSystem ps in m_ParticleSystems) {
                ps.Play();
            }
        }
        else {
            foreach (ParticleSystem ps in m_ParticleSystems) {
                ps.Stop();
            }
        }
    }
}
