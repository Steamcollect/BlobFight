using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool playStart;

    [Header("References")]
    [SerializeField] private List<SpriteRenderer> backgroundSprites;

    [Header("Input")]
    [SerializeField] private RSE_OnFightEnd rseOnFightEnd;
    [SerializeField] private RSE_Transit rseTransit;
    [SerializeField] private RSE_OnPause rseOnPause;
    [SerializeField] private RSE_OnResume rseOnResume;

    private bool isPaused = false;

    private void OnEnable()
    {
        rseOnFightEnd.action += HideBackGround;
        rseTransit.action += HideBackGround;
        rseOnPause.action += Pause;
        rseOnResume.action += Resume;
    }

    private void OnDisable()
    {
        rseOnFightEnd.action -= HideBackGround;
        rseTransit.action -= HideBackGround;
        rseOnPause.action -= Pause;
        rseOnResume.action -= Resume;
    }

    private void Start()
    {
        if (playStart)
        {
            foreach (var sprite in backgroundSprites)
            {
                if (sprite != null)
                {
                    StartCoroutine(FadeIn(sprite, 0.4f));
                }
            }
        }
    }

    private void Pause()
    {
        isPaused = true;
    }

    private void Resume()
    {
        isPaused = false;
    }

    private void HideBackGround()
    {
        foreach (var sprite in backgroundSprites)
        {
            if (sprite != null)
            {
                StartCoroutine(FadeOut(sprite, 1f));
            }
        }
    }

    private IEnumerator FadeIn(SpriteRenderer sprite, float duration)
    {
        float startAlpha = 0;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if(!isPaused)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, 1f, elapsed / duration);
                Color color = sprite.color;
                color.a = alpha;
                sprite.color = color;
                yield return null;
            }
        }
    }

    private IEnumerator FadeOut(SpriteRenderer sprite, float duration)
    {
        yield return new WaitForSeconds(1.4f);

        float startAlpha = sprite.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if(!isPaused)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
                Color color = sprite.color;
                color.a = alpha;
                sprite.color = color;
            }

            yield return null;
        }
    }
}