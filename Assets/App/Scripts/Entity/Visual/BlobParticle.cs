using System;
using System.Collections.Generic;
using UnityEngine;

public class BlobParticle : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int touchParticleStartingCount;
    [SerializeField] int hitParticleStartingCount;

    [SerializeField] AnimationCurve hitScaleBySpeedCurve;

    [Header("References")]
    [SerializeField] BlobTrigger trigger;
    [SerializeField] BlobHealth health;

    [Space(5)]
    [SerializeField] ParticleSystem dustParticlePrefab;
    Queue<ParticleSystem> dustParticles = new();

    [SerializeField] ParticleSystem deathParticlePrefab;
    Queue<ParticleSystem> deathParticles = new();
    
    [SerializeField] ParticleSystem destroyParticlePrefab;
    Queue<ParticleSystem> destroyParticles = new();

    [SerializeField] ParticleSystem hitParticlePrefab;
    Queue <ParticleSystem> hitParticles = new();

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
            dustParticles.Enqueue(CreateParticle(dustParticlePrefab, OnTouchParticleEnd));

        for (int i = 0; i < hitParticleStartingCount; i++)
            hitParticles.Enqueue(CreateParticle(hitParticlePrefab, OnHitParticleEnd));

        deathParticles.Enqueue(CreateParticle(deathParticlePrefab, OnDeathParticleEnd));
    }

    void OnTouchEnter(Collision2D coll)
    {
        if (coll.transform.CompareTag("Blob")) return;
        DustParticle(coll.GetContact(0).point, coll.GetContact(0).normal);
    }
    void OnTouchExit(ContactPoint2D contact)
    {
        if (contact.collider.transform.CompareTag("Blob")) return;
        DustParticle(contact.point, contact.normal);
    }

    void DustParticle(Vector2 position, Vector2 direction)
    {
        ParticleSystem particle;
        if (dustParticles.Count <= 0) particle = CreateParticle(dustParticlePrefab, OnTouchParticleEnd);
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

    public void DeathParticle(Vector2 position, BlobColor color)
    {
        ParticleSystem particle;
        if (deathParticles.Count <= 0) particle = CreateParticle(deathParticlePrefab, OnDeathParticleEnd);
        else particle = deathParticles.Dequeue();

        particle.gameObject.SetActive(true);

        ParticleSystem.MainModule main = particle.main;
        main.stopAction = ParticleSystemStopAction.Callback;

        particle.transform.position = position;
        //main.startColor = color.fillColor;

        particle.Play();
    }
    void OnDeathParticleEnd(ParticleSystem particle)
    {
        particle.gameObject.SetActive(false);
        deathParticles.Enqueue(particle);
    }
    
    public void DestroyParticle(ContactPoint2D contact, BlobColor color)
    {
        ParticleSystem particle;
        if (destroyParticles.Count <= 0) particle = CreateParticle(destroyParticlePrefab, OnDestroyParticleEnd);
        else particle = destroyParticles.Dequeue();

        particle.gameObject.SetActive(true);

        ParticleSystem.MainModule main = particle.main;
        main.stopAction = ParticleSystemStopAction.Callback;

        particle.transform.position = contact.point;
        particle.transform.up = contact.normal;
        //main.startColor = color.fillColor;

        particle.Play();
    }
    void OnDestroyParticleEnd(ParticleSystem particle)
    {
        particle.gameObject.SetActive(false);
        deathParticles.Enqueue(particle);
    }

    public void HitParticle(Vector2 position, Vector2 rotation, float speed)
    {
        ParticleSystem particle;
        if (hitParticles.Count <= 0) particle = CreateParticle(hitParticlePrefab, OnHitParticleEnd);
        else particle = hitParticles.Dequeue();

        particle.gameObject.SetActive(true);

        ParticleSystem.MainModule main = particle.main;
        main.stopAction = ParticleSystemStopAction.Callback;

        particle.transform.position = position;
        particle.transform.up = rotation;
        particle.transform.localScale = Vector2.one * hitScaleBySpeedCurve.Evaluate(speed);
        print(hitScaleBySpeedCurve.Evaluate(speed));

        particle.Play();
    }
    void OnHitParticleEnd(ParticleSystem particle)
    {
        particle.gameObject.SetActive(false);
        hitParticles.Enqueue(particle);
    }

    ParticleSystem CreateParticle(ParticleSystem prefab, Action<ParticleSystem> stopAction)
    {
        ParticleSystem particle = Instantiate(prefab, transform);
        particle.GetComponent<ParticleCallback>().Setup(stopAction, particle);

        particle.gameObject.SetActive(false);
        return particle;
    }
}