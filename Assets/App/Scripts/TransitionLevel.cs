using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TransitionLevel : MonoBehaviour
{
    //[Header("Settings")]

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    [SerializeField] private RSE_OnFightEnd rseOnFightEnd;
    [SerializeField] private RSE_LoadNextLevel rseLoadNextLevel;

    [SerializeField] private bool doStart;

    private void OnEnable()
    {
        rseOnFightEnd.action += TransitionEnd;
    }

    private void OnDisable()
    {
        rseOnFightEnd.action -= TransitionEnd;
    }

    private void Start()
    {
        if(doStart)
        {
            gameObject.transform.position = new Vector3(-100,0,-10);

            gameObject.transform.DOMove(transform.position + new Vector3(100, 0, 0), 0.6f).OnComplete(() =>
            {
            });
        }
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