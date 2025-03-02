using DG.Tweening;
using System.Collections;
using UnityEngine;

public class StartingVial : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int breakRequire;
    int breakCount;

    bool canBreak = true;
    bool isPaused = false;

    //[Header("References")]
    BlobMotor blob;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    [SerializeField] RSE_OnPause rseOnPause;
    [SerializeField] RSE_OnResume rseOnResume;

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

    void Lock()
    {
        isPaused = true;
    }

    void UnLock()
    {
        isPaused = false;
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

        breakCount++;
        if (breakCount >= breakRequire)
        {
            blob.Spawn(transform.position);
            canBreak = false;
            gameObject.SetActive(false);
            return;
        }

        transform.DOPunchRotation(Vector3.forward * 8, .5f, 20, 1);
        StartCoroutine(BreakCooldown());
    }

    IEnumerator BreakCooldown()
    {
        canBreak = false;
        yield return new WaitForSeconds(.6f);
        canBreak = true;
    }
}