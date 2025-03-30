using System;
using UnityEngine;

public class ParticleCallback : MonoBehaviour
{
    private Action<ParticleCallback> OnParticleStopped;
    private ParticleSystem particleConnected;

    public void Setup(Action<ParticleCallback> onStopped, ParticleSystem particleConnected)
    {
        ParticleSystem.MainModule main = particleConnected.main;
        main.stopAction = ParticleSystemStopAction.Callback;

        OnParticleStopped += onStopped;
        this.particleConnected = particleConnected;
    }

    public virtual void SetColor(Color newColor)
    {
        
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