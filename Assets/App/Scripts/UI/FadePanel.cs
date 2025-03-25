using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float fadeInDuration;
    [SerializeField] float fadeOutDuration;

    [Header("References")]
    [SerializeField] Image circleImage;
    [SerializeField] Image backgroundImage;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    [Header("Input")]
    [SerializeField] RSE_FadeIn rseFadeIn;
    [SerializeField] RSE_FadeOut rseFadeOut;

    //[Header("Output")]

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

    void OnDestroy()
    {
        backgroundImage?.rectTransform.DOKill();
        circleImage?.rectTransform.DOKill();
    }

    void FadeIn(Action OnEnd = null)
    {
        backgroundImage.rectTransform.DOKill();

        backgroundImage.rectTransform.sizeDelta = Vector2.zero;
        float screenSize = Mathf.Max(Screen.width, Screen.height);

        backgroundImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        circleImage.rectTransform.DOSizeDelta(new Vector2(screenSize * 1.5f, screenSize * 1.5f), fadeInDuration)
            .OnUpdate(() =>
            {
                if (backgroundImage && circleImage)
                {
                    backgroundImage.transform.localPosition = -circleImage.transform.localPosition;
                }
            })
            .OnComplete(() =>
            {
                OnEnd?.Invoke();
            });
    }
    void FadeOut(Action OnEnd = null)
    {
        /*backgroundImage.rectTransform.DOKill();

        float screenSize = Mathf.Max(Screen.width, Screen.height);
        backgroundImage.rectTransform.sizeDelta = new Vector2(screenSize * 2.5f, screenSize * 2.5f);

        circleImage.rectTransform.DOSizeDelta(Vector2.zero, fadeOutDuration)
            .OnUpdate(() =>
        {
            backgroundImage.transform.localPosition = -circleImage.transform.localPosition;
            backgroundImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        })
            .OnComplete(() =>
            {
                OnEnd?.Invoke();
            });*/
    }
}