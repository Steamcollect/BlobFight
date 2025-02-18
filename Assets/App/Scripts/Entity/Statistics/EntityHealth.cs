using System;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected int maxHealth;
    protected int currentHealth;

    protected bool isDead;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    public Action onDeath;
    public Action<ContactPoint2D> onDestroy;

    //[Header("Output")]

    protected void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0) Die();
    }
    protected void Die()
    {
        isDead = true;
        onDeath?.Invoke();
    }

    public bool IsDead() { return isDead; }
}