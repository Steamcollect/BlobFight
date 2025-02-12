using UnityEngine;
public class Damagable : MonoBehaviour, IDamagable
{
    [Header("Settings")]
    [SerializeField] int damage;
    [SerializeField] DamageType damageType;

    public enum DamageType
    {
        Damage,
        Destroy
    }

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]
    public int GetDamage()
    {
        return damage;
    }

    public bool CanInstanteKill()
    {
        return damageType == DamageType.Destroy;
    }
}