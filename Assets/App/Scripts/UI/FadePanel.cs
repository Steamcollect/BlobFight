using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float fadeOutDuration;

	[Header("References")]
    [SerializeField] private Image circleImage;
    [SerializeField] private Image backgroundImage;

    [Header("Input")]
    [SerializeField] private RSE_FadeIn rseFadeIn;
    [SerializeField] private RSE_FadeOut rseFadeOut;

    private void OnEnable()
    {
        rseFadeIn.action += FadeIn;
        rseFadeOut.action += FadeOut;
    }

    private void OnDisable()
    {
        rseFadeIn.action -= FadeIn;
        rseFadeOut.action -= FadeOut;
    }

    private void OnDestroy()
    {
        backgroundImage?.rectTransform.DOKill();
        circleImage?.rectTransform.DOKill();
    }

    private void FadeIn(Action OnEnd = null)
    {
        backgroundImage?.rectTransform.DOKill();
        circleImage?.rectTransform.DOKill();

        float screenSize = Mathf.Max(Screen.width, Screen.height);
        backgroundImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        circleImage.rectTransform.DOSizeDelta(new Vector2(screenSize * 1.5f, screenSize * 1.5f), fadeInDuration)
            .OnUpdate(() => backgroundImage.transform.localPosition = -circleImage.transform.localPosition)
            .OnComplete(() => OnEnd?.Invoke());
    }

    private void FadeOut(Action OnEnd = null)
    {
        backgroundImage?.rectTransform.DOKill();
        circleImage?.rectTransform.DOKill();

        float screenSize = Mathf.Max(Screen.width, Screen.height);
        backgroundImage.rectTransform.sizeDelta = new Vector2(screenSize * 2.5f, screenSize * 2.5f);

        circleImage.rectTransform.DOSizeDelta(Vector2.zero, fadeOutDuration)
            .OnUpdate(() => backgroundImage.transform.localPosition = -circleImage.transform.localPosition)
            .OnComplete(() => OnEnd?.Invoke());
    }
}