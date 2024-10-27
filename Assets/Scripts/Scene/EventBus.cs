using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class EventBus : MonoBehaviour
{
    public static EventBus Instance { get; private set; }

    public UnityEvent PlayerDied;

    private CinemachineImpulseSource camShakeSource;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            camShakeSource = GetComponent<CinemachineImpulseSource>();
        }
    }

    public void CameraShake(float force, float duration = 0.3f)
    {
        camShakeSource.m_ImpulseDefinition.m_ImpulseDuration = duration;
        camShakeSource.GenerateImpulseWithForce(force);
    }
}
