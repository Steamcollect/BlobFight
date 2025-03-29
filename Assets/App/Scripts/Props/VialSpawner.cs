using DG.Tweening;
using System.Collections;
using UnityEngine;

public class VialSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float breakCooldownDelay;
    [SerializeField] private float shakeForce;
    [SerializeField] private float shakeDuration;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionTorque;
    [SerializeField] private float explosionForce;

    [Header("References")]
    [SerializeField] private Transform content;
    [SerializeField] private SpriteRenderer blobGraphics;
    [SerializeField] private SpriteRenderer vialGraphics;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Rigidbody2D[] vialPieces;
    [SerializeField] private ParticleSystem[] breakParticles;

    [Header("Input")]
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    private int breakCount = 0;
    private bool canBreak = true;
    private bool isPaused = false;
    private BlobMotor blob;

    private void OnEnable()
    {
        rseOnPause.action += Lock;
        rseOnResume.action += UnLock;
    }

    private void OnDisable()
    {
        rseOnPause.action -= Lock;
        rseOnResume.action -= UnLock;
    }

    private void Start()
    {
        breakCount = sprites.Length - 1;
        vialGraphics.sprite = sprites[breakCount];
    }

    public void Setup(BlobMotor blob)
    {
        this.blob = blob;
        blob.GetInput().breakVialInput += OnPress;
        OnPress();
    }

    private void OnPress()
    {
        if (!canBreak || isPaused) return;

        breakCount--;
        breakParticles[breakCount + 1].Play();

        if (breakCount < 0)
        {
            blob.Spawn(transform.position);
            canBreak = false;

            blobGraphics.enabled = false;
            vialGraphics.enabled = false;

            foreach (var piece in vialPieces)
            {
                piece.gameObject.SetActive(true);
            }

            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
            foreach (var hit in hits)
            {
                if(hit.TryGetComponent(out Rigidbody2D rb))
                {
                    rb.AddForce((hit.transform.position - transform.position) * explosionForce);
                    rb.AddTorque(Random.Range(-explosionTorque, explosionTorque));
                }
            }

            blob.GetInput().breakVialInput -= OnPress;

            return;
        }

        content.DOKill();
        content.DOPunchRotation(Vector3.forward * shakeForce, shakeDuration, 20, 1);
        StartCoroutine(BreakCooldown());

        vialGraphics.sprite = sprites[breakCount];
    }

    private IEnumerator BreakCooldown()
    {
        canBreak = false;

        yield return new WaitForSeconds(breakCooldownDelay);

        canBreak = true;
    }

    private void Lock()
    {
        isPaused = true;
    }

    private void UnLock()
    {
        isPaused = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}