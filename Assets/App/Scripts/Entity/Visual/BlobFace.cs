using System.Collections;
using UnityEngine;

public class BlobFace : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Sprite idleSprite;
    [SerializeField] Sprite extendSprite;
    [SerializeField] Sprite dashSprite;
    [SerializeField] Sprite stunSprite;

    [Space(10)]
    [SerializeField] float dashTime;
    [SerializeField] float stunTime;

    [Header("References")]
    [SerializeField] BlobMovement movement;
    [SerializeField] BlobDash dash;

    [Space(10)]
    [SerializeField] SpriteRenderer graphics;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

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

    Coroutine dashDelay;
    IEnumerator DashDelay()
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
    Coroutine stunDelay;
    IEnumerator StunDelay()
    {
        graphics.sprite = stunSprite;
        yield return new WaitForSeconds(stunTime);
        SetIdle();
    }
}