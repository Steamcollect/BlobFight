using UnityEngine;

public class CustomCollision : MonoBehaviour
{
    void OnCollision(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Damagable damagable))
        {
            switch (damagable.GetDamageType())
            {
                case Damagable.DamageType.Destroy:
                    gameObject.SetActive(false);
                    break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollision(collision);
    }
}