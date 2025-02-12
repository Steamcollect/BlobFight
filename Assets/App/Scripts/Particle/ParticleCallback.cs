using System;
using UnityEngine;

public class ParticleCallback : MonoBehaviour
{
    //[Header("Settings")]

    //[Header("References")]
    Action<ParticleSystem> OnParticleStopped;
    ParticleSystem particleConnected;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    public void Setup(Action<ParticleSystem> onStopped, ParticleSystem particleConnected)
    {
        OnParticleStopped += onStopped;
        this.particleConnected = particleConnected;
    }

    private void OnParticleSystemStopped()
    {
        OnParticleStopped?.Invoke(particleConnected);
    }
}