using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BlobHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int maxHealth;
    int currentHealth;

    bool isDead;

    [Space(10)]
    [SerializeField] float shakeIntensityOnDeath;
    [SerializeField] float shakeTimeOnDeath;

    [Header("References")] 
    [SerializeField] BlobTrigger blobTrigger;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    [Header("Output")]
    public Action OnDeath;
    public Action<ContactPoint2D> OnDestroy;

    [SerializeField] RSE_CameraShake rseCamShake;

    private void OnDisable()
    {
        blobTrigger.OnCollisionEnter -= OnEnterCollision;
    }

    private void Start()
    {
        Setup();
        Invoke("LateStart", .05f);
    }
    void LateStart()
    {
        blobTrigger.OnCollisionEnter += OnEnterCollision;
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0) Die();
    }
    void Die()
    {
        rseCamShake.Call(shakeIntensityOnDeath, shakeTimeOnDeath);
        isDead = true;
        OnDeath?.Invoke();
    }

    public bool IsDead() { return isDead; }

    public void Setup()
    {
        isDead = false;
        currentHealth = maxHealth;
    }

    void OnEnterCollision(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamagable damagable))
        {
            if (damagable.CanInstanteKill())
            {
                rseCamShake.Call(shakeIntensityOnDeath, shakeTimeOnDeath);
                OnDestroy?.Invoke(collision.GetContact(0));
            }
            else
            {
                TakeDamage(damagable.GetDamage());
            }
        }
    }
}