using System;
using System.Collections.Generic;
using UnityEngine;

public class BlobParticle : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int touchParticleStartingCount;
    [SerializeField] int hitParticleStartingCount;
    [SerializeField] int extendHitParticleStartingCount;

    [SerializeField] AnimationCurve hitScaleBySpeedCurve;
    [SerializeField] AnimationCurve extendHitScaleBySpeedCurve;

    [Header("References")]
    [SerializeField] BlobMotor motor;
    [SerializeField] BlobTrigger trigger;
    [SerializeField] BlobHealth health;

    [Space(5)]
    [SerializeField] ParticleSystem dustParticlePrefab;
    Queue<ParticleCallback> dustParticles = new();

    [SerializeField] ParticleSystem deathParticlePrefab;
    Queue<ParticleCallback> deathParticles = new();
    
    [SerializeField] ParticleSystem destroyParticlePrefab;
    Queue<ParticleCallback> destroyParticles = new();

    [SerializeField] ParticleSystem hitParticlePrefab;
    Queue <ParticleCallback> hitParticles = new();

    [SerializeField] ParticleSystem extendHitParticlePrefab;
    Queue <ParticleCallback> extendHitParticles = new();

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

        for (int i = 0; i < extendHitParticleStartingCount; i++)
            extendHitParticles.Enqueue(CreateParticle(extendHitParticlePrefab, OnExtendHitParticleEnd));

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
        ParticleCallback particle;
        if (dustParticles.Count <= 0) particle = CreateParticle(dustParticlePrefab, OnTouchParticleEnd);
        else particle = dustParticles.Dequeue();

        particle.gameObject.SetActive(true);

        particle.transform.position = position;
        particle.transform.up = direction;

        particle.Play();
    }
    void OnTouchParticleEnd(ParticleCallback particle)
    {
        particle.gameObject.SetActive(false);
        dustParticles.Enqueue(particle);
    }

    public void DeathParticle(Vector2 position, BlobColor color)
    {
        ParticleCallback particle;
        if (deathParticles.Count <= 0) particle = CreateParticle(deathParticlePrefab, OnDeathParticleEnd);
        else particle = deathParticles.Dequeue();

        particle.gameObject.SetActive(true);

        particle.transform.position = position;
        //main.startColor = color.fillColor;

        particle.Play();
    }
    void OnDeathParticleEnd(ParticleCallback particle)
    {
        particle.gameObject.SetActive(false);
        deathParticles.Enqueue(particle);
    }
    
    public void DestroyParticle(ContactPoint2D contact, BlobColor color)
    {
        ParticleCallback particle;
        if (destroyParticles.Count <= 0) particle = CreateParticle(destroyParticlePrefab, OnDestroyParticleEnd);
        else particle = destroyParticles.Dequeue();

        particle.gameObject.SetActive(true);

        particle.transform.position = contact.point;
        particle.transform.up = contact.normal;
        //main.startColor = color.fillColor;

        particle.Play();
    }
    void OnDestroyParticleEnd(ParticleCallback particle)
    {
        particle.gameObject.SetActive(false);
        deathParticles.Enqueue(particle);
    }

    public void HitParticle(Vector2 position, Vector2 rotation, float speed)
    {
        ParticleCallback particle;
        if (hitParticles.Count <= 0) particle = CreateParticle(hitParticlePrefab, OnHitParticleEnd);
        else particle = hitParticles.Dequeue();

        particle.gameObject.SetActive(true);

        particle.SetColor(motor.GetColor().fillColor);

        particle.transform.position = position;
        particle.transform.up = -rotation;
        particle.transform.localScale = Vector2.one * hitScaleBySpeedCurve.Evaluate(speed);
        //print(hitScaleBySpeedCurve.Evaluate(speed));

        particle.Play();
    }
    void OnHitParticleEnd(ParticleCallback particle)
    {
        particle.gameObject.SetActive(false);
        hitParticles.Enqueue(particle);
    }

    public void ExtendHitParticle(Vector2 position, Vector2 rotation, float speed)
    {
        ParticleCallback particle;
        if (extendHitParticles.Count <= 0) particle = CreateParticle(extendHitParticlePrefab, OnHitParticleEnd);
        else particle = extendHitParticles.Dequeue();

        particle.gameObject.SetActive(true);

        particle.SetColor(motor.GetColor().fillColor);

        particle.transform.position = position;
        particle.transform.up = -rotation;
        particle.transform.localScale = Vector2.one * extendHitScaleBySpeedCurve.Evaluate(speed);

        particle.Play();
    }
    void OnExtendHitParticleEnd(ParticleCallback particle)
    {
        particle.gameObject.SetActive(false);
        extendHitParticles.Enqueue(particle);
    }

    ParticleCallback CreateParticle(ParticleSystem prefab, Action<ParticleCallback> stopAction)
    {
        ParticleSystem particle = Instantiate(prefab, transform);

        ParticleCallback callback = particle.GetComponent<ParticleCallback>();
        callback.Setup(stopAction, particle);

        particle.gameObject.SetActive(false);

        return callback;
    }
}