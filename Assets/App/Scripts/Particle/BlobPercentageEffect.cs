using DG.Tweening;
using TMPro;
using UnityEngine;
using System;
using System.Collections;

public class BlobPercentageEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] AnimationCurve sizeOverPercentage;
    [SerializeField] AnimationCurve maxRotOverPercentage;
    Color color;

    [Space(5)]
    [SerializeField] float readTime;
    [SerializeField] float disableTime;

    [Space(5)]
    [SerializeField] Vector2 posOffset;

    [Header("References")]
    [SerializeField] BlobMotor motor;
    [SerializeField] BlobPhysics physics;

    [Space(5)]
    [SerializeField] TMP_Text text;

    bool isVisible = false;

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    private void Start()
    {
        transform.localScale = Vector3.zero;

        Invoke("LateStart", .1f);
    }
    void LateStart()
    {
        color = motor.GetColor().fillColor;
    }

    private void Update()
    {
        if (isVisible) transform.position = physics.GetCenter() + posOffset;
    }

    Coroutine delayCoroutine;
    public void Setup(float percentage)
    {
        transform.DOKill();
        text.DOKill();

        if (delayCoroutine != null) StopCoroutine(delayCoroutine);

        isVisible = true;

        transform.localScale = Vector3.zero;

        float rot = maxRotOverPercentage.Evaluate(percentage);
        transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-rot, rot));
        print(transform.eulerAngles.z);

        text.text = (percentage * 100).ToString("F0") + '%';

        text.color = color;

        transform.BumpVisual(sizeOverPercentage.Evaluate(percentage), () =>
        {
            delayCoroutine = StartCoroutine(Utils.Delay(readTime, () =>
            {
                text.DOFade(0, disableTime).OnComplete(() =>
                {
                    isVisible = false;
                });
            }));
        });
    }
}