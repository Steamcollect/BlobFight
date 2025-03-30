using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlobPauseParticle : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    private bool isPaused = false;
    private List<ParticleSystem> particleSystems = new();

    private void OnEnable()
    {
        rseOnPause.action += Pause;
        rseOnResume.action += Resume;
    }

    private void OnDisable()
    {
        rseOnPause.action -= Pause;
        rseOnResume.action -= Resume;
    }

    private void Pause()
    {
        isPaused = true;

        particleSystems.Clear();
        particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();

        foreach (var particle in particleSystems)
        {
            if (particle.isPlaying)
            {
                particle.Pause();
            }
        }
    }

    private void Resume()
    {
        if (isPaused)
        {
            isPaused = false;

            foreach (var particle in particleSystems)
            {
                if (particle.isPaused)
                {
                    particle.Play();
                }
            }
        }
    }
}