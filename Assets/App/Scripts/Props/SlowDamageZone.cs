using UnityEngine;

public class SlowDamageZone : CollisionTrigger
{
    [Header("Settings")]
    [SerializeField] private bool doDamage;
    [SerializeField] private bool doSlowMovement;
    [SerializeField] private bool doSlowDash;

    private void OnEnable()
    {
        OnTriggerEnterWithBlob += OnBlobTriggerEnter;
        OnTriggerExitWithBlob += OnBlobTriggerExit;
    }

    private void OnDisable()
    {
        OnTriggerEnterWithBlob -= OnBlobTriggerEnter;
        OnTriggerExitWithBlob -= OnBlobTriggerExit;
    }

    private void OnBlobTriggerEnter(BlobMotor blobMotor)
    {
        Debug.Log("TriggerEnter");
    }

    private void OnBlobTriggerExit(BlobMotor blobMotor)
    {
        Debug.Log("TriggerExit");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnEnterTrigger(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnExitTrigger(collision);
    }
}