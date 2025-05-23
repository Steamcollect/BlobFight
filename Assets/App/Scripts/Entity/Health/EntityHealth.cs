using System;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public int maxHealth;
    [SerializeField] private bool canTakeDamage = true;

    protected bool isInvincible = false;
    protected bool isDead = false;
    protected int currentHealth = 0;

    public Action<int> onTakeDamage;
    public Action onDeath;
    public Action OnDestroyBlob;
    public Action<ContactPoint2D> onDestroy;

    public void TakeDamage(int damage)
    {
        if(!canTakeDamage || isDead || isInvincible) return;

        currentHealth -= damage;
        onTakeDamage?.Invoke(damage);

        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        onDeath?.Invoke();
    }

    protected void Destroy(Collision2D collision)
    {
        if (isDead) return;

        isDead = true;
        OnDestroyBlob?.Invoke();
        onDestroy?.Invoke(collision.GetContact(0));
    }

    public bool IsDead() { return isDead; }
}