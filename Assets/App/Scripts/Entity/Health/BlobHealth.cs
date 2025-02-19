using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class BlobHealth : EntityHealth
{
    //[Header("Settings")]

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
    //[Header("Output")]

    [SerializeField] RSE_CameraShake rseCamShake;

    private void OnDisable()
    {
        blobTrigger.OnCollisionEnter -= OnEnterCollision;
        onDeath -= OnDeath;
        //onTakeDamage -= OnTakeDamage;
    }

    private void Start()
    {
        Setup();
        Invoke("LateStart", .05f);
    }
    void LateStart()
    {
        blobTrigger.OnCollisionEnter += OnEnterCollision;
        onDeath += OnDeath;
        //onTakeDamage += OnTakeDamage;
    }

    public void Setup()
    {
        isDead = false;
        currentHealth = maxHealth;
    }

    //void OnTakeDamage()
    //{
        
    //}

    void OnDeath()
    {
        rseCamShake.Call(shakeIntensityOnDeath, shakeTimeOnDeath);
    }

    void OnEnterCollision(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Damagable damagable))
        {
            switch (damagable.GetDamageType())
            {
                case Damagable.DamageType.Damage:
                    TakeDamage(damagable.GetDamage());
                    break;
                
                case Damagable.DamageType.Kill:
                    rseCamShake.Call(shakeIntensityOnDeath, shakeTimeOnDeath);
                    onDeath?.Invoke();
                    break;
                
                case Damagable.DamageType.Destroy:
                    rseCamShake.Call(shakeIntensityOnDeath, shakeTimeOnDeath);
                    onDestroy?.Invoke(collision.GetContact(0));
                    break;
            }
        }
    }
}