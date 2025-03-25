using UnityEngine;

public class BlobVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float xShrinkScaleAtMaxSpeed;
    [SerializeField] float yShrinkScaleAtMaxSpeed;
    [SerializeField] float xExtendScaleAtMaxSpeed;
    [SerializeField] float yExtendScaleAtMaxSpeed;
    [SerializeField] float maxSpeed;

    [Space(5)]
    [SerializeField] int speedMinToSquash;

    Vector2 initScale;
    bool isExtend;

    [Header("References")]
    [SerializeField] BlobPhysics physics;
    [SerializeField] SpriteRenderer graphics;


    private void Start()
    {
        initScale = transform.localScale;
    }

    private void Update()
    {
        SquashAndStresh();
        physics.SyncColliderRadiusToVisual(transform.localScale, initScale);
    }

    void SquashAndStresh()
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
    }
    public void Hide()
    {
        graphics.enabled = false;
    }

    public void Shrink()
    {
        isExtend = false;
    }
    public void Extend()
    {
        isExtend = true;
    }

    public SpriteRenderer GetGraphics() { return graphics; }
}