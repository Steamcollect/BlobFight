using DG.Tweening;
using TMPro;
using UnityEngine;

public class BlobPercentageEffect : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AnimationCurve sizeOverPercentage;
    [SerializeField] private AnimationCurve maxRotOverPercentage;
    [SerializeField] private float readTime;
    [SerializeField] private float disableTime;
    [SerializeField] private Vector2 posOffset;

    [Header("References")]
    [SerializeField] private BlobMotor motor;
    [SerializeField] private BlobPhysics physics;
    [SerializeField] private TMP_Text text;

    private Color color = Color.black;
    private bool isVisible = false;
    private Coroutine delayCoroutine;

    private void Start()
    {
        transform.localScale = Vector3.zero;

        Invoke("LateStart", .1f);
    }

    private void LateStart()
    {
        color = motor.GetColor().fillColor;
    }

    private void Update()
    {
        if (isVisible) transform.position = physics.GetCenter() + posOffset;
    }

    public void Update(float percentage)
    {
        transform.DOKill();
        text.DOKill();

        if (delayCoroutine != null) StopCoroutine(delayCoroutine);

        isVisible = true;

        transform.localScale = Vector3.zero;
        text.text = (percentage * 100).ToString("F0") + '%';
        text.color = color;

        float size = sizeOverPercentage.Evaluate(percentage);
        float rot = maxRotOverPercentage.Evaluate(percentage);
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-rot, rot));

        transform.BumpVisual(size, () =>
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