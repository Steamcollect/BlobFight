using System.Collections;
using UnityEngine;

public class BlobFace : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite extendSprite;
    [SerializeField] private Sprite dashSprite;
    [SerializeField] private Sprite stunSprite;
    [SerializeField] private float dashTime;
    [SerializeField] private float stunTime;

    [Header("References")]
    [SerializeField] private BlobMovement movement;
    [SerializeField] private BlobDash dash;
    [SerializeField] private SpriteRenderer graphics;

    private Coroutine dashDelay;
    private Coroutine stunDelay;

    private void OnEnable()
    {
        movement.onExtend += SetExtend;
        movement.onShrink += SetIdle;
        dash.OnDash += SetDash;
    }

    public void SetIdle()
    {
        graphics.sprite = idleSprite;
    }

    public void SetExtend()
    {
        graphics.sprite = extendSprite;
    }

    public void SetDash()
    {
        graphics.sprite = dashSprite;

        if (dashDelay != null) StopCoroutine(dashDelay);
        dashDelay = StartCoroutine(DashDelay());
    }

    private IEnumerator DashDelay()
    {
        yield return new WaitForSeconds(dashTime);
        SetIdle();
    }

    public void SetStun()
    {
        graphics.sprite = stunSprite;

        if (stunDelay != null) StopCoroutine(stunDelay);
        stunDelay = StartCoroutine(StunDelay());
    }
    
    private IEnumerator StunDelay()
    {
        graphics.sprite = stunSprite;
        yield return new WaitForSeconds(stunTime);
        SetIdle();
    }
}