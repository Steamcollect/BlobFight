using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlobParticle : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int touchParticleStartingCount;
    [SerializeField] private int hitParticleStartingCount;
    [SerializeField] private int dustDashParticleStartingCount;
    [SerializeField] private float maxHitSpeed;

    [Header("References")]
    [SerializeField] private BlobMotor motor;
    [SerializeField] private BlobPhysics physics;
    [SerializeField] private BlobTrigger trigger;
    [SerializeField] private BlobHealth health;
    [SerializeField] private ParticleSystem dustParticlePrefab;
    [SerializeField] private ParticleSystem dustDashParticlePrefab;
    [SerializeField] private ParticleSystem deathParticlePrefab;
    [SerializeField] private ParticleSystem destroyParticlePrefab;
    [SerializeField] private ParticleSystem expulseParticle;
    [SerializeField] private HitParticle[] hitParticles;

    private Queue<ParticleCallback> dustParticles = new();
    private Queue<ParticleCallback> dustDashParticles = new();
    private Queue<ParticleCallback> deathParticles = new();
    private Queue<ParticleCallback> destroyParticles = new();

    [Serializable]
    private class HitParticle
    {
        [Range(0, 100)] public float hitStrenght = 0;

        public ParticleSystem particlePrefab = null;
        public Queue<ParticleCallback> particles = new();

        public void OnParticleEnd(ParticleCallback particle)
        {
            particle.gameObject.SetActive(false);
            particles.Enqueue(particle);
        }
    }

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
            dustParticles.Enqueue(CreateParticle(dustParticlePrefab, OnDustParticleEnd));

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

    private void OnTouchEnter(Collision2D coll)
    {
        if (coll.transform.CompareTag("Blob")) return;
        DustParticle(coll.GetContact(0).point, coll.GetContact(0).normal);
    }

    private void OnTouchExit(ContactPoint2D contact)
    {
        if (contact.collider.transform.CompareTag("Blob")) return;
        DustParticle(contact.point, contact.normal);
    }

    public void DustParticle(Vector2 position, Vector2 direction)
    {
        ParticleCallback particle;
        if (dustParticles.Count <= 0) particle = CreateParticle(dustParticlePrefab, OnDustParticleEnd);
        else particle = dustParticles.Dequeue();

        particle.gameObject.SetActive(true);

        particle.transform.position = position;
        particle.transform.up = direction;

        particle.Play();
    }

    private void OnDustParticleEnd(ParticleCallback particle)
    {
        particle.gameObject.SetActive(false);
        dustParticles.Enqueue(particle);
    }

    public void DustDashParticle(Vector2 position, Vector2 direction)
    {
        ParticleCallback particle;
        if (dustDashParticles.Count <= 0) particle = CreateParticle(dustDashParticlePrefab, OnDustDashParticleEnd);
        else particle = dustDashParticles.Dequeue();

        particle.gameObject.SetActive(true);

        particle.transform.position = position;
        particle.transform.up = direction;

        particle.Play();
    }

    private void OnDustDashParticleEnd(ParticleCallback particle)
    {
        particle.gameObject.SetActive(false);
        dustDashParticles.Enqueue(particle);
    }

    public void DeathParticle(Vector2 position, BlobColor color)
    {
        ParticleCallback particle;
        if (deathParticles.Count <= 0) particle = CreateParticle(deathParticlePrefab, OnDeathParticleEnd);
        else particle = deathParticles.Dequeue();

        particle.gameObject.SetActive(true);

        particle.transform.position = position;

        particle.Play();
    }

    private void OnDeathParticleEnd(ParticleCallback particle)
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

        particle.Play();
    }

    private void OnDestroyParticleEnd(ParticleCallback particle)
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
        particle.transform.up = rotation;

        particle.Play();
    }

    public void EnableExpulseParticle(Vector2 rotation)
    {
        expulseParticle.transform.up = rotation;
        expulseParticle.Play();
    }

    public void DisableExpulseParticle() { expulseParticle.Stop(); }

    public void SetExpulseParticleRotation(Vector2 rotation)
    {
        expulseParticle.transform.up = rotation;
    }

    private ParticleCallback CreateParticle(ParticleSystem prefab, Action<ParticleCallback> stopAction)
    {
        ParticleSystem particle = Instantiate(prefab, transform);

        ParticleCallback callback = particle.GetComponent<ParticleCallback>();
        callback.Setup(stopAction, particle);

        particle.gameObject.SetActive(false);

        return callback;
    }
}