using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TransitionLevel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool doStart;
    [SerializeField] private float delayStart;

    [Header("Input")]
    [SerializeField] private RSE_OnFightEnd rseOnFightEnd;
    [SerializeField] private RSE_Transit rseTransit;

    [Header("Output")]
    [SerializeField] private RSE_LoadNextLevel rseLoadNextLevel;
    [SerializeField] private RSE_FadeIn rseFadeIn;
    [SerializeField] private RSE_OnFightStart rseOnFightStart;
    [SerializeField] private RSE_SpawnPoint rseSpawnPoint;

    private void OnEnable()
    {
        rseOnFightEnd.action += TransitionEnd;
        rseTransit.action += TransitionEnd;
    }

    private void OnDisable()
    {
        rseOnFightEnd.action -= TransitionEnd;
        rseTransit.action -= TransitionEnd;
    }

    private void Start()
    {
        if(doStart)
        {
            gameObject.transform.position = new Vector3(-100,0,-10);

            gameObject.transform.DOMove(transform.position + new Vector3(100, 0, 0), 0.6f).OnComplete(() =>
            {
                rseSpawnPoint.Call();

                StartCoroutine(DelayStart());
            });
        }
        else
        {
            rseFadeIn.Call(null);
        }
    }

    private IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(delayStart);

        Debug.Log("Start");

        rseOnFightStart.Call();
    }

    private void TransitionEnd()
    {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.5f);

        if (doStart)
        {
            gameObject.transform.DOMove(transform.position + new Vector3(10, 0, 0), 0.6f).OnComplete(() =>
            {
                gameObject.transform.DOMove(transform.position - new Vector3(100, 0, 0), 1.4f).OnComplete(() =>
                {
                    rseLoadNextLevel.Call();
                });
            });
        }
        else
        {
            gameObject.transform.DOMove(transform.position - new Vector3(10, 0, 0), 0.6f).OnComplete(() =>
            {
                gameObject.transform.DOMove(transform.position + new Vector3(100, 0, 0), 1.4f).OnComplete(() =>
                {
                    rseLoadNextLevel.Call();
                });
            });
        }
    }
}