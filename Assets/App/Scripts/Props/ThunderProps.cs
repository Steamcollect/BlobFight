using System;
using UnityEngine;

public class ThunderProps : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D thunderCollider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ThunderAudio thunderAudio;
    [SerializeField] private WarningThunder warningThunder;
    
    [Header("Input")]
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    public Action<ThunderProps> onEndAction;
    public Action<bool> setWarning;
    private int randomSpawn;

    [Header("Output")]
    [SerializeField] RSE_LightFlash rseLightFlash;
    
    private void OnEnable()
    {
        rseOnPause.action += Pause;
        rseOnResume.action += Resume;
    }

    private void OnDisable()
    {
        rseOnPause.action -= Pause;
        rseOnResume.action -= Resume;
    }

    private void Pause()
    {
        animator.speed = 0;
    }

    private void Resume()
    {
        animator.speed = 1;
    }

    public void EnableCollider()
    {
        rseLightFlash.Call();
        thunderCollider.enabled = true;
    }

    public void DisableCollider()
    {
        thunderCollider.enabled = false;
    }

    public void OnEndAnimation()
    {
        gameObject.SetActive(false);
        onEndAction.Invoke(this);
    }

    public void Flip(bool flipX)
    {
        spriteRenderer.flipX = flipX;
    }

    public void LightningSound()
    {
        thunderAudio.PlayLightningSound();
    }
    public void SparkleSound()
    {
        thunderAudio.PlaySparkleSound();
    }

    public void SetRandomSpawn(int index)
    {
        randomSpawn = index;
    }

    public int GetRandomSpawn()
    {
        return randomSpawn;
    }
    public void SetWarningVisible()
    {
        warningThunder.onWarning.Invoke(true);
    }
    public void SetWarningInvisible()
    {
        warningThunder.onWarning.Invoke(false);
    }
}