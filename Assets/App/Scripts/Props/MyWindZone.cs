using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWindZone : GameProps
{
    [Header("Settings")]
    [SerializeField] float windForce;
    [SerializeField] float windForceDelay;
    bool isLaunched = false;

    //[Header("References")]
    //List<Rigidbody2D> rbs = new();
    List<WindEffect> windEffects = new List<WindEffect>();

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    public override void Launch()
    {
        isLaunched = true;
    }

    private void FixedUpdate()
    {
        if (!isLaunched) return;

        //if(rbs.Count > 0)
        //{
        //    foreach (var rbs in rbs)
        //    {
        //        rbs.AddForce(transform.up * windForce);
        //    }
        //}
        if (windEffects.Count > 0)
        {
            for (int i = windEffects.Count - 1; i >= 0; i--)
            {
                var windEffect = windEffects[i];
                if (windEffect.CanApplyForce())
                {
                    windEffect.ApplyWindForce(transform.up * windForce);
                }
                else
                {
                    windEffects[i] = windEffect;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Rigidbody2D rb))
        {
            if (collision.TryGetComponent(out BlobPhysics blobPhysics))
            {
                blobPhysics.GetMotor().GetTrigger().OnWindEnter();
            }

            //rbs.Add(rb);
            windEffects.Add(new WindEffect(rb, windForceDelay));
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D rb))
        {
            if(collision.TryGetComponent(out BlobPhysics blobPhysics))
            {
                blobPhysics.GetMotor().GetTrigger().OnWindExit();
            }
            //rbs.Remove(rb);
            windEffects.RemoveAll(windEffect => windEffect.Rb == rb);
        }
    }

    public void SetWindForce(float windForceChange)
    {
        windForce = windForceChange;
    }

    public float GetWindForce()
    {
        return windForce;
    }
}
public struct WindEffect
{
    public Rigidbody2D Rb;
    public float delayTimer;
    public float delayDuration;

    public WindEffect(Rigidbody2D rb, float delayDuration)
    {
        Rb = rb;
        this.delayDuration = delayDuration;
        delayTimer = 0f;
    }
    public bool CanApplyForce()
    {
        if(delayTimer < delayDuration)
        {
            delayTimer += Time.fixedDeltaTime;
            return false;
        }
        return true;
    }
    public void ApplyWindForce(Vector2 force)
    {
        Rb.AddForce(force);
    }
}