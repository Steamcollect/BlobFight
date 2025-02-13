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
        trigger.OnCollisionEnter += OnTouchEnter;
        trigger.OnCollisionExitGetLastContact += OnTouchExit;
    }
    private void OnDisable()
    {
        trigger.OnCollisionEnter -= OnTouchEnter;
        trigger.OnCollisionExitGetLastContact -= OnTouchExit;
    }

    private void Start()
    {
        for (int i = 0; i < touchParticleStartingCount; i++)
        {
            dustParticles.Enqueue(CreateParticle(dustParticlePrefab));
        }
    }

    void OnTouchEnter(Collision2D coll) => DustParticle(coll.GetContact(0).point, coll.GetContact(0).normal);
    void OnTouchExit(ContactPoint2D contact) => DustParticle(contact.point, contact.normal);

    void DustParticle(Vector2 position, Vector2 direction)
    {
        ParticleSystem particle;
        if (dustParticles.Count <= 0) particle = CreateParticle(dustParticlePrefab);
        else particle = dustParticles.Dequeue();

        particle.gameObject.SetActive(true);

        ParticleSystem.MainModule main = particle.main;
        main.stopAction = ParticleSystemStopAction.Callback;

        particle.transform.position = position;
        particle.transform.up = direction;

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