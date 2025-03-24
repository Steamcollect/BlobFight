using System;
using UnityEngine;

public class ParticleCallback : MonoBehaviour
{
    //[Header("Settings")]

    //[Header("References")]
    Action<ParticleCallback> OnParticleStopped;
    ParticleSystem particleConnected;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    public void Setup(Action<ParticleCallback> onStopped, ParticleSystem particleConnected)
    {
        ParticleSystem.MainModule main = particleConnected.main;
        main.stopAction = ParticleSystemStopAction.Callback;

        OnParticleStopped += onStopped;
        this.particleConnected = particleConnected;
    }

    public virtual void SetColor(Color newColor)
    {
        // Do nothing
    }

    public void Play()
    {
        particleConnected.Play();
    }

    private void OnParticleSystemStopped()
    {
        OnParticleStopped?.Invoke(this);
    }
}