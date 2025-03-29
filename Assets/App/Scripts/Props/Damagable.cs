using UnityEngine;

public class Damagable : MonoBehaviour
{
    public enum DamageType
    {
        Damage,
        Kill,
        Destroy
    }

    [Header("Settings")]
    [SerializeField] private int damage;
    [SerializeField] private DamageType damageType;

    public DamageType GetDamageType()
    {
        return damageType;
    }

    public int GetDamage()
    {
        return damage;
    }

    public void SetScripts(int damageVal, DamageType type)
    {
        damage = damageVal;
        damageType = type;
    }
}