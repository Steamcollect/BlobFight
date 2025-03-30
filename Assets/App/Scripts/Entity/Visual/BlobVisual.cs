using UnityEngine;

public class BlobVisual : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float xShrinkScaleAtMaxSpeed;
    [SerializeField] private float yShrinkScaleAtMaxSpeed;
    [SerializeField] private float xExtendScaleAtMaxSpeed;
    [SerializeField] private float yExtendScaleAtMaxSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private int speedMinToSquash;

    [Header("References")]
    [SerializeField] private BlobPhysics physics;
    [SerializeField] private SpriteRenderer graphics;

    private Vector2 initScale = Vector2.zero;
    private bool isExtend = false;

    private void Start()
    {
        initScale = transform.localScale;
    }

    private void Update()
    {
        SquashAndStresh();
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