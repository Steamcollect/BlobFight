using UnityEngine;
public class BlobStamina : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float maxStamina;
    float currentStamina;

    [SerializeField] float staminaGivenPerSec;

    bool canGetStamina = true;

    //[Header("References")]

    //[Space(10)]
    // RSO
    // RSF
    // RSP

    //[Header("Input")]
    //[Header("Output")]

    public void Setup()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (!canGetStamina) return;

        if (currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
        else
        {
            currentStamina += staminaGivenPerSec * Time.deltaTime;
        }
    }

    public void RemoveStamina(float stamina)
    {
        currentStamina -= stamina;
        if (currentStamina < 0) currentStamina = 0;
    }

    public bool HaveEnoughStamina(float staminaRequire) { return currentStamina >= staminaRequire; }
    public float GetStamina() {  return currentStamina; }
    public void EnableStaminaRecuperation() { canGetStamina = true;}
    public void DisableStaminaRecuperation() { canGetStamina = false;}
}