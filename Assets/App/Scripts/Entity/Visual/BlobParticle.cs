using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlobParticle : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int touchParticleStartingCount;
    [SerializeField] int hitParticleStartingCount;

    [SerializeField] float maxHitSpeed;

    [Space(5)]
    [SerializeField] Vector2 percentageEffectPosOffset;

    [Header("References")]
    [SerializeField] BlobMotor motor;
    [SerializeField] BlobPhysics physics;
    [SerializeField] BlobTrigger trigger;
    [SerializeField] BlobHealth health;

    [Space(5)]
    [SerializeField] ParticleSystem dustParticlePrefab;
    Queue<ParticleCallback> dustParticles = new();

    [SerializeField] ParticleSystem deathParticlePrefab;
    Queue<ParticleCallback> deathParticles = new();
    
    [SerializeField] ParticleSystem destroyParticlePrefab;
    Queue<ParticleCallback> destroyParticles = new();

    [SerializeField] HitParticle[] hitParticles;

    [SerializeField] ParticleSystem expulseParticle;

    [SerializeField] BlobPercentageEffect percentageEffectPrefab;

    [Serializable]
    class HitParticle
    {
        public ParticleSystem particlePrefab;
        public Queue<ParticleCallback> particles = new();

        [Range(0, 100)] public float hitStrenght;

        public void OnParticleEnd(ParticleCallback particle)
        {
            particle.gameObject.SetActive(false);
            particles.Enqueue(particle);
        }
    }

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

        deathParticles.Enqueue(CreateParticle(deathParticlePrefab, OnDeathParticleEnd));

        hitParticles = hitParticles.OrderBy(x => x.hitStrenght).ToArray();
        foreach (var hitParticle in hitParticles)
            for (int j = 0; j < hitParticleStartingCount; j++)
            {
                ParticleCallback particle = CreateParticle(hitParticle.particlePrefab, hitParticle.OnParticleEnd);
                particle.SetColor(motor.GetColor().fillColor);
                hitParticle.particles.Enqueue(particle);
            }
    }

    private void Update()
    {
        expulseParticle.transform.position = physics.GetCenter();
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

    public void DoHitParticle(Vector2 position, Vector2 rotation, float speed)
    {
        if (hitParticles.Length <= 0) return;

        HitParticle hitParticle = null;
        float speedPercent = Mathf.Clamp(speed / maxHitSpeed * 100, 0, 100);

        for (int i = 0; i < hitParticles.Length; i++)
        {
            if (hitParticles[i].hitStrenght >= speedPercent)
            {
                hitParticle = hitParticles[i];
                break;
            }
        }
        if (hitParticle == null) hitParticle = hitParticles[^1];

        ParticleCallback particle;
        if (hitParticle.particles.Count <= 0) particle = CreateParticle(hitParticle.particlePrefab, hitParticle.OnParticleEnd); 
        else particle = hitParticle.particles.Dequeue();

        particle.gameObject.SetActive(true);

        particle.transform.position = position;
        particle.transform.up = -rotation;

        particle.Play();
    }

    public void EnableExpulseParticle(Vector2 rotation)
    {
        expulseParticle.transform.up = rotation;
        expulseParticle.Play();
    }
    public void DisableExpulseParticle() { expulseParticle.Stop(); }

    ParticleCallback CreateParticle(ParticleSystem prefab, Action<ParticleCallback> stopAction)
    {
        ParticleSystem particle = Instantiate(prefab, transform);

        ParticleCallback callback = particle.GetComponent<ParticleCallback>();
        callback.Setup(stopAction, particle);

        particle.gameObject.SetActive(false);

        return callback;
    }
}