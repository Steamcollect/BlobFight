using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class BlobParticle : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int touchParticleStartingCount;
    [SerializeField] int touchExtendParticleStartingCount;
    [SerializeField] int hitParticleStartingCount;
    [SerializeField] int parryParticleStartingCount;
    [SerializeField] int dustDashParticleStartingCount;
    [SerializeField] int lavaBrunDust;
    [SerializeField] float maxHitSpeed;

    [HideInInspector] public bool extendDustParticle;

    [Header("References")]
    [SerializeField] BlobMotor motor;
    [SerializeField] BlobPhysics physics;
    [SerializeField] BlobTrigger trigger;
    [SerializeField] BlobHealth health;

    [Space(10)]
    [SerializeField] ParticleSystem dustParticlePrefab;
    [SerializeField] ParticleSystem dustExtendParticlePrefab;
    [SerializeField] ParticleSystem dustDashParticlePrefab;
    [SerializeField] ParticleSystem deathParticlePrefab;
    [SerializeField] ParticleSystem destroyParticlePrefab;
    [SerializeField] ParticleSystem expulseParticle;
    [SerializeField] ParticleSystem parryParticlePrefab;
    [SerializeField] ParticleSystem lavaBrunDustPrefab;

    [Space(10)]
    [SerializeField] HitParticle[] hitParticles;

    Queue<ParticleCallback> dustParticles = new();
    Queue<ParticleCallback> dustExtendParticles = new();
    Queue<ParticleCallback> dustDashParticles = new();
    Queue<ParticleCallback> deathParticles = new();
    Queue<ParticleCallback> destroyParticles = new();
    Queue<ParticleCallback> parryParticles = new();
    Queue<ParticleCallback> lavaBrunDustParticles = new();

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
    
    [Space(10)]
    [SerializeField] RSO_MainCamera rsoMainCamera;

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

        for (int i = 0; i < touchExtendParticleStartingCount; i++)
            dustExtendParticles.Enqueue(CreateParticle(dustExtendParticlePrefab, OnDustExtendParticleEnd));

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
        if (coll.transform.CompareTag("Blob"))
        {
            extendDustParticle = false;
            return;
        }
        
        if (extendDustParticle)
        {
            DustExtendParticle(coll.GetContact(0).point, coll.GetContact(0).normal);
            extendDustParticle = false;
        }
        else
        {
            DustParticle(coll.GetContact(0).point, coll.GetContact(0).normal);
        }
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

    void DustExtendParticle(Vector2 position, Vector2 rotation)
    {
        ParticleCallback particle;
        if (dustExtendParticles.Count <= 0) particle = CreateParticle(dustExtendParticlePrefab, OnDustExtendParticleEnd);
        else particle = dustExtendParticles.Dequeue();
        particle.gameObject.SetActive(true);
        particle.transform.position = position;
        particle.transform.up = rotation;
        particle.Play();
    }
    void OnDustExtendParticleEnd(ParticleCallback particle)
    {
        particle.gameObject.SetActive(false);
        dustExtendParticles.Enqueue(particle);
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
        particle.SetColor(color.fillColor);

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

        particle.transform.position = GetTheClosestScreenCoordinate(contact.point);
        particle.transform.up = contact.normal;
        particle.SetColor(color.fillColor);

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

    public void ParryParticle(Vector2 position, Vector2 rotation)
    {
        ParticleCallback particle;
        if (parryParticles.Count <= 0) particle = CreateParticle(parryParticlePrefab, OnParryParticleEnd);
        else particle = parryParticles.Dequeue();

        particle.gameObject.SetActive(true);

        particle.transform.position = position;
        particle.transform.up = rotation;

        particle.Play();
    }
    void OnParryParticleEnd(ParticleCallback particle)
    {
        particle.gameObject.SetActive(false);
        parryParticles.Enqueue(particle);
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

    public void LavaBrunDustParticle(Vector2 position, Vector2 rotation)
    {
        ParticleCallback particle;
        if (lavaBrunDustParticles.Count <= 0) particle = CreateParticle(lavaBrunDustPrefab, OnLavaBrunDustParticleEnd);
        else particle = lavaBrunDustParticles.Dequeue();
        particle.gameObject.SetActive(true);
        particle.transform.position = position;
        particle.transform.up = rotation;
        particle.Play();
    }
    void OnLavaBrunDustParticleEnd(ParticleCallback particle)
    {
        particle.gameObject.SetActive(false);
        lavaBrunDustParticles.Enqueue(particle);
    }

    private ParticleCallback CreateParticle(ParticleSystem prefab, Action<ParticleCallback> stopAction)
    {
        ParticleSystem particle = Instantiate(prefab, transform);

        ParticleCallback callback = particle.GetComponent<ParticleCallback>();
        callback.Setup(stopAction, particle);

        particle.gameObject.SetActive(false);

        return callback;
    }

    Vector2 GetTheClosestScreenCoordinate(Vector2 worldPosition)
    {
        if (rsoMainCamera.Value == null)
        {
            Debug.LogError("Main Camera not found");
            return worldPosition;
        }

        // Convertir la position en screen space
        Vector3 screenPos = rsoMainCamera.Value.WorldToScreenPoint(worldPosition);

        // Clamper aux bords de l'écran
        Vector2 clampedScreenPos = screenPos;
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float leftDist = screenPos.x;
        float rightDist = screenWidth - screenPos.x;
        float bottomDist = screenPos.y;
        float topDist = screenHeight - screenPos.y;

        float minDist = Mathf.Min(leftDist, rightDist, bottomDist, topDist);

        if (minDist == leftDist)
            clampedScreenPos.x = 0;
        else if (minDist == rightDist)
            clampedScreenPos.x = screenWidth;
        else if (minDist == bottomDist)
            clampedScreenPos.y = 0;
        else
            clampedScreenPos.y = screenHeight;

        // Reconversion en world space
        Vector3 worldEdgePos = rsoMainCamera.Value.ScreenToWorldPoint(new Vector3(clampedScreenPos.x, clampedScreenPos.y, screenPos.z));
        return new Vector2(worldEdgePos.x, worldEdgePos.y);
    }
}