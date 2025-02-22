using UnityEngine;
public class Damagable : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] int damage;
    [SerializeField] DamageType damageType;

    public enum DamageType
    {
        Damage,
        Kill,
        Destroy
    }

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
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