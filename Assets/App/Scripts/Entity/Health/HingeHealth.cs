using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class HingeHealth : EntityHealth
{
    [Header("Settings")]
    [SerializeField] bool instantDestroy;
    [SerializeField] float tempDestroy;

    [Header("References")]
    [SerializeField, ContextMenuItem("Get All Joints In Object", "GetAllJoints")] List<HingeJoint2D> joints = new List<HingeJoint2D>();

    [Space(10)]
    [SerializeField] SpriteRenderer graphics;
    [SerializeField] Color initColor;
    [SerializeField] Color endColor;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void OnEnable()
    {
        onTakeDamage += OnTakeDamage;
        onDeath += OnDeath;
    }
    private void OnDisable()
    {
        onTakeDamage -= OnTakeDamage;
        onDeath -= OnDeath;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    void OnTakeDamage(int damageTaken)
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

    IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(tempDestroy);

        gameObject.SetActive(false);
    }

    void OnDeath()
    {
        graphics.color = endColor;

        if(instantDestroy)
        {
            StartCoroutine(DestroyDelay());
        }
        else
        {
            for (int i = 0; i < joints.Count; i++)
            {
                joints[i].enabled = false;
            }
        }
    }
}