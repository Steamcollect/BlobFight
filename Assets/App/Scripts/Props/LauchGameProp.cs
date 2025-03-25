using UnityEngine;

public class LauchGameProp : GameProps
{
    private new void OnEnable()
    {
        base.OnEnable();
    }
    private new void OnDisable()
    {
        base.OnDisable();
    }

    public override void Launch()
    {
        Rigidbody2D[] rigidbodies = GetComponentsInChildren<Rigidbody2D>();

        foreach (Rigidbody2D rb in rigidbodies)
        {
            if (rb.gameObject != gameObject)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }
}