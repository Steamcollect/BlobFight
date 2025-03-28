using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private List<SpriteRenderer> backgroundSprites;

    [Header("Input")]
    [SerializeField] private RSE_OnFightEnd rseOnFightEnd;
    [SerializeField] private RSE_Transit rseTransit;

    private void OnEnable()
    {
        rseOnFightEnd.action += HideBackGround;
        rseTransit.action += HideBackGround;
    }

    private void OnDisable()
    {
        rseOnFightEnd.action -= HideBackGround;
        rseTransit.action -= HideBackGround;
    }

    private void Start()
    {
        foreach (var sprite in backgroundSprites)
        {
            StartCoroutine(FadeIn(sprite, 0.6f));
        }
    }

    private void HideBackGround()
    {
        foreach (var sprite in backgroundSprites)
        {
            StartCoroutine(FadeOut(sprite, 2f));
        }
    }

    private IEnumerator FadeIn(SpriteRenderer sprite, float duration)
    {
        float startAlpha = 0;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 1f, elapsed / duration);
            Color color = sprite.color;
            color.a = alpha;
            sprite.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeOut(SpriteRenderer sprite, float duration)
    {
        yield return new WaitForSeconds(1.5f);

        float startAlpha = sprite.color.a;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            Color color = sprite.color;
            color.a = alpha;
            sprite.color = color;
            yield return null;
        }
    }
}