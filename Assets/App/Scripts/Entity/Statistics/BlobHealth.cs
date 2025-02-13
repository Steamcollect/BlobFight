using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BlobHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int maxHealth;
    int currentHealth;

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
        OnDeath?.Invoke();
    }

    public void Setup()
    {
        currentHealth = maxHealth;
    }

    void OnEnterCollision(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamagable damagable))
        {
            if (damagable.CanInstanteKill())
            {
                OnDestroy?.Invoke(collision.GetContact(0));
            }
            else
            {
                TakeDamage(damagable.GetDamage());
            }
        }
    }
}