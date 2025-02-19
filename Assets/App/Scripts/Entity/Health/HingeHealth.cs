using UnityEngine;
using UnityEngine.Rendering;

public class HingeHealth : EntityHealth
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField, ContextMenuItem("Get All Joints In Object", "GetAllJoints")] HingeJoint2D[] joints;

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

    void OnTakeDamage()
    {
        graphics.color = Color.Lerp(endColor, initColor, (float)currentHealth / (float)maxHealth);
    }
    void OnDeath()
    {
        graphics.color = endColor;
        for (int i = 0; i < joints.Length; i++)
        {
            joints[i].enabled = false;
        }
    }

    void GetAllJoints()
    {
        joints = GetComponents<HingeJoint2D>();
    }
}