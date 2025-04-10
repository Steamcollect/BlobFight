using System;
using System.Collections;
using UnityEngine;
public class WarningThunder : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool haveWarning;
    [SerializeField] private bool doFlashWarning = true;
    [SerializeField] private float delay = 1.0f;
    [SerializeField] private float minDelay = 0.1f;
    [SerializeField] private float decreaseRate = 0.05f;

    [Header("References")]
    [SerializeField] private GameObject warningVisual;

    private Coroutine warningCoroutine = null;
    private float initDelay;
    public Action<bool> onWarning;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    private void OnEnable()
    {
        onWarning += SetWarningVisibility;
    }
    private void OnDisable()
    {
        onWarning -= SetWarningVisibility;
    }
    private void Awake()
    {
        initDelay = delay;
    }
    private void SetWarningVisibility(bool visible)
    {
        if (haveWarning)
        {
            UpdateWarning(visible);
        }
    }
    private void UpdateWarning(bool seeWarning)
    {
        if (seeWarning)
        {
            if (doFlashWarning)
            {
                warningCoroutine = StartCoroutine(FlashWarning());
                doFlashWarning = false;
            }
        }
        else
        {
            if (warningCoroutine != null)
            {
                if (!doFlashWarning)
                {
                    StopCoroutine(warningCoroutine);
                    warningVisual.SetActive(false);
                    delay = initDelay;
                    doFlashWarning = true;
                }
                warningCoroutine = null;
            }
        }
    }
    IEnumerator FlashWarning()
    {
        warningVisual.SetActive(true);
        yield return new WaitForSeconds(delay);
        warningVisual.SetActive(false);
        yield return new WaitForSeconds(delay);

        delay = Mathf.Max(minDelay, delay - decreaseRate);
        warningCoroutine = StartCoroutine(FlashWarning());
    }
}