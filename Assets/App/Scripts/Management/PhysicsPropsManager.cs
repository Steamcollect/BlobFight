using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsPropsManager : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [ContextMenuItem("Get all rigidbodys in the scene", "GetAllRigidbodysFromScene"),
        SerializeField] Rigidbody2D[] rigidbodys;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_OnFightStart rseOnFightStart;

    //[Header("Output")]

    private void OnEnable()
    {
        rseOnFightStart.action += UnlockRigidbodys;
    }
    private void OnDisable()
    {
        rseOnFightStart.action -= UnlockRigidbodys;
    }

    private void Start()
    {
        LockRigidbodys();
    }

    void LockRigidbodys()
    {
        if (rigidbodys.Length <= 0) return;

        foreach (var r in rigidbodys)
        {
            r.constraints = RigidbodyConstraints2D.FreezePosition | RigidbodyConstraints2D.FreezeRotation;
        }
    }
    void UnlockRigidbodys()
    {
        if (rigidbodys.Length <= 0) return;

        foreach (var r in rigidbodys)
        {
            r.constraints = RigidbodyConstraints2D.None;
            r.velocity = Vector2.one;
        }
    }

    void GetAllRigidbodysFromScene()
    {
        rigidbodys = FindObjectsOfType<Rigidbody2D>();
    }
}