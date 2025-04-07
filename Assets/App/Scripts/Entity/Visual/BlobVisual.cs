using UnityEngine;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;

public class BlobVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float xShrinkScaleAtMaxSpeed;
    [SerializeField] private float yShrinkScaleAtMaxSpeed;
    [SerializeField] private float xExtendScaleAtMaxSpeed;
    [SerializeField] private float yExtendScaleAtMaxSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private int speedMinToSquash;
    [SerializeField] float shakeStenght;
    [SerializeField] float shakeTime;


    [Header("References")]
    [SerializeField] private BlobPhysics physics;
    [SerializeField] private SpriteRenderer graphics;
    [SerializeField] private SpriteRenderer fill;
    [SerializeField] private SpriteRenderer stamina;

    private Vector2 initScale = Vector2.zero;
    private bool isExtend = false;
    private Vector3 pos = Vector3.zero;

    private void Start()
    {
        initScale = transform.localScale;
        pos = stamina.transform.localPosition;
    }

    private void Update()
    {
        SquashAndStresh();
        physics.SyncColliderRadiusToVisual(transform.localScale, initScale);
    }

    private void SquashAndStresh()
    {
        Vector2 velocity = physics.GetVelocity();
        float speed = velocity.magnitude;

        if (speed > speedMinToSquash)
        {
            float t = Mathf.Clamp01(speed / maxSpeed);

            float targetX = Mathf.Lerp(1, isExtend ? xExtendScaleAtMaxSpeed : xShrinkScaleAtMaxSpeed, t);
            float targetY = Mathf.Lerp(1, isExtend ? yExtendScaleAtMaxSpeed : yShrinkScaleAtMaxSpeed, t);

            transform.localScale = initScale * new Vector2(targetX, targetY);
        }
        else
        {
            transform.localScale = initScale;
        }

        if (velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void SetColor(Color color)
    {
        graphics.color = color;
    }

    public void Show()
    {
        graphics.enabled = true;
        fill.enabled = true;
        stamina.enabled = true;
    }

    public void Hide()
    {
        graphics.enabled = false;
        fill.enabled = false;
        stamina.enabled = false;
    }

    public void Shrink()
    {
        isExtend = false;
    }

    public void Extend()
    {
        isExtend = true;
    }

    public void Shake()
    {
        stamina.transform.DOShakePosition(shakeTime, shakeStenght, 10, 90).OnComplete(() =>
        {
            stamina.transform.localPosition = pos;
        });
    }

    public SpriteRenderer GetGraphics() { return graphics; }
}