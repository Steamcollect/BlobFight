using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeHealth : EntityHealth
{
    [Space(10)]
    [Header("Settings")]
    [SerializeField] private bool instantDestroy;
    [SerializeField] private Color initColor;
    [SerializeField] private Color endColor;

    [Header("References")]
    [SerializeField] private SpriteRenderer graphics;
    [SerializeField] private List<HingeJoint2D> joints;

    [Header("Input")]
    [SerializeField] private RSE_OnFightStart rseOnFightStart;

    private void OnEnable()
    {
        onTakeDamage += OnTakeDamage;
        onDeath += OnDeath;
        rseOnFightStart.action += DestroyInstant;
    }

    private void OnDisable()
    {
        onTakeDamage -= OnTakeDamage;
        onDeath -= OnDeath;
        rseOnFightStart.action -= DestroyInstant;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnTakeDamage(int damageTaken)
    {
        graphics.color = Color.Lerp(endColor, initColor, (float)currentHealth / (float)maxHealth);
    }

    public void SetHingeJoint(HingeJoint2D hingeJoint2D)
    {
        joints.Add(hingeJoint2D);
    }

    public void SetHingeColor(SpriteRenderer spriteRenderer, Color32 color, Color32 color1)
    {
        graphics = spriteRenderer;
        initColor = color;
        endColor = color1;
    }

    private void DestroyInstant()
    {
        if (instantDestroy)
        {
            StartCoroutine(Delay());
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.2f);

        gameObject.SetActive(false);
    }

    private void OnDeath()
    {
        graphics.color = endColor;

        foreach (var joint in joints)
        {
            joint.enabled = false;
        }
    }
}