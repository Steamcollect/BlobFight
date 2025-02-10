using UnityEngine;
public class Damagable : MonoBehaviour, IDamagable
{
    [Header("Settings")]
    [SerializeField] int damage;

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
}