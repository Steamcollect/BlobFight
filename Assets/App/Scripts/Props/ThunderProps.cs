using System;
using System.Collections;
using UnityEngine;
public class ThunderProps : MonoBehaviour
{
    //[Header("Settings")]

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] Collider2D thunderCollider;
    public Action<ThunderProps> onEndAction;
    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    [Header("Input")]
    [SerializeField] RSE_OnPause rseOnPause;
    [SerializeField] RSE_OnResume rseOnResume;

    bool isPaused = false;

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
        isPaused = true;
        animator.speed = 0;
    }

    private void Resume()
    {
        isPaused = false;
        animator.speed = 1;
    }

    public void EnableCollider()
    {
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
}