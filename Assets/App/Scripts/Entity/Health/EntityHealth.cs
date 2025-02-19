using System;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] protected int maxHealth;
    public int currentHealth;

    protected bool isDead;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    public Action onTakeDamage;
    public Action onDeath;
    public Action<ContactPoint2D> onDestroy;

    //[Header("Output")]

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        onTakeDamage?.Invoke();

        if (currentHealth <= 0) Die();
    }
    public void Die()
    {
        isDead = true;
        onDeath?.Invoke();
    }

    public bool IsDead() { return isDead; }
}