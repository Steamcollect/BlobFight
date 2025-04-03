using System.Collections.Generic;
using UnityEngine;

public class MyWindZone : GameProps
{
    private struct WindEffect
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
            return (delayTimer += Time.fixedDeltaTime) >= delayDuration;
        }

        public void ApplyWindForce(Vector2 force)
        {
            Rb.AddForce(force);
        }
    }

    [Space(10)]
    [Header("Settings")]
    [SerializeField] private float windForce;
    [SerializeField] private float windForceDelay;

	[Header("References")]
	[SerializeField] private ParticleSystem particle;

	private bool isLaunched = false;
    private List<WindEffect> windEffects = new();

    public override void Launch()
    {
        isLaunched = true;
    }

    private void FixedUpdate()
    {
        if (!isLaunched) return;

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