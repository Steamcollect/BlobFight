using UnityEngine;
using UnityEngine.Rendering;

public class Damagable : MonoBehaviour
{
    public enum DamageType
    {
        Damage,
        Kill,
        Destroy
    }

    [Header("Settings")]
    [SerializeField] float percentageGiven;
    [SerializeField] float pushBackForce;
    [SerializeField] private DamageType damageType;

    public DamageType GetDamageType()
    {
        return damageType;
    }

    public float GetPushBackForce()
    {
        return pushBackForce;
    }
    public float GetDamage()
    {
        return percentageGiven;
    }

    public void SetScripts(int damageVal, DamageType type)
    {
        //damage = damageVal;
        damageType = type;
    }
}