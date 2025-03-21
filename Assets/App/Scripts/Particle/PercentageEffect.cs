using DG.Tweening;
using TMPro;
using UnityEngine;
using System;

public class PercentageEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Gradient colorOverPercentage;
    [SerializeField] AnimationCurve sizeOverPercentage;
    [SerializeField] AnimationCurve maxRotOverPercentage;

    [Space(5)]
    [SerializeField] float enableScaleTime;
    [SerializeField] float readTime;
    [SerializeField] float disableTime;

    [Header("References")]
    [SerializeField] TMP_Text text;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    public Action<PercentageEffect> OnAnimationEnd;

    public void Setup(Vector2 position, float percentage)
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;
        transform.position = position;

        float rot = maxRotOverPercentage.Evaluate(percentage);
        transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-rot, rot));

        text.text = (percentage * 100).ToString("F0") + '%';

        percentage = Mathf.Clamp01(percentage);
        text.color = colorOverPercentage.Evaluate(percentage);

        transform.DOScale(Vector3.one * sizeOverPercentage.Evaluate(percentage), enableScaleTime).OnComplete(() =>
        {
            StartCoroutine(Utils.Delay(readTime, () =>
            {
                text.DOFade(0, disableTime).OnComplete(() =>
                {
                    OnAnimationEnd.Invoke(this);
                });
            }));
        });
    }
}