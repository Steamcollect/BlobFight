using DG.Tweening;
using System.Collections;
using UnityEngine;

public class VialSpawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float breakCooldownDelay = .5f;

    [Space(5)]
    [SerializeField] float shakeForce = 8;
    [SerializeField] float shakeDuration = .5f;

    [Space(5)]
    [SerializeField] float explosionRadius;
    [SerializeField] float explosionTorque;
    [SerializeField] float explosionForce;

    int breakCount;

    bool canBreak = true;
    bool isPaused = false;

    [Header("References")]
    BlobMotor blob;

    [SerializeField] Transform content;

    [Space(5)]
    [SerializeField] SpriteRenderer blobGraphics;
    [SerializeField] SpriteRenderer vialGraphics;
    [SerializeField] Sprite[] sprites;

    [Space(5)]
    [SerializeField] Rigidbody2D[] vialPieces;
    [SerializeField] ParticleSystem[] breakParticles;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_OnPause rseOnPause;
    [SerializeField] RSE_OnResume rseOnResume;
    
    //[Header("Output")]

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

    void OnPress()
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

            return;
        }

        content.DOKill();
        content.DOPunchRotation(Vector3.forward * shakeForce, shakeDuration, 20, 1);
        StartCoroutine(BreakCooldown());

        vialGraphics.sprite = sprites[breakCount];
    }

    IEnumerator BreakCooldown()
    {
        canBreak = false;
        yield return new WaitForSeconds(breakCooldownDelay);
        canBreak = true;
    }

    void Lock()
    {
        isPaused = true;
    }
    void UnLock()
    {
        isPaused = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}