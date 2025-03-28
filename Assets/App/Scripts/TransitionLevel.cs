using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TransitionLevel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool doStart;
    [SerializeField] private float delayStart;
    [SerializeField] private bool modeDev;

	[Header("Input")]
    [SerializeField] private RSE_OnFightEnd rseOnFightEnd;
    [SerializeField] private RSE_Transit rseTransit;
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    [Header("Output")]
    [SerializeField] private RSE_LoadNextLevel rseLoadNextLevel;
    [SerializeField] private RSE_FadeIn rseFadeIn;
    [SerializeField] private RSE_OnFightStart rseOnFightStart;
    [SerializeField] private RSE_SpawnPoint rseSpawnPoint;
    [SerializeField] private RSE_Message rseMessage;

    private bool isPaused = false;

    private void OnEnable()
    {
        rseOnFightEnd.action += TransitionEnd;
        rseTransit.action += TransitionEnd;
        rseOnPause.action += Pause;
        rseOnResume.action += Resume;
    }

    private void OnDisable()
    {
        rseOnFightEnd.action -= TransitionEnd;
        rseTransit.action -= TransitionEnd;
        rseOnPause.action -= Pause;
        rseOnResume.action -= Resume;
    }

    private void Pause()
    {
        isPaused = true;
        gameObject.transform.DOPause();
    }

    private void Resume()
    {
        isPaused = false;
        gameObject.transform.DOPlay();
    }

    private void Start()
    {
        if (doStart)
        {
            gameObject.transform.position = new Vector3(-100, 0, -10);

            StartCoroutine(Utils.Delay(0.1f, () => rseSpawnPoint.Call()));

            gameObject.transform.DOMove(transform.position + new Vector3(100, 0, 0), 0.6f).OnComplete(() =>
            {
                if (modeDev)
                {
                    rseMessage.Call("START!", 1f, Color.black);
                    rseOnFightStart.Call();
                }
                else
                {
                    StartCoroutine(DelayStart());
                }
            });
        }
        else
        {
            rseFadeIn.Call(null);
        }
    }

    private IEnumerator DelayStart()
    {
        float cooldown = delayStart;
        float timer = 0f;

        rseMessage.Call("READY?", 1f, Color.black);

        while (timer < cooldown)
        {
            yield return null;

            if (!isPaused)
            {
                timer += Time.deltaTime;
            }
        }

        rseMessage.Call("START!", 1f, Color.black);

        rseOnFightStart.Call();
    }

    private void TransitionEnd()
    {
        StartCoroutine(DelayEnd());
    }

    private IEnumerator DelayEnd()
    {
        float cooldown = 1.5f;
        float timer = 0f;

        while (timer < cooldown)
        {
            yield return null;

            if (!isPaused)
            {
                timer += Time.deltaTime;
            }
        }

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