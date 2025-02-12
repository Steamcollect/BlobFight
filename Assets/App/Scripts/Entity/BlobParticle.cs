using System.Collections.Generic;
using UnityEngine;

public class BlobParticle : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int touchParticleStartingCount;

    [Header("References")]
    [SerializeField] BlobTrigger trigger;

    [Space(5)]
    [SerializeField] ParticleSystem dustParticlePrefab;
    Queue<ParticleSystem> dustParticles = new();

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void OnEnable()
    {
        trigger.OnCollisionEnter += DustParticle;
        //trigger.OnCollisionExit += DustParticle;
    }
    private void OnDisable()
    {
        trigger.OnCollisionEnter -= DustParticle;
        trigger.OnCollisionExit -= DustParticle;
    }

    private void Start()
    {
        for (int i = 0; i < touchParticleStartingCount; i++)
        {
            dustParticles.Enqueue(CreateParticle(dustParticlePrefab));
        }
    }

    void DustParticle(Collision2D collision)
    {
        ParticleSystem particle;
        if (dustParticles.Count <= 0) particle = CreateParticle(dustParticlePrefab);
        else particle = dustParticles.Dequeue();

        particle.gameObject.SetActive(true);

        ParticleSystem.MainModule main = particle.main;
        main.stopAction = ParticleSystemStopAction.Callback;

        particle.transform.position = collision.GetContact(0).point;
        particle.transform.up = collision.GetContact(0).normal;

        particle.Play();
    }
    void OnTouchParticleEnd(ParticleSystem particle)
    {
        particle.gameObject.SetActive(false);
        dustParticles.Enqueue(particle);
    }

    ParticleSystem CreateParticle(ParticleSystem prefab)
    {
        ParticleSystem particle = Instantiate(prefab, transform);
        particle.GetComponent<ParticleCallback>().Setup(OnTouchParticleEnd, particle);

        particle.gameObject.SetActive(false);
        return particle;
    }
}