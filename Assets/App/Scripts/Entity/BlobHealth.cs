using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BlobHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int maxHealth;
    int currentHealth;

    bool isinvincible = false;

    List<Collision2D> collisions = new();

    [Header("References")]
    [SerializeField] BlobJoint blobJoint;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    [Header("Output")]
    [SerializeField] RSE_OnBlobDeath rseOnBlobDeath;

    private void OnDisable()
    {
        blobJoint.RemoveOnCollisionEnterListener(OnEnterCollision);
        blobJoint.RemoveOnCollisionExitListener(OnExitCollision);
    }

    private void Start()
    {
        Setup();
        Invoke("LateStart", .05f);
    }
    void LateStart()
    {
        blobJoint.AddOnCollisionEnterListener(OnEnterCollision);
        blobJoint.AddOnCollisionExitListener(OnExitCollision);
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0) Die();
    }
    void Die()
    {
        rseOnBlobDeath.Call(blobJoint);
    }

    void Setup()
    {
        currentHealth = maxHealth;
    }

    void OnEnterCollision(Collision2D collision)
    {
        if (isinvincible) return;

        if(collisions.Find(x => x == collision) == null)
        {
            if (collision.gameObject.TryGetComponent(out IDamagable damagable))
            {
                TakeDamage(damagable.GetDamage());
            }
        }

        collisions.Add(collision);
    }
    void OnExitCollision(Collision2D collision)
    {
        collisions.Remove(collision);
    }
}